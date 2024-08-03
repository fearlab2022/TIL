using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;

public class SessionGenerator : MonoBehaviour
{
    public string csvFileName = "conditions";
    public int participantID = 01;

    public TaskManager taskManager;
    public GameObject task;

    private string persistentDataPath;

    void Start()
    {
        // Initialize the CSV reader and read trials

                CSVReader csvReader = new CSVReader();
                List<TIL_Trial> trials = csvReader.ReadTrialCSV(csvFileName);

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
                task.SetActive(true);
                taskManager.InitializeTrials(trials);
            


    }

    public void PushDataToDatabase(TIL_Trial trial, int trialNumber, float confidenceValue, List<PlayerVector> positionDataList, List<TimingDescription> timings, List<JoystickInput> joystickInputList, float playerInLavaTime, List<PlayerVector> chaserPositionData)
    {
        string trialKey = "trial_" + trialNumber; 
        string trialPath = persistentDataPath + "/trials/" + trialKey + ".json"; // Nest under /trials/

        trial.index = trialNumber;
        trial.playerQuestionInput = confidenceValue;
        trial.positionDataList = positionDataList;
        trial.chaserPositionList = chaserPositionData;
        trial.joystickInputList = joystickInputList;

        trial.timings = timings;
        trial.playerInLavaTime = playerInLavaTime;

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
