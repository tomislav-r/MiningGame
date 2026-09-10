using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text healthText;

    private void Update()
    {
        healthText.text =
            $"HP: {playerHealth.GetCurrentHealth()} / " +
            $"{playerHealth.GetMaxHealth()}";
    }

}