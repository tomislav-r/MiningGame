using TMPro;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private EconomyManager economyManager;


    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    [SerializeField] private TMP_Text miningText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private LightPlacer lightPlacer;
    [SerializeField] private TMP_Text lightText;


    private void OnEnable()
    {
        UpdateUI();
    }


    public void BuyMiningUpgrade()
    {
        bool success =
            upgradeManager.UpgradeMining();

        messageText.text =
            success
            ? "Mining upgraded."
            : GetFailureMessage(
                upgradeManager.GetNextMiningCost()
            );

        UpdateUI();
    }


    public void BuySpeedUpgrade()
    {
        bool success =
            upgradeManager.UpgradeSpeed();

        messageText.text =
            success
            ? "Movement speed upgraded."
            : GetFailureMessage(
                upgradeManager.GetNextSpeedCost()
            );

        UpdateUI();
    }


    public void BuyHealthUpgrade()
    {
        bool success =
            upgradeManager.UpgradeHealth();

        messageText.text =
            success
            ? "Max health upgraded."
            : GetFailureMessage(
                upgradeManager.GetNextHealthCost()
            );

        UpdateUI();
    }


    private string GetFailureMessage(int nextCost)
    {
        if (nextCost == -1)
        {
            return "Maximum level reached.";
        }

        return "Not enough money.";
    }


    private void UpdateUI()
    {
        if (
            upgradeManager == null ||
            economyManager == null
        )
        {
            return;
        }

        moneyText.text =
            $"Available money: ${economyManager.GetMoney()}";


        int miningCost =
            upgradeManager.GetNextMiningCost();

        miningText.text =
            $"Mining - Level {upgradeManager.GetMiningLevel()}\n" +
            $"Power: {upgradeManager.GetMiningPower()}\n" +
            GetCostText(miningCost);


        int speedCost =
            upgradeManager.GetNextSpeedCost();

        speedText.text =
            $"Movement - Level {upgradeManager.GetSpeedLevel()}\n" +
            $"Speed: {upgradeManager.GetMovementSpeed():0.0}\n" +
            GetCostText(speedCost);


        int healthCost =
            upgradeManager.GetNextHealthCost();

        healthText.text =
            $"Health - Level {upgradeManager.GetHealthLevel()}\n" +
            $"Max HP: {upgradeManager.GetMaxHealth()}\n" +
            GetCostText(healthCost);

        lightText.text =
            $"Portable Light\n" +
            $"Owned: {lightPlacer.GetRemainingLights()}\n" +
            $"Price: ${upgradeManager.GetLightPrice()}";
    }


    private string GetCostText(int cost)
    {
        if (cost == -1)
        {
            return "MAX LEVEL";
        }

        return $"Upgrade cost: ${cost}";
    }

    public void BuyLight()
    {
        bool success = upgradeManager.BuyLight();

        if (success)
        {
            messageText.text = "Portable light purchased.";
        }
        else
        {
            messageText.text = "Not enough money.";
        }

        UpdateUI();
    }
}