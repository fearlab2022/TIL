using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TILTask : MonoBehaviour
{

    public MapEvents mapEvents;
    public SessionGenerator sessionGenerator;


    public GameObject arena;
    public GameObject fixation;
    public GameObject[] threatStateEnvi;
    public GridMap[] GridMap;

    public GameObject player;
    public GameObject predator;


    private float trialOnsetTime;
    private float trialOffsetTime;

    private TILTrial trial;
    public bool trialEndable;
    public bool shouldStopCoroutine = false;
    private Coroutine freeMovementCoroutine; // Reference to store the coroutine
    private float freeMovementTime;

    
    public enum GameState
        {
            SpawningPeriod,
            SubjectiveResponsePeriod,
            FreeMovementPeriod,
            EndingPeriod,
            NextTrialPeriod
        }




    public void StartNextTrial()
    {
        int trialNum = PlayerPrefs.GetInt("trialNum");
        trial = sessionGenerator.trials[trialNum];
        trialOnsetTime = Time.realtimeSinceStartup;
        HandleGameState(GameState.SpawningPeriod);


    }
    public void HandleGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.SpawningPeriod:
                    StartCoroutine(HandleSpawnSequence(trial));
                    break;
                case GameState.SubjectiveResponsePeriod:
                    break;
                case GameState.FreeMovementPeriod:
                    BeginFreeMovement(trial);
                    break;
                case GameState.EndingPeriod:
                    EndTrial();
                    break;
                case GameState.NextTrialPeriod:
                    OnTrialEnd();
                    break;
            }
        }
    

    IEnumerator HandleSpawnSequence(TILTrial trial)
    {
        hideFixation();
        arena.SetActive(true);
        threatStateEnvi[trial.threatState].SetActive(true);
        spawnPredator(trial.predatorState);
        yield return new WaitForSeconds(1.0f);
        spawnPlayer(trial.preyPosX, trial.preyPosY, player);
        yield return new WaitForSeconds(1.0f);
        GameState gameState = trial.stimType == 1 ? GameState.SubjectiveResponsePeriod : GameState.FreeMovementPeriod;
        HandleGameState(gameState);
    }

    
    public void spawnPlayer(int x, int y, GameObject obj)
    {
        Vector3 pos = GridMap[x].rowdata[y].transform.position;
        obj.transform.position = pos;
    }  

    public void spawnPredator(int predatorState)
    {
        if(predatorState == 0) predator.SetActive(false);
        else
        {
            predator.GetComponent<TIL_Predator>().setColor(predatorState);
            predator.SetActive(true);
        }
    }


    #region FreeMovementPeriod

    void BeginFreeMovement(TILTrial trial)
    {
        player.GetComponent<TIL_PlayerManager>().enableMovement();
        predator.GetComponent<TIL_Predator>().calculateAttack(trial.attackingSteps);

        // Start free movement and store the reference
        if (freeMovementCoroutine == null)
        {
            shouldStopCoroutine = false;  // Reset the flag before starting the coroutine
            freeMovementCoroutine = StartCoroutine(FreeMovement());
        }

    }

     IEnumerator FreeMovement()
    {

        float elapsedTime = 0f;
        while (elapsedTime < freeMovementTime)
        {
            if (shouldStopCoroutine) // Check if the coroutine should stop
            {
                Debug.Log("Coroutine stopped");
                yield break;  // Exit the coroutine immediately
            }

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Check if the trial is still endable after movement
        if (trialEndable && !shouldStopCoroutine)
        {
            HandleGameState(GameState.EndingPeriod);
        }

        freeMovementCoroutine = null;  // Reset reference when finished
    }

    #endregion

    #region Ending a Trial
    
    public void EndTrial()
    {
        
        // Set flag to stop the coroutine at the next check
        shouldStopCoroutine = true;

        // Ensure that the free movement coroutine is stopped
        if (freeMovementCoroutine != null)
        {
            StopCoroutine(freeMovementCoroutine);
            freeMovementCoroutine = null;
        }

        trialEndable = false;
        trialOffsetTime = Time.realtimeSinceStartup;
        StartCoroutine(TrialEndRoutine(trial));

    }


     IEnumerator TrialEndRoutine(TILTrial trial)
    {
        LogTrialData(trial);
        showFixation();
        yield return new WaitForSeconds(2.0f);
        ResetTrialSettings();
        HandleGameState(GameState.NextTrialPeriod);
    }

    public void OnTrialEnd()
    {
        int trialNum = PlayerPrefs.GetInt("trialNum");

        Debug.Log("pushing trial data");
        sessionGenerator.pushTrialData(trial);

        trialNum++;
        PlayerPrefs.SetInt("trialNum", trialNum);

        if (trialNum < sessionGenerator.numTrials)
        {
            Debug.Log($"trialNum {trialNum}");
            StartNextTrial();
        }
        else
        {
            Application.Quit();
            Debug.Log("End reached");
        }
    }

    private void ResetTrialSettings()
    {
        player.GetComponent<TIL_PlayerManager>().resetPlayer();
        predator.GetComponent<TIL_Predator>().resetPredator();
    }

    #endregion 

    #region Helpers and Tools


    void showFixation()
    {
        arena.SetActive(false);
        fixation.SetActive(true);
    }

    void hideFixation()
    {
        arena.SetActive(true);
        fixation.SetActive(false);
    }


    void LogTrialData(TILTrial trial)
    {
        //log the data from the trial here
    }
    #endregion








}
