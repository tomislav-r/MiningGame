using UnityEngine;
using UnityEngine.InputSystem;

public class MiningController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MineGenerator mineGenerator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PickaxeController pickaxe;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("Mining Settings")]
    [SerializeField] private float miningInterval = 0.2f;

    // Više se ne koristi za rudarenje.
    // Ostavljen je samo zbog postojećih skripti
    // koje možda koriste GetMiningRange().
    [SerializeField] private float miningRange = 1.5f;

    [Header("Target Visual")]
    [SerializeField] private LineRenderer targetLine;

    [Header("Combat")]
    [SerializeField] private float enemyAttackRange = 1.5f;
    [SerializeField] private float enemyAttackCooldown = 0.4f;

    [Header("Audio")]
    [SerializeField] private AudioSource miningAudioSource;
    [SerializeField] private float miningVolume = 1f;
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField] private float fadeOutDuration = 0.2f;

    private bool miningSoundActive;

    private float nextEnemyAttackTime;
    private float nextMineTime;


    private void Start()
    {
        SetupTargetLine();

        if (miningAudioSource != null)
        {
            miningAudioSource.loop = true;
            miningAudioSource.playOnAwake = false;
            miningAudioSource.volume = 0f;
        }
    }


    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            StopMiningEffects();
            StopMiningAudioImmediately();

            if (targetLine != null)
            {
                targetLine.enabled = false;
            }

            return;
        }

        UpdateTargetVisual();

        bool leftMousePressed =
            Mouse.current.leftButton.isPressed;

        if (!leftMousePressed)
        {
            StopMiningEffects();
            return;
        }

        if (Time.time >= nextMineTime)
        {
            bool miningSuccessful =
                Mine();

            if (animationController != null)
            {
                animationController.SetMining(
                    miningSuccessful
                );
            }

            if (cameraFollow != null)
            {
                cameraFollow.SetMiningShake(
                    miningSuccessful
                );
            }

            if (!miningSuccessful)
            {
                StopMiningSound();
            }

            nextMineTime =
                Time.time + miningInterval;
        }
    }


    private bool Mine()
    {
        Vector3 mouseWorldPosition =
            GetMouseWorldPosition();

        // Napad na neprijatelja i dalje koristi miš.
        if (TryAttackEnemy(mouseWorldPosition))
        {
            StopMiningSound();
            return true;
        }

        // Za stijene se rudari isključivo
        // susjedni blok direktno ispred Playera.
        if (!TryGetFrontMineableCell(
                out Vector3 frontCellWorldPosition))
        {
            StopMiningSound();
            return false;
        }

        mineGenerator.MineTile(
            frontCellWorldPosition,
            pickaxe.GetMiningPower()
        );

        StartMiningSound();

        return true;
    }


    private bool TryGetFrontMineableCell(
        out Vector3 frontCellWorldPosition)
    {
        frontCellWorldPosition =
            Vector3.zero;

        if (!mineGenerator.TryGetMapCellFromWorldPosition(
                transform.position,
                out Vector2Int playerCell))
        {
            return false;
        }

        Vector2 forward = transform.up.normalized;

        int directionX = Mathf.RoundToInt(forward.x);

        int directionY = Mathf.RoundToInt(forward.y);

        Vector2Int direction = new Vector2Int(directionX, directionY);

        if (direction == Vector2Int.zero)
        {
            return false;
        }

        Vector2Int frontCell = playerCell + direction;

        frontCellWorldPosition =
            mineGenerator.GetWorldPositionFromMapCell(
                frontCell
            );

        if (!mineGenerator.HasRockAtWorldPosition(
                frontCellWorldPosition))
        {
            return false;
        }

        // Ova provjera koja sprječava pokušaj rudarenja Obsidiana
        if (!mineGenerator.IsRockExposedToPlayer(
                transform.position,
                frontCellWorldPosition))
        {
            return false;
        }

        return true;
    }


    private void StartMiningSound()
    {
        if (
            miningAudioSource == null ||
            miningAudioSource.clip == null
        )
        {
            return;
        }

        miningSoundActive = true;

        if (!miningAudioSource.isPlaying)
        {
            miningAudioSource.volume = 0f;
            miningAudioSource.Play();
        }
    }


    private void StopMiningSound()
    {
        miningSoundActive = false;
    }


    private void StopMiningEffects()
    {
        if (animationController != null)
        {
            animationController.SetMining(false);
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetMiningShake(false);
        }

        StopMiningSound();
    }


    private void UpdateMiningSoundFade()
    {
        if (miningAudioSource == null)
        {
            return;
        }

        float targetVolume =
            miningSoundActive
                ? miningVolume
                : 0f;

        float duration =
            miningSoundActive
                ? fadeInDuration
                : fadeOutDuration;

        if (duration <= 0f)
        {
            miningAudioSource.volume =
                targetVolume;
        }
        else
        {
            float fadeSpeed =
                miningVolume /
                duration;

            miningAudioSource.volume =
                Mathf.MoveTowards(
                    miningAudioSource.volume,
                    targetVolume,
                    fadeSpeed *
                    Time.deltaTime
                );
        }

        if (
            !miningSoundActive &&
            miningAudioSource.isPlaying &&
            miningAudioSource.volume <= 0.001f
        )
        {
            miningAudioSource.Stop();
            miningAudioSource.volume = 0f;
        }
    }


    private void StopMiningAudioImmediately()
    {
        miningSoundActive = false;

        if (miningAudioSource != null)
        {
            miningAudioSource.Stop();
            miningAudioSource.volume = 0f;
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                mouseScreenPosition
            );

        mouseWorldPosition.z = 0;

        return mouseWorldPosition;
    }


    private void UpdateTargetVisual()
    {
        if (targetLine == null)
        {
            return;
        }

        if (!TryGetFrontMineableCell(
                out Vector3 frontCellWorldPosition))
        {
            targetLine.enabled = false;
            return;
        }

        targetLine.enabled = true;

        targetLine.startColor =
            Color.green;

        targetLine.endColor =
            Color.green;

        DrawTargetSquare(
            frontCellWorldPosition
        );
    }


    private void SetupTargetLine()
    {
        if (targetLine == null)
        {
            return;
        }

        targetLine.positionCount = 5;
        targetLine.enabled = false;
    }


    private void DrawTargetSquare(
        Vector3 center)
    {
        Vector2 cellSize =
            mineGenerator.GetCellWorldSize();

        float halfWidth =
            cellSize.x * 0.5f;

        float halfHeight =
            cellSize.y * 0.5f;

        center.z = -2f;

        targetLine.SetPosition(
            0,
            center + new Vector3(
                -halfWidth,
                -halfHeight,
                0
            )
        );

        targetLine.SetPosition(
            1,
            center + new Vector3(
                -halfWidth,
                halfHeight,
                0
            )
        );

        targetLine.SetPosition(
            2,
            center + new Vector3(
                halfWidth,
                halfHeight,
                0
            )
        );

        targetLine.SetPosition(
            3,
            center + new Vector3(
                halfWidth,
                -halfHeight,
                0
            )
        );

        targetLine.SetPosition(
            4,
            center + new Vector3(
                -halfWidth,
                -halfHeight,
                0
            )
        );
    }


    private bool TryAttackEnemy(
        Vector3 mouseWorldPosition)
    {
        Collider2D hit =
            Physics2D.OverlapPoint(
                mouseWorldPosition
            );

        if (hit == null)
        {
            return false;
        }

        EnemyHealth enemy =
            hit.GetComponent<EnemyHealth>();

        if (enemy == null)
        {
            return false;
        }

        float distance =
            Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

        if (distance > enemyAttackRange)
        {
            return false;
        }

        if (
            Time.time <
            nextEnemyAttackTime
        )
        {
            return true;
        }

        enemy.TakeDamage(
            pickaxe.GetMiningPower(),
            transform.position
        );

        nextEnemyAttackTime =
            Time.time +
            enemyAttackCooldown;

        return true;
    }


    public float GetMiningRange()
    {
        return miningRange;
    }


    private void LateUpdate()
    {
        UpdateMiningSoundFade();
    }


    private void OnDisable()
    {
        StopMiningAudioImmediately();

        if (animationController != null)
        {
            animationController.SetMining(false);
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetMiningShake(false);
        }
    }
}
