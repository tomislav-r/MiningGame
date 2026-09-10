using TMPro;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    [Header("Day Settings")]
    [SerializeField] private float dayDuration = 120f;

    [SerializeField] private int currentDay = 1;
    [SerializeField] private int maxDays = 10;
    [SerializeField] private LightPlacer lightPlacer;


    [Header("References")]
    [SerializeField] private ExtractionZone extractionZone;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private EndOfDayUI endOfDayUI;
    [SerializeField] private MineGenerator mineGenerator;
    [SerializeField] private FamilyManager familyManager;
    [SerializeField] private DebtManager debtManager;
    [SerializeField] private GameManager gameManager;


    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text dayText;


    private float timeRemaining;
    private bool dayRunning;


    private void Start()
    {
        endOfDayUI.gameObject.SetActive(false);

        UpdateDayUI();

        StartDay();
    }


    private void Update()
    {
        if (!dayRunning)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;

            EndDay();
        }

        UpdateTimerUI();
    }


    private void StartDay()
    {
        timeRemaining = dayDuration;
        dayRunning = true;

        UpdateTimerUI();
        UpdateDayUI();

        Debug.Log(
            $"{currentDay}/{maxDays}. godina je započela."
        );
    }


    private void EndDay()
    {
        dayRunning = false;

        if (extractionZone.IsPlayerInside())
        {
            int earnings =
                economyManager.BankDailyEarnings(
                    playerInventory
                );

            Debug.Log(
                $"{currentDay}. godina uspješno završena. " +
                $"Zarađeno: ${earnings}"
            );

            endOfDayUI.ShowSuccessfulDay(
                playerInventory,
                economyManager,
                earnings
            );
        }
        else
        {
            Debug.Log(
                $"{currentDay}. godina završena - " +
                "Player se nije vratio. Loot je izgubljen!"
            );

            playerInventory.ClearInventory();

            endOfDayUI.ShowFailedDay();
        }

        if (currentDay >= maxDays)
        {
            Debug.Log(
                "Završena je posljednja godina."
            );
        }

        Time.timeScale = 0f;
    }


    public void StartNextDay()
    {
        if (gameManager.IsGameFinished())
        {
            return;
        }

        // Prvo završavamo obiteljsku obvezu za ovaj dan.
        familyManager.ResolveDay();

        // Obitelj nije preživjela.
        if (!familyManager.IsFamilyAlive())
        {
            gameManager.GameOver();
            return;
        }

        // Dug je otplaćen i obitelj je živa.
        if (debtManager.IsDebtPaid())
        {
            gameManager.Victory();
            return;
        }

        // Potrošen je posljednji raspoloživi dan,
        // a dug još postoji.
        if (currentDay >= maxDays)
        {
            gameManager.GameOver();
            return;
        }

        // Igra se nastavlja.
        Time.timeScale = 1f;

        currentDay++;

        playerInventory.ClearInventory();

        lightPlacer.ResetLightsForNewDay();

        endOfDayUI.gameObject.SetActive(false);

        mineGenerator.RegenerateMap();

        StartDay();
    }


    private void UpdateTimerUI()
    {
        if (timerText == null)
        {
            return;
        }

        int totalSeconds =
            Mathf.CeilToInt(timeRemaining);

        int minutes =
            totalSeconds / 60;

        int seconds =
            totalSeconds % 60;

        timerText.text =
            $"{minutes:00}:{seconds:00}";
    }


    private void UpdateDayUI()
    {
        if (dayText == null)
        {
            return;
        }

        dayText.text = $"Year {currentDay} / {maxDays}";
    }


    public int GetCurrentDay()
    {
        return currentDay;
    }


    public int GetMaxDays()
    {
        return maxDays;
    }
}