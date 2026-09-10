using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightPlacer : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference placeLightAction;

    [Header("Light")]
    [SerializeField] private GameObject portableLightPrefab;

    [Header("Starting Lights")]
    [SerializeField] private int startingLights = 3;

    [Header("Optional")]
    [SerializeField] private Transform lightParent;

    private int remainingLights;

    private readonly List<GameObject> placedLights =
        new List<GameObject>();

    public event Action OnLightsChanged;


    private void Start()
    {
        remainingLights = startingLights;

        OnLightsChanged?.Invoke();
    }


    private void OnEnable()
    {
        placeLightAction.action.Enable();
        placeLightAction.action.performed += OnPlaceLight;
    }


    private void OnDisable()
    {
        placeLightAction.action.performed -= OnPlaceLight;
        placeLightAction.action.Disable();
    }


    private void OnPlaceLight(
        InputAction.CallbackContext context)
    {
        PlaceLight();
    }


    private void PlaceLight()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (remainingLights <= 0)
        {
            Debug.Log("Nema više svjetala.");
            return;
        }

        if (portableLightPrefab == null)
        {
            return;
        }

        GameObject newLight =
            Instantiate(
                portableLightPrefab,
                transform.position,
                Quaternion.identity,
                lightParent
            );

        placedLights.Add(newLight);

        remainingLights--;

        OnLightsChanged?.Invoke();

        Debug.Log(
            $"Postavljeno svjetlo. Preostalo: {remainingLights}"
        );
    }


    public void AddLights(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        remainingLights += amount;

        OnLightsChanged?.Invoke();
    }


    public int GetRemainingLights()
    {
        return remainingLights;
    }


    public void ResetLightsForNewDay()
    {
        foreach (GameObject placedLight in placedLights)
        {
            if (placedLight != null)
            {
                Destroy(placedLight);
            }
        }

        placedLights.Clear();

        OnLightsChanged?.Invoke();
    }
}