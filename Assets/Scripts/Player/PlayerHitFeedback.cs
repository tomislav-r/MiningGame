using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("Player Sprite")]
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Screen Damage Overlay")]
    [SerializeField] private Image damageOverlay;

    [Header("Settings")]
    [SerializeField] private Color spriteHitColor = Color.red;

    [SerializeField] private float spriteFlashDuration = 0.12f;

    [SerializeField, Range(0f, 1f)]
    private float overlayMaxAlpha = 0.12f;

    [SerializeField] private float overlayFadeDuration = 0.35f;

    private Color originalSpriteColor;
    private Coroutine hitCoroutine;


    private void Awake()
    {
        if (playerSprite != null)
        {
            originalSpriteColor = playerSprite.color;
        }

        ClearHitEffect();
    }


    public void PlayHitEffect()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        hitCoroutine = StartCoroutine(HitEffect());
    }


    private IEnumerator HitEffect()
    {
        if (playerSprite != null)
        {
            playerSprite.color = spriteHitColor;
        }

        if (damageOverlay != null)
        {
            Color overlayColor = damageOverlay.color;
            overlayColor.a = overlayMaxAlpha;
            damageOverlay.color = overlayColor;
        }

        yield return new WaitForSecondsRealtime(spriteFlashDuration);

        if (playerSprite != null)
        {
            playerSprite.color = originalSpriteColor;
        }

        float timer = 0f;

        while (timer < overlayFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = timer / overlayFadeDuration;

            if (damageOverlay != null)
            {
                Color overlayColor = damageOverlay.color;

                overlayColor.a = Mathf.Lerp(
                    overlayMaxAlpha,
                    0f,
                    progress
                );

                damageOverlay.color = overlayColor;
            }

            yield return null;
        }

        ClearHitEffect();

        hitCoroutine = null;
    }


    public void ClearHitEffect()
    {
        if (playerSprite != null)
        {
            playerSprite.color = originalSpriteColor;
        }

        if (damageOverlay != null)
        {
            Color overlayColor = damageOverlay.color;
            overlayColor.a = 0f;
            damageOverlay.color = overlayColor;
        }
    }
}