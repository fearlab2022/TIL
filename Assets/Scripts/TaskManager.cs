using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public Dictionary<float, Vector2> userVectorInputs;

    // Private variables
    private List<Trial> trials;
    private int currentTrialIndex = 0;
    private bool isTrialRunning = false;
    private Vector3 chaserStartPosition = new Vector3(4, 4, 0);
    private SessionGenerator sessionGenerator;
    private float playerQuestionInput = 0;

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
        // Initialize and generate the grid
        gridManager = FindObjectOfType<GridManager>();
        gridManager.GenerateGrid();
        gridManager.SetNewLavaTile();

        // Initialize pathfinding, player, cage, and chaser
        pathfinding = new Pathfinding(gridManager);

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found!");
            return;
        }

        cage = GameObject.Find("Square");
        if (cage == null)
        {
            Debug.LogError("Cage GameObject not found!");
            return;
        }

        Player playerScript = player.GetComponent<Player>();
        if (playerScript == null)
        {
            Debug.LogError("Player script component not found on the player GameObject!");
            return;
        }

        playerScript.Initialize(cage);

        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        if (cageRenderer != null)
        {
            cageRenderer.render = false;
        }

        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            chaser.Initialize(gridManager, player.transform);
            chaser.chase = false;
            chaser.chaserRender = false;
        }

        sessionGenerator = FindObjectOfType<SessionGenerator>();
    }

    public void InitializeTrials(List<Trial> trialList)
    {
        trials = trialList;

        if (trials.Count > 0)
        {
            StartCoroutine(RunTrials());
        }
        else
        {
            Debug.LogError("No trials found.");
        }
    }

    private IEnumerator RunTrials()
    {
        while (currentTrialIndex < trials.Count)
        {
            Trial currentTrial = trials[currentTrialIndex];
            Debug.Log("Starting Trial: " + (currentTrialIndex + 1));

            SetChaserStartPosition();
            yield return StartCoroutine(StartTrial(currentTrial));

            currentTrialIndex++;
        }

        Debug.Log("All trials completed.");
    }

    private void SetChaserStartPosition()
    {
        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            chaser.transform.position = chaserStartPosition;
        }
    }

    private IEnumerator StartTrial(Trial trial)
    {
        isTrialRunning = true;

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
            Debug.Log("Trial ended.");
            yield break;
        }

        // Transform player to initial position for trial
        playerScript.SetInitialPosition(trial.startX, trial.startY);
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;

        // Initialize and show the cage first
        CageRenderer cageRenderer = cage.GetComponent<CageRenderer>();
        if (cageRenderer != null)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            cageRenderer.render = trial.cageRender;
            cageRenderer.SetColor(trial.isGreen ? Color.green : Color.white);
        }

        // Show chaser next
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
        Debug.Log("Trial started with parameters: " + trial.ToString());
        showStartTrialScreen("Trial Started");
        yield return new WaitForSecondsRealtime(2.0f);

        // Update the lava, allow player to move and chaser to chase
        StartCoroutine(gridManager.UpdateLavaTile());
        playerScript.EnableMovement();
        chaser.chase = trial.predChase;

        // Make the trial end when chaser catches player, if not a chase trial, end the trial in 10s
        if (trial.predChase)
        {
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
        }

        // Trial is finished and player cannot move
        isTrialRunning = false;
        playerScript.DisableMovement();

        Debug.Log("Trial ended.");
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
        sessionGenerator.PushDataToDatabase(currentTrialIndex, userVectorInputs);
        Debug.Log(userVectorInputs);
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

    private IEnumerator WaitForAnyKey()
    {
        playerQuestionInput = 0;
        ShowMiddleScreen("This is a question: " + playerQuestionInput.ToString());

        while (true)
        {
            float horizontalInput = Input.GetAxis("Horizontal");

            if (horizontalInput < 0)
            {
                playerQuestionInput -= 1;
                ShowMiddleScreen("This is a question: " + playerQuestionInput.ToString());
            }
            else if (horizontalInput > 0)
            {
                playerQuestionInput += 1;
                ShowMiddleScreen("This is a question: " + playerQuestionInput.ToString());
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
