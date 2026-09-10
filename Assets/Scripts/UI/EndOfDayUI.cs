using TMPro;
using UnityEngine;

public class EndOfDayUI : MonoBehaviour
{
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private DebtManager debtManager;

    [Header("Debt")]
    [SerializeField] private TMP_Text debtText;
    [SerializeField] private TMP_InputField debtInputField;
    [SerializeField] private TMP_Text debtMessageText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private TMP_Text earningsText;
    [SerializeField] private TMP_Text moneyText;

    [SerializeField] private FamilyManager familyManager;

    [Header("Family")]
    [SerializeField] private TMP_Text familyText;
    [SerializeField] private TMP_Text foodCostText;
    [SerializeField] private TMP_Text foodMessageText;

    [Header("Panels")]
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private GameObject upgradePanel;


    public void ShowSuccessfulDay(PlayerInventory inventory, EconomyManager economy, int earnings)
    {
        summaryPanel.SetActive(true);
        upgradePanel.SetActive(false);
        gameObject.SetActive(true);

        titleText.text =
            "DAY COMPLETE";

        resourcesText.text =
            $"Coal: {inventory.GetCoal()} " +
            $"(${inventory.GetCoal() * economy.GetResourceValue(TileType.Coal)})\n" +

            $"Copper: {inventory.GetCopper()} " +
            $"(${inventory.GetCopper() * economy.GetResourceValue(TileType.Copper)})\n" +

            $"Iron: {inventory.GetIron()} " +
            $"(${inventory.GetIron() * economy.GetResourceValue(TileType.Iron)})\n" +

            $"Gold: {inventory.GetGold()} " +
            $"(${inventory.GetGold() * economy.GetResourceValue(TileType.Gold)})";

        earningsText.text =
            $"Earned: ${earnings}";

        moneyText.text =
            $"Money: ${economy.GetMoney()}";

        debtMessageText.text = "";

        UpdateEconomyUI();

        foodMessageText.text = "";

        UpdateFamilyUI();
    }


    public void ShowFailedDay()
    {
        summaryPanel.SetActive(true);
        upgradePanel.SetActive(false);
        gameObject.SetActive(true);

        titleText.text =
            "FAILED TO RETURN";

        resourcesText.text =
            "All resources collected today were lost.";

        earningsText.text =
            "Earned: $0";

        moneyText.text =
            "";

        debtMessageText.text = "";

        UpdateEconomyUI();

        foodMessageText.text = "";

        UpdateFamilyUI();
    }

    public void PayDebt()
    {
        if (!int.TryParse(
                debtInputField.text,
                out int requestedAmount))
        {
            debtMessageText.text =
                "Enter a valid amount.";

            return;
        }

        int paidAmount =
            debtManager.PayDebt(
                requestedAmount,
                economyManager
            );

        if (paidAmount <= 0)
        {
            debtMessageText.text =
                "Not enough money.";

            return;
        }

        debtMessageText.text =
            $"Paid: ${paidAmount}";

        debtInputField.text = "";

        UpdateEconomyUI();
    }

    private void UpdateEconomyUI()
    {
        moneyText.text =
            $"Money: ${economyManager.GetMoney()}";

        debtText.text =
            $"Debt remaining: ${debtManager.GetCurrentDebt()}";
    }

    public void BuyFood()
    {
        if (familyManager.WasFoodBought())
        {
            foodMessageText.text =
                "Food already purchased.";

            return;
        }

        bool success =
            familyManager.BuyFood(
                economyManager
            );

        if (!success)
        {
            foodMessageText.text =
                "Not enough money.";

            return;
        }

        foodMessageText.text =
            "Food purchased.";

        UpdateEconomyUI();
        UpdateFamilyUI();
    }

    private void UpdateFamilyUI()
    {
        familyText.text =
            $"Family Health: " +
            $"{familyManager.GetFamilyHealth()}/" +
            $"{familyManager.GetMaxFamilyHealth()}";

        if (familyManager.WasFoodBought())
        {
            foodCostText.text =
                "Food: PAID";
        }
        else
        {
            foodCostText.text =
                $"Food cost: ${familyManager.GetFoodCost()}";
        }
    }

    public void OpenUpgradePanel()
    {
        summaryPanel.SetActive(false);
        upgradePanel.SetActive(true);
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
        summaryPanel.SetActive(true);
    }
}