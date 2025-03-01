using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI userDictionary;
    public int stage = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Started");
        setUserDict();
    }

    // Update is called once per frame
    void Update()
    {
        // if current stage != current stage trigger puzzle update
    }

    public void setUserDict(){
        LetterData letterData = GlobalManager.Instance.letterData;
        string userDictVal = GlobalManager.Instance.userDict[GlobalManager.Instance.chapter];

        string userDictText = "";
        for (int i = 0; i < 26; i++)
        {
            // alphabet, colon, space, alphabet, 3 spaces (7 characters)
            userDictText += letterData.Latin[i] + ": " + userDictVal[i] + "   "; 
            if (i % 3 == 2)
            {
                userDictText += "\n";
            }
        }

        userDictionary.text = userDictText;
    }
}
