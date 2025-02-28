using UnityEngine;
using TMPro;

public class Transceiver : MonoBehaviour
{
    public TextMeshProUGUI messageDisplay; // UI Text for message
    private bool isMessageVisible = false;

    void Start()
    {
        ShowEncryptedMessage();
    }

    public void ShowEncryptedMessage()
    {
        string encryptedText = GenerateEncryptedMessage();
        messageDisplay.text = encryptedText;
        isMessageVisible = true;
        messageDisplay.gameObject.SetActive(true);
    }

    public void HideMessage()
    {
        isMessageVisible = false;
        messageDisplay.gameObject.SetActive(false);
    }

    private string GenerateEncryptedMessage()
    {
        string message = "";
        for (int i = 0; i < 5; i++) // Generate a 5-letter message
        {
            // message += LetterLoader.Instance.GetRandomLetter("Greek") + " ";
        }
        return message;
    }
}