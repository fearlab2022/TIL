using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    private List<Trial> trials;
    private int currentTrialIndex = 0;
    private bool isTrialRunning = false;

    public GameObject middleScreenCanvas;
    public TextMeshProUGUI middleScreenText;

    private GridManager gridManager;
    private Vector3 chaserStartPosition = new Vector3(4, 4, 0); 
    public PlayerMovement playerMovement;
    public GameObject player;
    public Pathfinding pathfinding;
    public GameObject cage;

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
        GameObject cage = GameObject.Find("Square");

        if (playerMovement != null && player != null)
        {
            playerMovement.Initialize(cage);
        }
        Chaser chaser = FindObjectOfType<Chaser>();
        chaser.Initialize(gridManager, player.transform);
        

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
    
 
 
    Chaser chaser = FindObjectOfType<Chaser>();
    if (chaser != null)
    {
        chaser.chaserRender = trial.predRender;
        
    }

    CageRenderer cageRenderer = FindObjectOfType<CageRenderer>();
    if (cageRenderer != null)
    {
        cageRenderer.render = trial.cageRender;
        cageRenderer.SetColor(trial.isGreen ? Color.green : Color.white);
    }

    if (trial.showQuestionScreen)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2.0f);
        Time.timeScale = 1;

        ShowMiddleScreen(trial.questionText);
        yield return StartCoroutine(WaitForAnyKey());
        HideMiddleScreen();
    }

    Debug.Log("Trial started with parameters: " + trial.ToString());

    chaser.chase = trial.predChase;
    playerMovement.EnableMovement();
    
    if (trial.predChase)
    {
        while (chaser != null && chaser.chase)
        {
            yield return null;
        }
    }
    else
    {
        float trialDuration = 10.0f; 
        yield return new WaitForSeconds(trialDuration);
    }

    if (cageRenderer != null)
    {
        cageRenderer.SetColor(Color.white);
    }

    isTrialRunning = false;


    playerMovement.DisableMovement();
    
    Debug.Log("Trial ended.");
}

    private void ShowMiddleScreen(string text)
    {
        if (middleScreenCanvas != null && middleScreenText != null)
        {
            middleScreenText.text = text;
            middleScreenCanvas.SetActive(true);
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
        while (!Input.anyKeyDown)
        {
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
