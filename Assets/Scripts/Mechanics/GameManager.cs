using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject endOfTheDayPanel;

    [Header("Pause")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused;
    private bool gameFinished;


    private void Start()
    {
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        endOfTheDayPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }


    public void Victory()
    {
        if (gameFinished)
        {
            return;
        }

        gameFinished = true;
        isPaused = false;

        endOfTheDayPanel.SetActive(false);
        pausePanel.SetActive(false);

        victoryPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("VICTORY");
    }


    public void GameOver()
    {
        if (gameFinished)
        {
            return;
        }

        gameFinished = true;
        isPaused = false;

        endOfTheDayPanel.SetActive(false);
        pausePanel.SetActive(false);

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("GAME OVER");
    }


    public bool IsGameFinished()
    {
        return gameFinished;
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }


    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }


    private void OnEnable()
    {
        pauseAction.action.Enable();

        pauseAction.action.performed +=
            OnPausePressed;
    }


    private void OnDisable()
    {
        pauseAction.action.performed -=
            OnPausePressed;

        pauseAction.action.Disable();
    }


    private void OnPausePressed(
        InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
            return;
        }

        // Ako je igra završena ili je trenutno End of Year,
        // Time.timeScale je već 0.
        if (Time.timeScale == 0f)
        {
            return;
        }

        PauseGame();
    }


    public void PauseGame()
    {
        isPaused = true;

        pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }


    public void ResumeGame()
    {
        isPaused = false;

        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }


    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}