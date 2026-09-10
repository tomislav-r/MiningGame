using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused;


    private void Start()
    {
        pausePanel.SetActive(false);
    }


    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPausePressed;
    }


    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
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

        // Ako je igra već zaustavljena zbog
        // End of Year / Victory / Game Over,
        // ne otvaramo Pause Menu.
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
}