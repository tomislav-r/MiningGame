using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    [Header("Pickaxe")]
    [SerializeField] private int pickaxeLevel = 1;
    [SerializeField] private int miningPower = 1;

    [Header("Upgrade Settings")]
    [SerializeField] private int maxPickaxeLevel = 3;

    public int GetPickaxeLevel()
    {
        return pickaxeLevel;
    }

    public int GetMiningPower()
    {
        return miningPower;
    }

    public void UpgradePickaxe()
    {
        if (pickaxeLevel >= maxPickaxeLevel)
        {
            Debug.Log("Pickaxe je već maksimalnog levela.");
            return;
        }

        pickaxeLevel++;

        UpdateMiningPower();

        Debug.Log(
            $"Pickaxe upgraded! Level: {pickaxeLevel} | " +
            $"Mining Power: {miningPower}"
        );
    }

    private void UpdateMiningPower()
    {
        switch (pickaxeLevel)
        {
            case 1:
                miningPower = 1;
                break;

            case 2:
                miningPower = 2;
                break;

            case 3:
                miningPower = 3;
                break;
        }
    }

    public void SetPickaxeStats(int level, int power)
    {
        pickaxeLevel = level;
        miningPower = power;
    }
}