using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
 
    public static TaskManager Instance;
    public GameObject middleScreenCanvas;
    public GameObject startTrialScreenCanvas;
    public TextMeshProUGUI startTrialScreenText;
    public TextMeshProUGUI middleScreenText;
    public GameObject player; 
    public GameObject cage; 
    public GridManager gridManager;
    public SessionGenerator sessionGenerator; 
    public Pathfinding pathfinding;
    public DateTime startTime;
    public DateTime endTime;

    public Dictionary<float, Vector2> userVectorInputs;

    // Private variables
    private List<Trial> trials;
    private int currentTrialIndex = 0;
    private bool isTrialRunning = false;
    private Vector3 chaserStartPosition = new Vector3(4, 4, 0);
    private float playerQuestionInput = 0;
    private float trialTime = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Initialize and generate the grid
        gridManager.GenerateGrid();
        gridManager.SetNewLavaTile();

        // Initialize pathfinding
        pathfinding = new Pathfinding(gridManager);

        // Initialize player, cage, and chaser
        InitializePlayer();
        InitializeCageRenderer();
        InitializeChaser();
    }

    private void InitializePlayer()
    {
        
        if (player == null)
        {
            UnityEngine.Debug.LogError("Player GameObject not assigned!");
            return;
        }

        Player playerScript = player.GetComponent<Player>();
        if (playerScript == null)
        {
            UnityEngine.Debug.LogError("Player script component not found on the player GameObject!");
            return;
        }

        playerScript.Initialize(cage);
    }

    private void InitializeCageRenderer()
    {
        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        if (cageRenderer != null)
        {
            cageRenderer.render = false;
        }
    }

    private void InitializeChaser()
    {
        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            chaser.Initialize(gridManager, player.transform);
            chaser.chase = false;
            chaser.chaserRender = false;
        }
    }

    public void InitializeTrials(List<Trial> trialList)
    {
        trials = trialList;

        if (trials.Count > 0)
        {
            StartCoroutine(RunTrialsCoroutine());
        }
        else
        {
            UnityEngine.Debug.LogError("No trials found.");
        }
    }

    private IEnumerator RunTrialsCoroutine()
    {
        while (currentTrialIndex < trials.Count)
        {
            Trial currentTrial = trials[currentTrialIndex];
            UnityEngine.Debug.Log("Starting Trial: " + (currentTrialIndex + 1));

            SetChaserStartPosition();
            yield return StartCoroutine(StartTrialCoroutine(currentTrial));

            currentTrialIndex++;
        }

        UnityEngine.Debug.Log("All trials completed.");
    }

    private void SetChaserStartPosition()
    {
        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            chaser.transform.position = chaserStartPosition;
        }
    }

    private IEnumerator StartTrialCoroutine(Trial trial)
    {
        List<PlayerVector> positionDataList = new List<PlayerVector>();
        List<JoystickInput> joystickInputList = new List<JoystickInput>();
        isTrialRunning = true;

        Player playerScript = player.GetComponent<Player>();
        playerScript.DeactivatePlayer();
        playerScript.DisableMovement();

        if (trial.EOB)
        {
            Time.timeScale = 0;
            ShowMiddleScreen("+");
            yield return new WaitForSeconds(5.0f);  
            Time.timeScale = 1;
            HideMiddleScreen();
            isTrialRunning = false;
            UnityEngine.Debug.Log("Trial ended.");
            yield break;
        }

        playerScript.SetInitialPosition(trial.startX, trial.startY);

        yield return new WaitForSeconds(2.0f);  
        playerScript.ActivatePlayer();

        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        if (trial.cageRender)
        {
            yield return new WaitForSeconds(2.0f);  
            cageRenderer.SetActive();
            cageRenderer.render = true;
            cageRenderer.SetColor(trial.isGreen ? Color.green : Color.white);
        }

        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            yield return new WaitForSeconds(2.0f); 
            chaser.chaserRender = trial.predRender;
        }

        if (trial.showQuestionScreen)
        {
            Time.timeScale = 0;
            yield return new WaitForSeconds(2.0f); 
            Time.timeScale = 1;

            ShowMiddleScreen(trial.questionText + ":" + playerQuestionInput.ToString());
            yield return StartCoroutine(WaitForAnyKey(trial.questionText));
            HideMiddleScreen();
        }

        showStartTrialScreen("Trial Starts Now!");
        yield return new WaitForSeconds(1.0f); 
        UnityEngine.Debug.Log("Trial started with parameters: " + trial.ToString());

        startTime = DateTime.Now;

        playerScript.startRecording(positionDataList, joystickInputList);

        StartCoroutine(gridManager.UpdateLavaTile());
        playerScript.EnableMovement();
        chaser.chase = trial.predChase;

        float trialDuration = 10.0f;
        float elapsedTime = 0.0f;

        while (elapsedTime < trialDuration)
        {
            if (chaser != null && !chaser.chase) 
            {
                break;
            }

            hideStartTrialScreen();
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        playerScript.StopRecording();
        endTime = DateTime.Now;
        EndTrial(trial, playerScript, chaser, cageRenderer, positionDataList, trials.IndexOf(trial), startTime, endTime, joystickInputList);
    }

    private void EndTrial(Trial trial, Player playerScript, Chaser chaser, CageRenderer cageRenderer, List<PlayerVector> positionDataList, int index, DateTime startTime, DateTime endTime, List<JoystickInput> joystickInputList)
    {
        isTrialRunning = false;
        playerScript.DisableMovement();
        playerScript.DeactivatePlayer();
        cageRenderer.SetColor(Color.white);
        cageRenderer.render = false;
        chaser.chaserRender = false;

        UnityEngine.Debug.Log("Trial ended.");
        showStartTrialScreen("Trial ended.");
        StartCoroutine(HideStartTrialScreenWithDelay());

        
        // add logic for pushing data
        String startTimeString = startTime.ToString();
        String endTimeString = endTime.ToString();

        sessionGenerator.PushDataToDatabase(trial, index, playerQuestionInput, trialTime, positionDataList, startTimeString, endTimeString, joystickInputList);

    }
    

    private IEnumerator HideStartTrialScreenWithDelay()
    {
        yield return new WaitForSeconds(2.0f);
        hideStartTrialScreen();
    }

    private void ShowMiddleScreen(string text)
    {
        if (middleScreenCanvas != null && middleScreenText != null)
        {
            middleScreenText.text = text;
            middleScreenCanvas.SetActive(true);
        }
    }

    private void showStartTrialScreen(string text)
    {
        if (startTrialScreenCanvas != null && startTrialScreenText != null)
        {
            
            startTrialScreenText.text = text;
            startTrialScreenCanvas.SetActive(true);
            UnityEngine.Debug.Log("test flag 1");
        }
    }

    private void hideStartTrialScreen()
    {
        if (startTrialScreenCanvas != null)
        {
            startTrialScreenCanvas.SetActive(false);
        }
    }

    private void HideMiddleScreen()
    {
        if (middleScreenCanvas != null)
        {
            middleScreenCanvas.SetActive(false);
        }
    }

    private IEnumerator WaitForAnyKey(string text)
    {
        playerQuestionInput = 0;
        float inputDelay = 0.5f;
        float lastInputTime = 0f;
        ShowMiddleScreen(text + playerQuestionInput.ToString());

        while (true)
        {
            float horizontalInput = Input.GetAxis("Horizontal");

            if (Time.time - lastInputTime >= inputDelay)
            {
                if (horizontalInput < 0)
                {
                    playerQuestionInput = Mathf.Max(playerQuestionInput - 1, 0);
                    lastInputTime = Time.time;
                    ShowMiddleScreen(text + playerQuestionInput.ToString());
                }
                else if (horizontalInput > 0)
                {
                    playerQuestionInput = Mathf.Min(playerQuestionInput + 1, 10);
                    lastInputTime = Time.time;
                    ShowMiddleScreen(text + playerQuestionInput.ToString());
                }
            }

            if (Input.anyKeyDown)
            {
                break;
            }

            yield return null;
        }
    }

    public void EndTrial()
    {
        if (!isTrialRunning) return;

        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            chaser.chase = false;
        }

        isTrialRunning = false;
    }
}
