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

    public PlayerMovement playerMovement;
    public GameObject player;
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
        gridManager = FindObjectOfType<GridManager>();
        gridManager.GenerateGrid();

        pathfinding = new Pathfinding(gridManager);

        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();

        cage = GameObject.Find("Square");
        CageRenderer cageRenderer = FindObjectOfType<CageRenderer>();
        cageRenderer.render = false;

        if (playerMovement != null)
        {
            playerMovement.Initialize(cage);
        }

        Chaser chaser = FindObjectOfType<Chaser>();
        chaser.Initialize(gridManager, player.transform);
        chaser.chase = false;
        chaser.chaserRender = false;

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

        if (trial.EOB)
        {
            Time.timeScale = 0;
            Debug.Log("Fixation Trial");
            ShowMiddleScreen("+");
            yield return new WaitForSecondsRealtime(5.0f);
            Time.timeScale = 1;
            HideMiddleScreen();
            isTrialRunning = false;
            Debug.Log("Trial ended.");
            yield break;
        }

        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.SetInitialPosition(trial.startX, trial.startY);

        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;

        CageRenderer cageRenderer = FindObjectOfType<CageRenderer>();
        if (cageRenderer != null)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            cageRenderer.render = trial.cageRender;
            cageRenderer.SetColor(trial.isGreen ? Color.green : Color.white);
        }

        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            chaser.chaserRender = trial.predRender;
        }

        if (!trial.showQuestionScreen)
        {
            yield return new WaitForSecondsRealtime(2.0f);
        }

        if (trial.showQuestionScreen)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(2.0f);
            Time.timeScale = 1;

            ShowMiddleScreen(trial.questionText + ":" + playerQuestionInput.ToString());
            yield return StartCoroutine(WaitForAnyKey());
            HideMiddleScreen();
        }

        Debug.Log("Trial started with parameters: " + trial.ToString());
        showStartTrialScreen("Trial Started");
        yield return new WaitForSecondsRealtime(2.0f);

        chaser.chase = trial.predChase;
        playerMovement.EnableMovement();

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

        isTrialRunning = false;
        playerMovement.DisableMovement();

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

        userVectorInputs = playerMovement.ExportInputRecords();
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
        float horizontalInput = Input.GetAxis("Horizontal");

        while (!Input.anyKeyDown)
        {
            if (horizontalInput < 0)
            {
                playerQuestionInput -= 1;
            }
            else if (horizontalInput > 0)
            {
                playerQuestionInput += 1;
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
