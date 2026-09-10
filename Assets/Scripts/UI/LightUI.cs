using TMPro;
using UnityEngine;

public class LightUI : MonoBehaviour
{
    [SerializeField] private LightPlacer lightPlacer;
    [SerializeField] private TMP_Text lightsText;


    private void OnEnable()
    {
        if (lightPlacer != null)
        {
            lightPlacer.OnLightsChanged += UpdateUI;
        }

        UpdateUI();
    }


    private void OnDisable()
    {
        if (lightPlacer != null)
        {
            lightPlacer.OnLightsChanged -= UpdateUI;
        }
    }


    private void UpdateUI()
    {
        if (lightPlacer == null || lightsText == null)
        {
            return;
        }

        lightsText.text =
            $"Lights: {lightPlacer.GetRemainingLights()}";
    }
}