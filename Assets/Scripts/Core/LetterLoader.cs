using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class LetterData
{
    public List<string> Latin;
    public List<string> Greek;
    public List<string> Glagolitic;
    public List<string> Runic;
    public List<string> Phoenician;
}

public static class LetterLoader // No need for MonoBehaviour
{
    public static LetterData LoadLetters()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("letter_data");
        
        if (jsonFile != null)
        {
            LetterData letterData = JsonUtility.FromJson<LetterData>(jsonFile.text);
            Debug.Log("Letters Loaded Successfully!");
            return letterData;
        }
        else
        {
            Debug.LogError("Letter JSON file not found! Returning empty letter data.");
            return new LetterData();
        }
    }
}