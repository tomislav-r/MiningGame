using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightController : MonoBehaviour
{
    [SerializeField] private MiningController miningController;
    [SerializeField] private Light2D playerLight;

    private void Start()
    {
        UpdateLightRange();
    }

    public void UpdateLightRange()
    {
        if (miningController == null || playerLight == null)
        {
            return;
        }

        playerLight.pointLightOuterRadius =
            miningController.GetMiningRange();
    }
}