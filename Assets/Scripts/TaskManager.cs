using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    // Public variables
    public static TaskManager Instance;
    public GameObject middleScreenCanvas;
    public GameObject startTrialScreenCanvas;
    public TextMeshProUGUI startTrialScreenText;
    public TextMeshProUGUI middleScreenText;

    public GameObject player; // Reference to the player GameObject
    public Pathfinding pathfinding;
    public GameObject cage;
    public GridManager gridManager;
    private Stopwatch stopwatch;

    public Dictionary<float, Vector2> userVectorInputs;

    // Private variables
    private List<Trial> trials;
    private int currentTrialIndex = 0;
    private bool isTrialRunning = false;
    private Vector3 chaserStartPosition = new Vector3(4, 4, 0);
    private SessionGenerator sessionGenerator;
    private float playerQuestionInput = 0;

    private bool endStopWatch = false;

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
        


    
        // TODO:   Remove this and make grid static object.

        // Initialize and generate the grid
        gridManager = FindObjectOfType<GridManager>();
        gridManager.GenerateGrid();
        gridManager.SetNewLavaTile();
        stopwatch = new Stopwatch();

        // Initialize pathfinding, player, cage, and chaser
        pathfinding = new Pathfinding(gridManager);



        // TODO: Remove this and assign player as a public variable 
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            UnityEngine.Debug.LogError("Player GameObject not found!");
            return;
        }
        // TODO: Remove this and assign cage as a public variable 
        cage = GameObject.Find("Square");
        if (cage == null)
        {
            UnityEngine.Debug.LogError("Cage GameObject not found!");
            return;
        }

        //Let's make this a  method under: InitializePlayer()

        Player playerScript = player.GetComponent<Player>();
        if (playerScript == null)
        {
            UnityEngine.Debug.LogError("Player script component not found on the player GameObject!");
            return;
        }

        playerScript.Initialize(cage);


        // Let's make this a method under InitializeCageRenderer()
        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        if (cageRenderer != null)
        {
            cageRenderer.render = false;
        }
        // Let's make this a method under InitializeChaser()
        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            chaser.Initialize(gridManager, player.transform);
            chaser.chase = false;
            chaser.chaserRender = false;
        }

        //TODO: Assign this as a public var.
        sessionGenerator = FindObjectOfType<SessionGenerator>();
    }


    // This belongs in SessionGenerator
    public void InitializeTrials(List<Trial> trialList)
    {
        trials = trialList;

        if (trials.Count > 0)
        {
            StartCoroutine(RunTrials());
        }
        else
        {
            UnityEngine.Debug.LogError("No trials found.");
        }
    }


    //TODO: Change this to be a method

    private IEnumerator RunTrials()
    {
        while (currentTrialIndex < trials.Count)
        {
            Trial currentTrial = trials[currentTrialIndex];
            UnityEngine.Debug.Log("Starting Trial: " + (currentTrialIndex + 1));

            SetChaserStartPosition();
            yield return StartCoroutine(StartTrial(currentTrial));

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


    // Change name to be TIL_main
    private IEnumerator StartTrial(Trial trial)
    {
        isTrialRunning = true;

        //This should be already assigned at start. no need to have it here
        Player playerScript = player.GetComponent<Player>();
        playerScript.DisableMovement();  // Ensure movement is disabled at the start

        // Fixation screen
        if (trial.EOB)
        {
            Time.timeScale = 0;
            ShowMiddleScreen("+");
            yield return new WaitForSecondsRealtime(5.0f);
            Time.timeScale = 1;
            HideMiddleScreen();
            isTrialRunning = false;
            UnityEngine.Debug.Log("Trial ended.");
            yield break; // do you really need to break here?
        }

        // Transform player to initial position for trial
        playerScript.SetInitialPosition(trial.startX, trial.startY);
        //TODO:  I would disable/enable the whole object not just the sprite renderer
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;

        // Initialize and show the cage first
        //TODO: Let's just setactive to true/false for the gameobject. Better to minimize script interactions
        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        if (cageRenderer != null)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            cageRenderer.render = trial.cageRender;
            cageRenderer.SetColor(trial.isGreen ? Color.green : Color.white);
        }

        // Show chaser next
        //TODO: this should already be assigned already. No need to reassign
        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            chaser.chaserRender = trial.predRender;
        }

        // If a question trial, show the position of player for a few seconds then ask question
        if (trial.showQuestionScreen)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(2.0f);
            Time.timeScale = 1;

            ShowMiddleScreen(trial.questionText + ":" + playerQuestionInput.ToString());
            yield return StartCoroutine(WaitForAnyKey());
            HideMiddleScreen();
        }

        // Once all players are rendered in then start the trial
        UnityEngine.Debug.Log("Trial started with parameters: " + trial.ToString());

        //TODO: Let's indicate that a player can move by showing a light on the player. See predation game for reference
        showStartTrialScreen("Trial Started");
        yield return new WaitForSecondsRealtime(2.0f);



        // Update the lava, allow player to move and chaser to chase
        StartCoroutine(gridManager.UpdateLavaTile());
        playerScript.EnableMovement();
        chaser.chase = trial.predChase;

        //TODO: THE TRIAL Should always end in 10 secs. If the predator catches sooner end it early
        // Make the trial end when chaser catches player, if not a chase trial, end the trial in 10s
        if (trial.predChase)
        {
            stopwatch.Reset();
            stopwatch.Start();
            endStopWatch = true;
            while (chaser != null && chaser.chase)
            {
                
                hideStartTrialScreen();
                yield return null;
            }
        }
        else
        {
            float trialDuration = 10.0f;
            hideStartTrialScreen();
            yield return new WaitForSeconds(trialDuration);
            trialTime = 10;
        }

        

    
        // Trial is finished and player cannot move
        isTrialRunning = false;
        playerScript.DisableMovement();
        if(endStopWatch) {
            stopwatch.Stop();
            TimeSpan timespan = stopwatch.Elapsed;
            trialTime = (float) timespan.TotalSeconds;
            endStopWatch = false;
        }

        UnityEngine.Debug.Log("Trial ended.");
        showStartTrialScreen("Trial ended.");
        yield return new WaitForSecondsRealtime(2.0f);

        if (cageRenderer != null)
        {
            cageRenderer.SetColor(Color.white);
        }
        chaser.chaserRender = false;
        cageRenderer.render = false;
        hideStartTrialScreen();

        userVectorInputs = playerScript.ExportInputRecords();
        sessionGenerator.PushDataToDatabase(currentTrialIndex, userVectorInputs, playerQuestionInput, trialTime);
        UnityEngine.Debug.Log(userVectorInputs);
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



//They don't have a keyboard in fMRI let's make the scale a visual analog scale w/ left right axis response and trigger for registration
    private IEnumerator WaitForAnyKey()
{
    playerQuestionInput = 0;
    float inputDelay = 0.5f; 
    float lastInputTime = 0f;
    ShowMiddleScreen("This is a question: " + playerQuestionInput.ToString());

    while (true)
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Time.time - lastInputTime >= inputDelay)
        {
            if (horizontalInput < 0)
            {
                playerQuestionInput = Mathf.Max(playerQuestionInput - 1, 0);
                lastInputTime = Time.time; 
                ShowMiddleScreen("This is a question: " + playerQuestionInput.ToString());
            }
            else if (horizontalInput > 0)
            {
                playerQuestionInput = Mathf.Min(playerQuestionInput + 1, 10);
                lastInputTime = Time.time; 
                ShowMiddleScreen("This is a question: " + playerQuestionInput.ToString());
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
