using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;

    [Header("Resource Text")]
    [SerializeField] private TMP_Text coalText;
    [SerializeField] private TMP_Text copperText;
    [SerializeField] private TMP_Text ironText;
    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += UpdateUI;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (inventory == null)
        {
            return;
        }

        coalText.text =
            "Coal: " + inventory.GetCoal();

        copperText.text =
            "Copper: " + inventory.GetCopper();

        ironText.text =
            "Iron: " + inventory.GetIron();

        goldText.text =
            "Gold: " + inventory.GetGold();
    }
}