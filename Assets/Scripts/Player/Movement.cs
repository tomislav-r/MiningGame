using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f; // degrees per second
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private PlayerAnimationController animationController;

    [Header("Audio")]
    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private float walkingVolume = 0.8f;
    [SerializeField] private float fadeInDuration = 0.1f;
    [SerializeField] private float fadeOutDuration = 0.15f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool walkingSoundActive;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        if (walkingAudioSource != null)
        {
            walkingAudioSource.loop = true;
            walkingAudioSource.playOnAwake = false;
            walkingAudioSource.volume = 0f;
        }
    }


    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            StopWalkingAudioImmediately();

            return;
        }

        UpdateWalkingSoundFade();
    }


    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            moveInput.normalized *
            moveSpeed *
            Time.fixedDeltaTime
        );

        UpdatePlayerRotation();
    }


    private void OnEnable()
    {
        moveActionReference.action.Enable();

        moveActionReference.action.performed +=
            OnMovePerformed;

        moveActionReference.action.canceled +=
            OnMoveCanceled;
    }


    private void OnDisable()
    {
        moveActionReference.action.performed -=
            OnMovePerformed;

        moveActionReference.action.canceled -=
            OnMoveCanceled;

        moveActionReference.action.Disable();

        StopWalkingAudioImmediately();
    }


    private void OnMovePerformed(
        InputAction.CallbackContext context)
    {
        moveInput =
            context.ReadValue<Vector2>();

        bool isMoving =
            moveInput.sqrMagnitude > 0.01f;

        animationController.SetMoving(
            isMoving
        );

        walkingSoundActive =
            isMoving;

        if (
            isMoving &&
            walkingAudioSource != null &&
            walkingAudioSource.clip != null &&
            !walkingAudioSource.isPlaying
        )
        {
            walkingAudioSource.volume = 0f;

            walkingAudioSource.Play();
        }
    }


    private void OnMoveCanceled(
        InputAction.CallbackContext context)
    {
        moveInput =
            Vector2.zero;

        animationController.SetMoving(
            false
        );

        walkingSoundActive =
            false;
    }


    private void UpdatePlayerRotation()
    {
        if (
            moveInput.sqrMagnitude 
            < 0.01f
        )
        {
            return;
        }

        float angle =
            Mathf.Atan2(
                moveInput.y,
                moveInput.x
            ) *
            Mathf.Rad2Deg;

        float targetAngle =
            angle - 90f;

        float smoothedRotation =
            Mathf.MoveTowardsAngle(
                rb.rotation,
                targetAngle,
                rotationSpeed *
                Time.fixedDeltaTime
            );

        rb.MoveRotation(
            smoothedRotation
        );
    }


    private void UpdateWalkingSoundFade()
    {
        if (walkingAudioSource == null)
        {
            return;
        }

        float targetVolume =
            walkingSoundActive
                ? walkingVolume
                : 0f;

        float duration =
            walkingSoundActive
                ? fadeInDuration
                : fadeOutDuration;

        if (duration <= 0f)
        {
            walkingAudioSource.volume =
                targetVolume;
        }
        else
        {
            float fadeSpeed =
                walkingVolume /
                duration;

            walkingAudioSource.volume =
                Mathf.MoveTowards(
                    walkingAudioSource.volume,
                    targetVolume,
                    fadeSpeed *
                    Time.deltaTime
                );
        }

        if (
            !walkingSoundActive &&
            walkingAudioSource.isPlaying &&
            walkingAudioSource.volume <= 0.001f
        )
        {
            walkingAudioSource.Stop();
            walkingAudioSource.volume = 0f;
        }
    }


    private void StopWalkingAudioImmediately()
    {
        walkingSoundActive = false;

        if (walkingAudioSource != null)
        {
            walkingAudioSource.Stop();
            walkingAudioSource.volume = 0f;
        }
    }


    public float GetMoveSpeed()
    {
        return moveSpeed;
    }


    public void SetMoveSpeed(
        float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}