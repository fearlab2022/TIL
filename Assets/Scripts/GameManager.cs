using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public string csvFileName = "conditions"; 
    private List<Trial> trials;
    private int currentTrialIndex = 0;
    private GridManager gridManager;
    private Vector3 chaserStartPosition = new Vector3(4, 4, 0); 
    private bool isTrialRunning = false;

    public GameObject middleScreenCanvas; 
    public TextMeshProUGUI middleScreenText; 

    private int questionResponse;

    void Awake()
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

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }
        gridManager.GenerateGrid();

        CSVReader csvReader = new CSVReader();
        trials = csvReader.ReadTrialCSV(csvFileName);

        if (trials.Count > 0)
        {
            StartCoroutine(RunTrials());
        }
        else
        {
            Debug.LogError("No trials found in the CSV file.");
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

        Chaser chaser = FindObjectOfType<Chaser>();
        if (chaser != null)
        {
            chaser.chaserRender = trial.predRender;
            chaser.chase = trial.predChase;
        }

        CageRenderer cageRenderer = FindObjectOfType<CageRenderer>();
        if (cageRenderer != null)
        {
            cageRenderer.render = trial.cageRender;
            if (trial.isGreen)
            {
                cageRenderer.SetColor(Color.green);
            }
            else
            {
                cageRenderer.SetColor(Color.white);
            }
        }

        if (trial.renderMiddleScreen)
        {
            if (trial.middleScreenType == 5) // Fixation point
            {
                ShowMiddleScreen("+");
                yield return new WaitForSeconds(10.0f);
                if (!IsNextTrialMiddleScreen())
                {
                    HideMiddleScreen();
                }
            }
            else if (trial.middleScreenType == 6) // Question
            {
                ShowMiddleScreen("This is a test question");
                yield return StartCoroutine(WaitForAnyKey());
                if (!IsNextTrialMiddleScreen())
                {
                    HideMiddleScreen();
                }
            }
        }
        else
        {
            Debug.Log("Trial started with parameters: " + trial.ToString());

            if (trial.predChase)
            {
                while (chaser.chase)
                {
                    yield return null;
                }
            }
            else // Normal trials
            {
                // Trial duration for normal trials
                float trialDuration = 10.0f;
                yield return new WaitForSeconds(trialDuration);
            }

            if (cageRenderer != null)
            {
                cageRenderer.SetColor(Color.white);
            }
        }

        isTrialRunning = false;

        Debug.Log("Trial ended.");

        yield return new WaitForSeconds(2.0f);
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

    private bool IsNextTrialMiddleScreen()
    {
        if (currentTrialIndex + 1 < trials.Count)
        {
            Trial nextTrial = trials[currentTrialIndex + 1];
            return nextTrial.renderMiddleScreen;
        }
        return false;
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
