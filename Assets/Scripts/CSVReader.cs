using System;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader
{
    public List<Trial> ReadTrialCSV(string filePath)
    {
        List<Trial> trials = new List<Trial>();

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
                Trial trial = ConditionToTrial(condition);
                if (trial != null)
                {
                    trials.Add(trial);
                }
            }
        }

        return trials;
    }

    private Trial ConditionToTrial(int condition)
    {
        switch (condition)
        {
            case 1:
                return new Trial(false, true, false);
            case 2:
                return new Trial(true, true, false);
            case 3:
                return new Trial(true, false, true);
            default:
                Debug.LogError("Unknown condition: " + condition);
                return null;
        }
    }
}
