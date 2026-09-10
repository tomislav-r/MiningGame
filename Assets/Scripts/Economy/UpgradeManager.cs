using System;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EconomyManager economy;
    [SerializeField] private PickaxeController pickaxe;
    [SerializeField] private Movement movement;
    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private LightPlacer lightPlacer;

    [Header("Portable Lights")]
    [SerializeField] private int lightPrice = 50;

    [Header("Mining Upgrade")]
    [SerializeField] private int miningLevel = 1;

    [SerializeField]
    private int[] miningPowers =
    {
        1,
        2,
        3,
        5,
        7
    };

    [SerializeField]
    private int[] miningCosts =
    {
        0,
        250,
        500,
        900,
        1500
    };


    [Header("Speed Upgrade")]
    [SerializeField] private int speedLevel = 1;

    [SerializeField]
    private float[] movementSpeeds =
    {
        5f,
        5.5f,
        6f,
        6.5f,
        7f
    };

    [SerializeField]
    private int[] speedCosts =
    {
        0,
        200,
        400,
        700,
        1200
    };


    [Header("Health Upgrade")]
    [SerializeField] private int healthLevel = 1;

    [SerializeField]
    private int[] maxHealthValues =
    {
        100,
        120,
        140,
        170,
        200
    };

    [SerializeField]
    private int[] healthCosts =
    {
        0,
        200,
        400,
        700,
        1200
    };


    public event Action OnUpgradeChanged;


    private void Start()
    {
        ApplyCurrentStats();
    }


    private void ApplyCurrentStats()
    {
        pickaxe.SetPickaxeStats(
            miningLevel,
            miningPowers[miningLevel - 1]
        );

        movement.SetMoveSpeed(
            movementSpeeds[speedLevel - 1]
        );

        playerHealth.SetMaxHealth(
            maxHealthValues[healthLevel - 1]
        );
    }


    public bool UpgradeMining()
    {
        if (miningLevel >= miningPowers.Length)
        {
            return false;
        }

        int cost =
            miningCosts[miningLevel];

        if (!economy.TrySpendMoney(cost))
        {
            return false;
        }

        miningLevel++;

        pickaxe.SetPickaxeStats(
            miningLevel,
            miningPowers[miningLevel - 1]
        );

        OnUpgradeChanged?.Invoke();

        return true;
    }


    public bool UpgradeSpeed()
    {
        if (speedLevel >= movementSpeeds.Length)
        {
            return false;
        }

        int cost =
            speedCosts[speedLevel];

        if (!economy.TrySpendMoney(cost))
        {
            return false;
        }

        speedLevel++;

        movement.SetMoveSpeed(
            movementSpeeds[speedLevel - 1]
        );

        OnUpgradeChanged?.Invoke();

        return true;
    }


    public bool UpgradeHealth()
    {
        if (healthLevel >= maxHealthValues.Length)
        {
            return false;
        }

        int cost =
            healthCosts[healthLevel];

        if (!economy.TrySpendMoney(cost))
        {
            return false;
        }

        healthLevel++;

        playerHealth.SetMaxHealth(
            maxHealthValues[healthLevel - 1]
        );

        OnUpgradeChanged?.Invoke();

        return true;
    }


    public int GetMiningLevel()
    {
        return miningLevel;
    }

    public int GetSpeedLevel()
    {
        return speedLevel;
    }

    public int GetHealthLevel()
    {
        return healthLevel;
    }


    public int GetMiningPower()
    {
        return miningPowers[miningLevel - 1];
    }

    public float GetMovementSpeed()
    {
        return movementSpeeds[speedLevel - 1];
    }

    public int GetMaxHealth()
    {
        return maxHealthValues[healthLevel - 1];
    }


    public int GetNextMiningCost()
    {
        if (miningLevel >= miningCosts.Length)
        {
            return -1;
        }

        return miningCosts[miningLevel];
    }

    public int GetNextSpeedCost()
    {
        if (speedLevel >= speedCosts.Length)
        {
            return -1;
        }

        return speedCosts[speedLevel];
    }

    public int GetNextHealthCost()
    {
        if (healthLevel >= healthCosts.Length)
        {
            return -1;
        }

        return healthCosts[healthLevel];
    }

    public bool BuyLight()
    {
        if (!economy.TrySpendMoney(lightPrice))
        {
            return false;
        }

        lightPlacer.AddLights(1);

        OnUpgradeChanged?.Invoke();

        return true;
    }

    public int GetLightPrice()
    {
        return lightPrice;
    }
}