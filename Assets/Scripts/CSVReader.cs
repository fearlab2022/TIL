using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class CSVReader
{
    public List<TIL_Trial> ReadTrialCSV(string filePath)
    {
        List<TIL_Trial> trials = new List<TIL_Trial>();

        TextAsset csvFile = Resources.Load<TextAsset>(filePath);
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found at path: " + filePath);
            return trials;
        }

        string[] lines = csvFile.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string[] data = line.Split(new string[] { "," }, StringSplitOptions.None);

            if (data.Length < 8)
            {
                Debug.LogError("Invalid data format: " + line);
                continue;
            }

            bool predRender = bool.Parse(data[0].Trim());
            bool cageRender = bool.Parse(data[1].Trim());
            bool predChase = bool.Parse(data[2].Trim());
            bool isGreen = bool.Parse(data[3].Trim());
            bool EOB = bool.Parse(data[4].Trim());
            bool showQuestionScreen = bool.Parse(data[5].Trim());
            float startX = float.Parse(data[6].Trim());
            float startY = float.Parse(data[7].Trim());

            string questionText = null;
            if (showQuestionScreen && data.Length > 8)
            {
                questionText = data[8].Trim();
            }

            TIL_Trial trial = new TIL_Trial(predRender, cageRender, predChase, isGreen, EOB, showQuestionScreen, questionText, startX, startY);
            trials.Add(trial);
        }

        return trials;
    }
}
