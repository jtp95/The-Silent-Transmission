using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

    // // Open the settings menu
    // public GameObject settingsPanel; // Assign this in the Inspector

    // public void OpenSettings()
    // {
    //     if (settingsPanel != null)
    //     {
    //         settingsPanel.SetActive(true);
    //     }
    // }

    // public void CloseSettings()
    // {
    //     if (settingsPanel != null)
    //     {
    //         settingsPanel.SetActive(false);
    //     }
    // }
}