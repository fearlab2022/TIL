using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;

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
    public float startTime;
    public float playerShowTimestamp;
    public float cageShowTimestamp;
    public float predShowTimestamp;
    public float playerMoveTimestamp;
    public float questionScreenTimestamp;
    public float endTime;

    public float TR = 1;



    // Private variables
    private List<TIL_Trial> trials;
    private int currentTrialIndex = 0;
    private bool isTrialRunning = false;
    private Vector3 chaserStartPosition = new Vector3(4, 4, 0);
    private float playerQuestionInput = 0;
    private float playerInLavaTime = 0f;
    private bool isPlayerInLava = false;

    public TimingDescription time = null;
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
        Player playerScript = player.GetComponent<Player>();
        playerScript.Initialize(cage);
    }

    private void InitializeCageRenderer()
    {
        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        cageRenderer.render = false;
        
    }

    private void InitializeChaser()
    {
        Chaser chaser = FindObjectOfType<Chaser>();

        chaser.Initialize(gridManager, player.transform);
        chaser.chase = false;
        chaser.chaserRender = false;
        
    }

    public void InitializeTrials(List<TIL_Trial> trialList)
    {
        trials = trialList;
        StartCoroutine(RunTrialsCoroutine());

    }

    private IEnumerator RunTrialsCoroutine()
    {
        while (currentTrialIndex < trials.Count)
        {

            TIL_Trial currentTrial = trials[currentTrialIndex];
            UnityEngine.Debug.Log("Starting Trial: " + (currentTrialIndex + 1));

            if (currentTrial.EOB)
                {
                    Time.timeScale = 0;
                    ShowMiddleScreen("+");
                    yield return new WaitForSecondsRealtime(5.0f);
                    Time.timeScale = 1;
                    HideMiddleScreen();
                    isTrialRunning = false;
                    UnityEngine.Debug.Log("Trial ended.");
                }

            else {
                SetChaserStartPosition();
                yield return StartCoroutine(TIL_Main(currentTrial));
            }

            currentTrialIndex++;
        }

        UnityEngine.Debug.Log("All trials completed.");
    }

    private void SetChaserStartPosition()
    {
        Chaser chaser = FindObjectOfType<Chaser>();

        chaser.transform.position = chaserStartPosition;
    }

    private IEnumerator TIL_Main(TIL_Trial trial)
    {
        
        List<TimingDescription> timings = new List<TimingDescription>();

        startTime = Time.realtimeSinceStartup;
        time.setDescription("Start Time");
        time.setTiming(startTime);
        timings.Add(time.ShallowCopy());

        List<PlayerVector> positionDataList = new List<PlayerVector>();
        List<JoystickInput> joystickInputList = new List<JoystickInput>();
        isTrialRunning = true;
        
        // render player

        Player playerScript = player.GetComponent<Player>();
        playerScript.DeactivatePlayer();
        playerScript.DisableMovement();
        playerScript.SetInitialPosition(trial.startX, trial.startY);
        playerScript.ActivatePlayer();
        yield return new WaitForSeconds(2.0f * TR);
        playerShowTimestamp = Time.realtimeSinceStartup;

        time.setDescription("Player Render Time");
        time.setTiming(playerShowTimestamp);
        timings.Add(time.ShallowCopy());

        
        // wait between player and cage
        yield return new WaitForSeconds(2.0f * TR);

        // render cage

        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        if (trial.cageRender)
        {

            cageRenderer.SetActive();
            cageRenderer.render = true;
            cageRenderer.SetColor(trial.isGreen ? Color.green : Color.white);
            cageShowTimestamp = Time.realtimeSinceStartup;
            

            
        }
        time.setDescription("Cage Render Time");
        time.setTiming(cageShowTimestamp);
        timings.Add(time.ShallowCopy());

        // wait between cage and chaser
        yield return new WaitForSeconds(2.0f * TR);


        // render predator
        Chaser chaser = FindObjectOfType<Chaser>();

        yield return new WaitForSeconds(2.0f);
        chaser.chaserRender = trial.predRender;
        predShowTimestamp = Time.realtimeSinceStartup;

        time.setDescription("Predator Render Time");
        time.setTiming(predShowTimestamp);
        timings.Add(time.ShallowCopy());

        


        // wait between predator and next step
        yield return new WaitForSeconds(2.0f * TR);


        if (trial.showQuestionScreen)
        {
            questionScreenTimestamp = Time.realtimeSinceStartup;
            ShowMiddleScreen(trial.questionText + ":" + playerQuestionInput.ToString());
            yield return StartCoroutine(WaitForAnyKey(trial.questionText));
            HideMiddleScreen();

            // wait before starting chase and movement
            yield return new WaitForSeconds(2.0f * TR);
        }

        time.setDescription("Question Screen Timestamp");
        time.setTiming(questionScreenTimestamp);
        timings.Add(time.ShallowCopy());
    

       
        
        // player can move
        playerInLavaTime = 0f;
        isPlayerInLava = false;
        playerScript.ChangeColor(Color.cyan);
        playerMoveTimestamp = Time.realtimeSinceStartup;
        
        InvokeRepeating("runGridLavaUpdate", 0, 2f);
        InvokeRepeating("CheckPlayerInLavaZone", 0, 0.2f);

        time.setDescription("Player Move Timestamp");
        time.setTiming(playerMoveTimestamp);
        timings.Add(time.ShallowCopy());

        
        playerScript.startRecording(positionDataList, joystickInputList);
        playerScript.EnableMovement();
        
        


        // predator starts chasing
        chaser.chase = trial.predChase;


        // run trial until predator catches or until 10 seconds
        if (trial.predChase)
            {
                float trialDuration = 10.0f;
                float elapsedTime = 0.0f;

                while (chaser != null && chaser.chase && elapsedTime < trialDuration)
                {
                    hideStartTrialScreen();
                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                if (elapsedTime >= trialDuration)
                {
                    
                    UnityEngine.Debug.Log("Trial ended after 10 seconds.");
                    
                }
                else if (!chaser.chase)
                {
                    UnityEngine.Debug.Log("Predator caught the player.");
                }
            }

        else
        {
            float trialDuration = 10.0f;
            hideStartTrialScreen();
            yield return new WaitForSeconds(trialDuration);
        }

        


        chaser.chaserRender = false;
        chaser.chase = false;
        // end of trial data handeling and reseting variables for next trial
        playerScript.StopRecording();
        endTime = Time.realtimeSinceStartup;
        time.setDescription("Ending Time");
        time.setTiming(endTime);
        timings.Add(time.ShallowCopy());
        CancelInvoke();

        EndTrial(trial, playerScript, chaser, cageRenderer, positionDataList, trials.IndexOf(trial), timings, joystickInputList, playerInLavaTime);
        yield return new WaitForSeconds(2.0f);
        hideStartTrialScreen();
        yield return new WaitForSeconds(2.0f);
    }

    private void EndTrial(TIL_Trial trial, Player playerScript, Chaser chaser, CageRenderer cageRenderer, List<PlayerVector> positionDataList, int index, List<TimingDescription> timings,  List<JoystickInput> joystickInputList, float playerInLavaTime)
    {
        playerScript.ChangeColor(Color.blue);
        isTrialRunning = false;
        playerScript.DisableMovement();
        playerScript.DeactivatePlayer();
        cageRenderer.SetColor(Color.white);
        cageRenderer.render = false;
        
        chaser.transform.position = new Vector3(4,4, chaser.transform.position.z);

        List<PlayerVector> chaserPositionData = chaser.GetChaserPositionData();
        chaser.ClearChaserPositionData();

        sessionGenerator.PushDataToDatabase(trial, index, playerQuestionInput, positionDataList, timings, joystickInputList, playerInLavaTime, chaserPositionData);


    }

    public void runGridLavaUpdate() {
        gridManager.SetNewLavaTile();
    }


    private void CheckPlayerInLavaZone()
    {


            Vector3 playerPosition = player.transform.position;
            Tile playerTile = gridManager.GetTileAt((int)playerPosition.x, (int)playerPosition.y);

            bool playerInLavaNow = gridManager.GetSurroundingLavaTiles().Contains(playerTile);
            
            if (playerInLavaNow)
            {
                if (!isPlayerInLava)
                {
                    isPlayerInLava = true;
                }
                playerInLavaTime += 0.2f; 
            }
            else
            {
                if (isPlayerInLava)
                {
                    isPlayerInLava = false;
                }
            }

        
    }
    



    private void ShowMiddleScreen(string text)
    {
        if (middleScreenCanvas != null && middleScreenText != null)
        {
            middleScreenText.text = text;
            middleScreenCanvas.SetActive(true);
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

}