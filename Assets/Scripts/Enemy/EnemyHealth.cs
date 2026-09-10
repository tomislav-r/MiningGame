using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;

    [Header("Hit Feedback")]
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private float hitFlashDuration = 0.08f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackDuration = 0.15f;

    private EnemyAI enemyAI;
    private Coroutine knockbackCoroutine;
    private Rigidbody2D rb;

    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }


    private void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Max(
            currentHealth,
            0
        );

        Debug.Log(
            $"{gameObject.name} HP: " +
            $"{currentHealth}/{maxHealth}"
        );

        FlashWhenHit();
        ApplyKnockback(attackerPosition);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Vector2 attackerPosition)
    {
        if (rb == null)
        {
            return;
        }

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        knockbackCoroutine =
            StartCoroutine(
                KnockbackRoutine(attackerPosition)
            );
    }


    private void FlashWhenHit()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine =
            StartCoroutine(HitFlash());
    }


    private IEnumerator HitFlash()
    {
        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(
            hitFlashDuration
        );

        spriteRenderer.color = originalColor;

        flashCoroutine = null;
    }


    private void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator KnockbackRoutine(
    Vector2 attackerPosition)
    {
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        Vector2 direction =
            ((Vector2)transform.position -
            attackerPosition).normalized;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            direction * knockbackForce,
            ForceMode2D.Impulse
        );

        yield return new WaitForSeconds(
            knockbackDuration
        );

        rb.linearVelocity = Vector2.zero;

        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }

        knockbackCoroutine = null;
    }
}