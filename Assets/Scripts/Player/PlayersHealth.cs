using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameManager gameManager;

    private int currentHealth;

    private PlayerHitFeedback hitFeedback;


    private void Awake()
    {
        hitFeedback = GetComponent<PlayerHitFeedback>();
    }


    private void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth =
            Mathf.Max(currentHealth, 0);

        Debug.Log(
            $"Player HP: {currentHealth}/{maxHealth}"
        );

        if (hitFeedback != null)
        {
            hitFeedback.PlayHitEffect();
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player je umro.");

            if (hitFeedback != null)
            {
                hitFeedback.ClearHitEffect();
            }

            gameManager.GameOver();
        }
    }


    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;

        currentHealth = maxHealth;
    }


    public int GetCurrentHealth()
    {
        return currentHealth;
    }


    public int GetMaxHealth()
    {
        return maxHealth;
    }


    public void HealFull()
    {
        currentHealth = maxHealth;
    }
}