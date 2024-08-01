using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;

public class SessionGenerator : MonoBehaviour
{
    public string csvFileName = "conditions";
    public int participantID = 01;

    // Assign this in the Unity Editor
    public TaskManager taskManager;

    private string persistentDataPath;

    void Start()
    {
        // Initialize the CSV reader and read trials
        CSVReader csvReader = new CSVReader();
        List<Trial> trials = csvReader.ReadTrialCSV(csvFileName);

        string experimentKey = participantID + "_experiment";
        persistentDataPath = "https://sprint2-95476-default-rtdb.firebaseio.com/" + experimentKey;

        ExperimentDescription exp = new ExperimentDescription
        {
            sessionAdministrator = "Varun Reddy",
            participantID = participantID,
            experimentDate = System.DateTime.Now.ToString()
        };

        // Create the experiment entry in the database
        RestClient.Put(persistentDataPath + "/details.json", exp);

        Debug.Log("Experiment Data Pushed");

        // Ensure TaskManager is assigned
        if (taskManager != null)
        {
            taskManager.InitializeTrials(trials);
        }
        else
        {
            Debug.LogError("TaskManager not assigned!");
        }
    }

    public void PushDataToDatabase(Trial trial, int trialNumber, float confidenceValue, float trialTime, List<PlayerVector> positionDataList, String startTime, String endTime, List<JoystickInput> joystickInputList)
    {
        string trialKey = "trial_" + trialNumber; 
        string trialPath = persistentDataPath + "/trials/" + trialKey + ".json"; // Nest under /trials/

        trial.index = trialNumber;
        trial.playerQuestionInput = confidenceValue;
        trial.positionDataList = positionDataList;
        trial.joystickInputList = joystickInputList;
        trial.startTime = startTime;
        trial.endTime = endTime;

        // Push trial data under the experiment's trials path
        RestClient.Put(trialPath, trial);
        Debug.Log($"Data for trial {trialNumber} pushed to database.");
    }

    public class ExperimentDescription
    {
        public string sessionAdministrator;
        public int participantID;
        public string experimentDate;
    }
}
