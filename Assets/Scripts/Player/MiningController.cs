using UnityEngine;
using UnityEngine.InputSystem;

public class MiningController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MineGenerator mineGenerator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PickaxeController pickaxe;

    [Header("Mining Settings")]
    [SerializeField] private float miningRange = 3f;
    [SerializeField] private float miningInterval = 0.2f;

    [Header("Range Visual")]
    [SerializeField] private LineRenderer rangeLine;
    [SerializeField] private int circleSegments = 64;

    [Header("Target Visual")]
    [SerializeField] private LineRenderer targetLine;

    [Header("Combat")]
    [SerializeField] private float enemyAttackCooldown = 0.4f;

    private float nextEnemyAttackTime;

    private float nextMineTime;


    private void Start()
    {
        DrawMiningRange();
        SetupTargetLine();
    }


    private void Update()
    {
        UpdateTargetVisual();

        if (
            Mouse.current.leftButton.isPressed &&
            Time.time >= nextMineTime
        )
        {
            Mine();

            nextMineTime =
                Time.time + miningInterval;
        }
    }


    private void Mine()
    {
        Vector3 mouseWorldPosition =
            GetMouseWorldPosition();

        if (TryAttackEnemy(mouseWorldPosition))
        {
            return;
        }

        Vector3 cellCenter =
            mineGenerator.GetCellCenter(mouseWorldPosition);

        float distance =
            Vector2.Distance(
                transform.position,
                cellCenter
            );

        // Predaleko
        if (distance > miningRange)
        {
            return;
        }

        // Drugi kamen blokira ovaj kamen
        if (!mineGenerator.IsRockExposedToPlayer(
                transform.position,
                mouseWorldPosition))
        {
            return;
        }

        mineGenerator.MineTile(mouseWorldPosition, pickaxe.GetMiningPower()
);
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
        Vector3 mouseWorldPosition =
            GetMouseWorldPosition();

        // Ako miš nije na stijeni,
        // sakrij outline.
        if (!mineGenerator.HasRockAtWorldPosition(mouseWorldPosition))
        {
            targetLine.enabled = false;
            return;
        }

        targetLine.enabled = true;

        Vector3 cellCenter =
            mineGenerator.GetCellCenter(mouseWorldPosition);

        DrawTargetSquare(cellCenter);

        float distance =
            Vector2.Distance(
                transform.position,
                cellCenter
            );

        bool isInRange =
            distance <= miningRange;

        bool isExposed =
            mineGenerator.IsRockExposedToPlayer(
                transform.position,
                mouseWorldPosition
            );

        if (isInRange && isExposed)
        {
            targetLine.startColor = Color.green;
            targetLine.endColor = Color.green;
        }
        else
        {
            targetLine.startColor = Color.red;
            targetLine.endColor = Color.red;
        }
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


    private void DrawTargetSquare(Vector3 center)
    {
        float halfSize = 0.5f;

        // Stavljamo outline bliže kameri
        center.z = -2f;

        targetLine.SetPosition(
            0,
            center + new Vector3(-halfSize, -halfSize, 0)
        );

        targetLine.SetPosition(
            1,
            center + new Vector3(-halfSize, halfSize, 0)
        );

        targetLine.SetPosition(
            2,
            center + new Vector3(halfSize, halfSize, 0)
        );

        targetLine.SetPosition(
            3,
            center + new Vector3(halfSize, -halfSize, 0)
        );

        targetLine.SetPosition(
            4,
            center + new Vector3(-halfSize, -halfSize, 0)
        );
    }


    private void DrawMiningRange()
    {
        if (rangeLine == null)
        {
            return;
        }

        rangeLine.positionCount =
            circleSegments + 1;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle =
                i / (float)circleSegments *
                Mathf.PI * 2f;

            float x =
                Mathf.Cos(angle) *
                miningRange;

            float y =
                Mathf.Sin(angle) *
                miningRange;

            rangeLine.SetPosition(
                i,
                new Vector3(x, y, 0)
            );
        }
    }

    private bool TryAttackEnemy(Vector3 mouseWorldPosition)
    {
        Collider2D hit =
            Physics2D.OverlapPoint(mouseWorldPosition);

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

        if (distance > miningRange)
        {
            return false;
        }

        // Enemy je pogođen mišem,
        // ali pickaxe još nije spreman za novi udarac.
        if (Time.time < nextEnemyAttackTime)
        {
            return true;
        }

        enemy.TakeDamage(
            pickaxe.GetMiningPower(),
            transform.position
        );

        nextEnemyAttackTime =
            Time.time + enemyAttackCooldown;

        return true;
    }

    public float GetMiningRange()
    {
        return miningRange;
    }
}