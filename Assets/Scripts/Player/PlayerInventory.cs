using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private int coal;
    private int copper;
    private int iron;
    private int gold;
    public event Action OnInventoryChanged;

    public void AddResource(TileType type, int amount = 1)
    {
        switch (type)
        {
            case TileType.Coal:
                coal += amount;
                break;

            case TileType.Copper:
                copper += amount;
                break;

            case TileType.Iron:
                iron += amount;
                break;

            case TileType.Gold:
                gold += amount;
                break;

            default:
                return;
        }

        OnInventoryChanged?.Invoke();
    }

    public void ClearInventory()
    {
        coal = 0;
        copper = 0;
        iron = 0;
        gold = 0;

        OnInventoryChanged?.Invoke();

        Debug.Log("Dnevni inventory je izgubljen.");
    }

    public int GetCoal()
    {
        return coal;
    }

    public int GetCopper()
    {
        return copper;
    }

    public int GetIron()
    {
        return iron;
    }

    public int GetGold()
    {
        return gold;
    }
}