using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.UI;

public class SessionGenerator : MonoBehaviour
{


    public string ppid;
    public string conditionFile;

    public GameObject task;
    public List<TILTrial> trials;
    public int numTrials;
    public TILTrial trial;
    public ExperimentDescription exp;
    public string persistentDataPath;




    void Start()
    {
        ppid = PlayerPrefs.GetString("participant_ID");
        conditionFile = PlayerPrefs.GetString("experimenter_condition");
        persistentDataPath = Application.streamingAssetsPath+"/"+ppid+"/"+conditionFile;
        
        
        // Check if the directory exists
        if (Directory.Exists(persistentDataPath)) 
        {
            // Directory already exists, add an integer to the directory name
            int counter = 1;
            string newDirectoryPath;
    
            do {
                newDirectoryPath = persistentDataPath + "_" + counter;
                counter++;
                } 
            while (Directory.Exists(newDirectoryPath));

            persistentDataPath = newDirectoryPath;
        }


        Directory.CreateDirectory(persistentDataPath);
        generateLog();
        Cursor.visible = false;
    }



      
    public void writeToLog(string s)
    {
        string time =""+System.DateTime.Now;
        string gameTime =""+Time.realtimeSinceStartup;
        string myFilePath = persistentDataPath+"/system_log_"+ppid+".txt";
        StreamWriter filewriter = new StreamWriter(myFilePath, true);
        filewriter.Write(time+"\tGAMECLOCK:"+gameTime+"\t EVENT:"+s+"\n");
        filewriter.Close();
    }

    public void generateLog()
    {
        File.WriteAllText(persistentDataPath+"/system_log_"+ppid+".txt", "=== TIL FMRI ===" + "\n");
    }

    public void pushTrialData(TILTrial trialRecieved)
    {
        int trial_num = PlayerPrefs.GetInt("trial_num");
        trial = trialRecieved;
        pushToJson(trial_num, "trial_"+trial_num, trial);
        writeToLog("Trial"+trial_num+" recorded as json");
    }

    private void pushToJson(int trial_num, string attributeName, TILTrial attribute)
    {
        string jsonData = JsonUtility.ToJson(attribute); // Convert the data object to JSON
        File.WriteAllText(persistentDataPath+"/"+attributeName+".json", jsonData);
    }

    private void pushToJson(string ppid, string attributeName, ExperimentDescription attribute)
    {
        string jsonData = JsonUtility.ToJson(attribute); // Convert the data object to JSON
        File.WriteAllText(persistentDataPath+"/"+attributeName+".json", jsonData);
    }

    private void generateCondition(string s)
    {
        string condition =s;
        Debug.Log("setting condition to"+condition);
        PlayerPrefs.SetString("conditionFile", condition);
       
    }




}
