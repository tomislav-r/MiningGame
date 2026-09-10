using UnityEngine;

public class DebtManager : MonoBehaviour
{
    [Header("Debt")]
    [SerializeField] private int startingDebt = 10000;

    private int currentDebt;


    private void Start()
    {
        currentDebt = startingDebt;
    }


    public int PayDebt(
        int requestedAmount,
        EconomyManager economy)
    {
        if (requestedAmount <= 0)
        {
            return 0;
        }

        if (currentDebt <= 0)
        {
            return 0;
        }

        // Ne dopuštamo plaćanje više nego što dug iznosi.
        int actualPayment =
            Mathf.Min(
                requestedAmount,
                currentDebt
            );

        if (!economy.TrySpendMoney(actualPayment))
        {
            return 0;
        }

        currentDebt -= actualPayment;

        return actualPayment;
    }


    public int GetCurrentDebt()
    {
        return currentDebt;
    }


    public bool IsDebtPaid()
    {
        return currentDebt <= 0;
    }
}