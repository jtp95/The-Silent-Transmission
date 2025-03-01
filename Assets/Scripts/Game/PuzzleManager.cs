using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;

public class PuzzleManager : MonoBehaviour
{
    public TextMeshProUGUI question;
    public TextMeshProUGUI answer;
    public string puzzleWord;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string Encode(string word)
    {
        string encoded = "";
        int chapter = GlobalManager.Instance.chapter;
        Dictionary<string, List<string>> letterDict = GlobalManager.Instance.letterDict;

        foreach (char c in word)
        {
            // encoded += letterDict[c][chapter];
        }

        Debug.Log(word + " encoded to " + encoded);

        return encoded;
    }

    public void Check()
    {
        // check answer
    }

    public void Proceed()
    {
        if (puzzleWord is not null)
        {
            UpdateDict();
        }

        GameManager.Instance.stage += 1;
        int stage = GameManager.Instance.stage;

        puzzleWord = ""; //newword
    }

    public void UpdateDict()
    {
        Dictionary<string, List<string>> letterDict = GlobalManager.Instance.letterDict;
        int chapter = GlobalManager.Instance.chapter;
        string userDictVal = GlobalManager.Instance.userDict[chapter];

        for (int i = 0; i < 26; i++)
        {
            List<string> latin = new List<string>(letterDict.Keys);
            if (puzzleWord.Contains(latin[i]))
            {
                StringBuilder sb = new StringBuilder(userDictVal);
                sb[i] = char.Parse(letterDict[latin[i]][chapter]);
                userDictVal = sb.ToString();
            }
        }

        GlobalManager.Instance.userDict[chapter] = userDictVal;
        GameManager.Instance.setUserDict();
    }

    public void GetSign()
    {
        // call sign detection and get character
    }
}
