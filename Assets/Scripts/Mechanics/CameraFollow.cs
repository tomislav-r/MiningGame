using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Camera Shake")]
    [SerializeField] private float shakeStrength = 0.04f;

    private bool isShaking;


    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }


        Vector3 targetPosition =
            new Vector3(
                player.position.x,
                player.position.y,
                transform.position.z
            );


        bool screenShakeEnabled =
            PlayerPrefs.GetInt(
                "ScreenShake",
                1
            ) == 1;


        if (
            isShaking &&
            screenShakeEnabled
        )
        {
            Vector2 offset =
                Random.insideUnitCircle *
                shakeStrength;

            targetPosition.x +=
                offset.x;

            targetPosition.y +=
                offset.y;
        }


        transform.position =
            targetPosition;
    }


    public void SetMiningShake(
        bool shaking)
    {
        isShaking =
            shaking;
    }
}