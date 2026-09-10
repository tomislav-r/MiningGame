using UnityEngine;

public class FamilyManager : MonoBehaviour
{
    [Header("Family")]
    [SerializeField] private int maxFamilyHealth = 100;
    [SerializeField] private int familyHealth = 100;

    [Header("Food")]
    [SerializeField] private int foodCost = 200;
    [SerializeField] private int healthLossWithoutFood = 25;

    private bool foodBoughtThisDay;


    public bool BuyFood(EconomyManager economy)
    {
        if (foodBoughtThisDay)
        {
            return false;
        }

        if (!economy.TrySpendMoney(foodCost))
        {
            return false;
        }

        foodBoughtThisDay = true;

        return true;
    }


    public void ResolveDay()
    {
        if (!foodBoughtThisDay)
        {
            familyHealth -= healthLossWithoutFood;

            familyHealth =
                Mathf.Max(familyHealth, 0);
        }

        foodBoughtThisDay = false;
    }


    public int GetFamilyHealth()
    {
        return familyHealth;
    }


    public int GetMaxFamilyHealth()
    {
        return maxFamilyHealth;
    }


    public int GetFoodCost()
    {
        return foodCost;
    }


    public bool WasFoodBought()
    {
        return foodBoughtThisDay;
    }


    public bool IsFamilyAlive()
    {
        return familyHealth > 0;
    }
}