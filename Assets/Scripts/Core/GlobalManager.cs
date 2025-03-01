using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Data;
public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance; // Singleton instance
    public LetterData letterData;

    public int chapter = 0;
    public int section = 0;
    public Dictionary<string, List<string>> letterDict = new Dictionary<string, List<string>>();
    public List<string> userDict;

    private void Awake()
    {
        // Make sure this object is not destroyed between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object when switching scenes
            InitDictionary();
            InitUserDict();
        }
        else
        {
            Destroy(gameObject); // Prevents duplicates
        }
    }

    public void InitDictionary()
    {
        letterData = LetterLoader.LoadLetters();

        for (int i = 0; i < 26; i++)
        {
            List<string> letters = new()
            {
                letterData.Greek[i],
                letterData.Glagolitic[i],
                letterData.Runic[i],
                letterData.Phoenician[i]
            };
            letterDict.Add(letterData.Latin[i], letters);
        }
    }

    public void SetChapter(int ch)
    {
        chapter = ch;
        Debug.Log("Changed chapter to: " + chapter);
    }

    public void InitUserDict()
    {
        userDict = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            userDict.Add("--------------------------");
        }
    }
}