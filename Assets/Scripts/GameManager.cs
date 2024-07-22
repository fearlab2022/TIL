using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public string csvFileName = "conditions"; // Name of your CSV file without the .csv extension
    private List<Trial> trials;
    private int currentTrialIndex = 0;
    private GridManager gridManager;
    private Vector3 chaserStartPosition = new Vector3(4, 4, 0); // Start position for the Chaser
    private bool isTrialRunning = false;

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
        // Initialize GridManager and generate the grid
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }
        gridManager.GenerateGrid();

        // Load trials from the CSV file
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

            // Set Chaser start position
            SetChaserStartPosition();

            // Start the trial
            yield return StartCoroutine(StartTrial(currentTrial));

            // Increment the trial index
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

        // Set up the trial based on the trial parameters
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
        }

        // Start the trial logic (e.g., enable certain game objects, start timers, etc.)
        Debug.Log("Trial started with parameters: " + trial.ToString());

        if (trial.predChase) // If chase is true, wait until the chaser catches the player
        {
            // Wait until the trial is manually ended
            while (chaser.chase)
            {
                yield return null;
            }
        }
        else // If chase is false, end the trial after 10 seconds
        {
            // Trial duration for trials without chasing
            float trialDuration = 10.0f;
            yield return new WaitForSeconds(trialDuration);
        }

        isTrialRunning = false;

        Debug.Log("Trial ended.");

        // Wait for a short duration before starting the next trial
        yield return new WaitForSeconds(0.2f);
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
