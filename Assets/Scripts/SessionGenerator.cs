using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;


public class SessionGenerator : MonoBehaviour
{
    public string csvFileName = "conditions";
    private string persistentDataPath;

    public int participantID = 01;

    void Start()
{
    CSVReader csvReader = new CSVReader();
    List<Trial> trials = csvReader.ReadTrialCSV(csvFileName);

    string experimentKey = participantID + "_experiment";
    persistentDataPath = "https://sprint2-95476-default-rtdb.firebaseio.com/" + experimentKey;

    ExperimentDescription exp = new ExperimentDescription();
    exp.sessionAdministrator = "Varun Reddy";
    exp.participantID = participantID;
    exp.experimentDate = System.DateTime.Now.ToString();

    // Create the experiment entry in the database
    RestClient.Put(persistentDataPath + "/details.json", exp);
    Debug.Log("Experiment Data Pushed");

    TaskManager taskManager = FindObjectOfType<TaskManager>();
    if (taskManager != null)
    {
        taskManager.InitializeTrials(trials);
    }
    else
    {
        Debug.LogError("TaskManager not found!");
    }
}
  public void PushDataToDatabase(int trialNumber, Dictionary<float, Vector2> playerInputs, float confidenceValue, float trialTime)
{
    string trialKey = "trial_" + trialNumber; // Create a unique key for each trial
    string trialPath = persistentDataPath + "/trials/" + trialKey + ".json"; // Nest under /trials/

    trialData data = new trialData();
    data.trialNumber = trialNumber;
    data.vectorInputs = playerInputs;
    data.questionInput = confidenceValue;
    data.trialTime = trialTime;

    // Push trial data under the experiment's trials path
    RestClient.Put(trialPath, data);
    Debug.Log($"Data for trial {trialNumber} pushed to database.");
}

    public class trialData {

        public int trialNumber;
        public Dictionary<float, Vector2> vectorInputs;
        public float questionInput;
        public float trialTime;
    }

    public class ExperimentDescription
        {
            public string sessionAdministrator;
            public int participantID;
            public string experimentDate;
        }



}
