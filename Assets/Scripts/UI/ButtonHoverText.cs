using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverText : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private TMP_Text buttonText;

    [Header("Hover")]
    [SerializeField] private float hoverSizeIncrease = 2f;

    private float originalFontSize;


    private void Awake()
    {
        if (buttonText == null)
        {
            buttonText =
                GetComponentInChildren<TMP_Text>();
        }

        if (buttonText != null)
        {
            originalFontSize =
                buttonText.fontSize;
        }
    }


    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (buttonText == null)
        {
            return;
        }

        buttonText.fontSize =
            originalFontSize +
            hoverSizeIncrease;
    }


    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (buttonText == null)
        {
            return;
        }

        buttonText.fontSize =
            originalFontSize;
    }


    private void OnDisable()
    {
        if (buttonText != null)
        {
            buttonText.fontSize =
                originalFontSize;
        }
    }
}