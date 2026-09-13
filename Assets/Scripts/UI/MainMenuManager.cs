using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;


    public void PlayGame()
    {
        SceneManager.LoadScene(
            "GameScene",
            LoadSceneMode.Single
        );
    }


    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }


    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }


    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }
}