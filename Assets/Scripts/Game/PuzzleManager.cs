using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
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

    public void GetSign()
    {
        // call sign detection and get character
    }
}
