using System;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader
{
    public List<Trial> ReadTrialCSV(string filePath)
    {
        List<Trial> trials = new List<Trial>();

        // Load the CSV file from the Resources folder
        TextAsset csvFile = Resources.Load<TextAsset>(filePath);
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found at path: " + filePath);
            return trials;
        }

        string[] data = csvFile.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        for (int i = 0; i < data.Length; i++)
        {
            if (int.TryParse(data[i], out int condition))
            {
                int middleScreenType = (data.Length > i + 1) && int.TryParse(data[i + 1], out int ms) ? ms : 0;
                Trial trial = ConditionToTrial(condition, middleScreenType);
                if (trial != null)
                {
                    trials.Add(trial);
                }
                i++; // Skip next element as it's already processed
            }
        }

        return trials;
    }

    private Trial ConditionToTrial(int condition, int middleScreenType)
    {
        switch (condition)
        {
            case 1:
                return new Trial(false, true, false, false, false, 0);
            case 2:
                return new Trial(true, true, false, false, false, 0);
            case 3:
                return new Trial(true, false, true, false, false, 0);
            case 4:
                return new Trial(true, true, false, true, false, 0);
            case 5:
                return new Trial(false, false, false, false, true, 5);
            case 6:
                return new Trial(false, false, false, false, true, 6);
            default:
                Debug.LogError("Unknown condition: " + condition);
                return null;
        }
    }
}
