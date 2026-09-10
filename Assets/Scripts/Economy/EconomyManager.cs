using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [Header("Ore Values")]
    [SerializeField] private int coalValue = 5;
    [SerializeField] private int copperValue = 10;
    [SerializeField] private int ironValue = 20;
    [SerializeField] private int goldValue = 50;

    [Header("Player Money")]
    [SerializeField] private int money = 0;


    public int BankDailyEarnings(PlayerInventory inventory)
    {
        int earnings =
            inventory.GetCoal() * coalValue +
            inventory.GetCopper() * copperValue +
            inventory.GetIron() * ironValue +
            inventory.GetGold() * goldValue;

        money += earnings;

        return earnings;
    }


    public int GetMoney()
    {
        return money;
    }


    public int GetResourceValue(TileType type)
    {
        switch (type)
        {
            case TileType.Coal:
                return coalValue;

            case TileType.Copper:
                return copperValue;

            case TileType.Iron:
                return ironValue;

            case TileType.Gold:
                return goldValue;

            default:
                return 0;
        }
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (money < amount)
        {
            return false;
        }

        money -= amount;

        return true;
    }
}