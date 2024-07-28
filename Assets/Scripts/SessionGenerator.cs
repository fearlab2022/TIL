using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;


public class SessionGenerator : MonoBehaviour
{
    public string csvFileName = "conditions";
    private string persistentDataPath;

    void Start()
    {
        CSVReader csvReader = new CSVReader();
        List<Trial> trials = csvReader.ReadTrialCSV(csvFileName);

        
        persistentDataPath = "https://sprint2-95476-default-rtdb.firebaseio.com/" + 01;

        ExperimentDescription exp = new ExperimentDescription();
        exp.condition = "this";
        exp.participantID = "is";
        exp.experimentDate = System.DateTime.Now.ToString();
        exp.datapath = persistentDataPath;

        RestClient.Put(persistentDataPath + "/" + 01 + "_experiment.json", exp);
        Debug.Log("Data Pushed");

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

    public void PushDataToDatabase(int trialNumber, Dictionary<float, Vector2> playerInputs)
    {
        persistentDataPath = "https://sprint2-95476-default-rtdb.firebaseio.com/" + trialNumber;
        trialData data = new trialData();   
        data.trialNumber = trialNumber;  
        data.vectorInputs = playerInputs; 
        RestClient.Put(persistentDataPath + "/" + trialNumber + "_experiment.json", data);
        Debug.Log($"Data for trial {trialNumber} pushed to database.");
    }

    public class trialData {

        public int trialNumber;
        public Dictionary<float, Vector2> vectorInputs;
        
    }

    public class ExperimentDescription
        {
            public string condition;
            public string participantID;
            public string experimentDate;
            public string datapath;
        }



}
