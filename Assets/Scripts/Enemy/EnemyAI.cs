using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.2f;


    [Header("Vision")]
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField]
    private float losePlayerAfter = 1.5f;


    [Header("Pathfinding")]
    [SerializeField]
    private float pathRefreshInterval = 0.75f;

    [SerializeField]
    private float waypointReachedDistance = 0.15f;

    [SerializeField]
    private int maxPathSearchNodes = 5000;


    [Header("Attack")]
    [SerializeField] private int damage = 10;

    [SerializeField]
    private float attackCooldown = 1f;

    [SerializeField]
    private float attackAnimationDuration = 0.35f;


    [Header("Animation")]
    [SerializeField]
    private EnemyAnimationController animationController;


    private Transform player;
    private PlayerHealth playerHealth;
    private MineGenerator mineGenerator;

    private Rigidbody2D rb;


    private List<Vector2Int> currentPath =
        new List<Vector2Int>();

    private int pathIndex;

    private float pathRefreshTimer;
    private float nextAttackTime;
    private float lostSightTimer;

    private bool playerDetected;
    private bool isAttacking;

    private Vector2Int lastKnownPlayerCell;


    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    public void SetReferences(
        Transform playerTransform,
        MineGenerator generator)
    {
        player = playerTransform;
        mineGenerator = generator;

        if (player != null)
        {
            playerHealth =
                player.GetComponent<PlayerHealth>();
        }
    }


    private void FixedUpdate()
    {
        if (
            player == null ||
            mineGenerator == null
        )
        {
            StopMovement();

            return;
        }


        float distanceToPlayer =
            Vector2.Distance(
                rb.position,
                player.position
            );


        bool canSeePlayer =
            distanceToPlayer <= detectionRange &&
            CanSeePlayer();


        // Player je trenutno vidljiv.
        if (canSeePlayer)
        {
            playerDetected = true;

            lostSightTimer = 0f;

            UpdateLastKnownPlayerPosition();
        }

        // Enemy je prije vidio Playera,
        // ali ga sada više ne vidi.
        else if (playerDetected)
        {
            lostSightTimer +=
                Time.fixedDeltaTime;


            if (
                lostSightTimer >=
                losePlayerAfter
            )
            {
                ForgetPlayer();

                return;
            }
        }


        // VAŽNO:
        // Enemy ne smije računati path
        // dok još nije detektirao Playera.
        if (!playerDetected)
        {
            StopMovement();

            return;
        }


        // Player je dovoljno blizu za napad.
        if (
            canSeePlayer &&
            distanceToPlayer <= attackRange
        )
        {
            currentPath.Clear();

            StopMovement();

            FaceDirection(
                (Vector2)player.position -
                rb.position
            );

            TryAttack();

            return;
        }


        pathRefreshTimer -=
            Time.fixedDeltaTime;


        // Path se računa samo kada timer istekne.
        // Ne računamo ga ponovno svaki FixedUpdate
        // samo zato što je path prazan.
        if (pathRefreshTimer <= 0f)
        {
            RecalculatePath();

            pathRefreshTimer =
                pathRefreshInterval;
        }


        FollowPath();


        // Enemy je stigao do posljednje poznate
        // pozicije Playera, ali ga tamo ne vidi.
        if (
            !canSeePlayer &&
            HasReachedLastKnownPosition()
        )
        {
            ForgetPlayer();
        }
    }


    private bool CanSeePlayer()
    {
        Vector2 direction =
            (
                (Vector2)player.position -
                rb.position
            ).normalized;


        float distance =
            Vector2.Distance(
                rb.position,
                player.position
            );


        RaycastHit2D hit =
            Physics2D.Raycast(
                rb.position,
                direction,
                distance,
                obstacleLayer
            );


        return hit.collider == null;
    }


    private void UpdateLastKnownPlayerPosition()
    {
        if (
            mineGenerator
                .TryGetMapCellFromWorldPosition(
                    player.position,
                    out Vector2Int playerCell
                )
        )
        {
            lastKnownPlayerCell =
                playerCell;
        }
    }


    private void RecalculatePath()
    {
        if (
            !mineGenerator
                .TryGetMapCellFromWorldPosition(
                    transform.position,
                    out Vector2Int enemyCell
                )
        )
        {
            currentPath.Clear();

            return;
        }


        currentPath =
            FindPath(
                enemyCell,
                lastKnownPlayerCell
            );


        pathIndex = 0;
    }


    private List<Vector2Int> FindPath(
        Vector2Int start,
        Vector2Int target)
    {
        Queue<Vector2Int> queue =
            new Queue<Vector2Int>();


        HashSet<Vector2Int> visited =
            new HashSet<Vector2Int>();


        Dictionary<Vector2Int, Vector2Int>
            cameFrom =
                new Dictionary<
                    Vector2Int,
                    Vector2Int
                >();


        queue.Enqueue(start);

        visited.Add(start);


        int searchedNodes = 0;


        while (queue.Count > 0)
        {
            searchedNodes++;


            // Sigurnosni limit.
            // Enemy nikada ne smije pregledavati
            // cijelu ogromnu mapu.
            if (
                searchedNodes >=
                maxPathSearchNodes
            )
            {
                return new List<Vector2Int>();
            }


            Vector2Int current =
                queue.Dequeue();


            if (current == target)
            {
                return ReconstructPath(
                    cameFrom,
                    start,
                    target
                );
            }


            foreach (
                Vector2Int direction
                in directions
            )
            {
                Vector2Int neighbour =
                    current +
                    direction;


                if (
                    visited.Contains(
                        neighbour
                    )
                )
                {
                    continue;
                }


                if (
                    !mineGenerator
                        .IsWalkableCell(
                            neighbour
                        )
                )
                {
                    continue;
                }


                visited.Add(
                    neighbour
                );


                cameFrom[neighbour] =
                    current;


                queue.Enqueue(
                    neighbour
                );
            }
        }


        return new List<Vector2Int>();
    }


    private List<Vector2Int> ReconstructPath(
        Dictionary<Vector2Int, Vector2Int> cameFrom,
        Vector2Int start,
        Vector2Int target)
    {
        List<Vector2Int> path =
            new List<Vector2Int>();


        Vector2Int current =
            target;


        while (current != start)
        {
            path.Add(current);


            if (
                !cameFrom.ContainsKey(
                    current
                )
            )
            {
                return new List<Vector2Int>();
            }


            current =
                cameFrom[current];
        }


        path.Reverse();


        return path;
    }


    private void FollowPath()
    {
        if (
            currentPath == null ||
            pathIndex >= currentPath.Count
        )
        {
            SetMoving(false);

            return;
        }


        Vector3 waypointWorld =
            mineGenerator
                .GetWorldPositionFromMapCell(
                    currentPath[pathIndex]
                );


        Vector2 targetPosition =
            new Vector2(
                waypointWorld.x,
                waypointWorld.y
            );


        Vector2 moveDirection =
            targetPosition -
            rb.position;


        if (
            moveDirection.sqrMagnitude >
            0.001f
        )
        {
            FaceDirection(
                moveDirection
            );

            SetMoving(true);
        }
        else
        {
            SetMoving(false);
        }


        Vector2 newPosition =
            Vector2.MoveTowards(
                rb.position,
                targetPosition,
                moveSpeed *
                Time.fixedDeltaTime
            );


        rb.MovePosition(
            newPosition
        );


        float distanceToWaypoint =
            Vector2.Distance(
                newPosition,
                targetPosition
            );


        if (
            distanceToWaypoint <=
            waypointReachedDistance
        )
        {
            pathIndex++;
        }
    }


    private void FaceDirection(
        Vector2 direction)
    {
        if (
            direction.sqrMagnitude <
            0.001f
        )
        {
            return;
        }


        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;


        // Sprite u rotaciji 0°
        // pretpostavljeno gleda prema gore.
        float targetRotation =
            angle - 90f;


        rb.MoveRotation(
            targetRotation
        );
    }


    private bool HasReachedLastKnownPosition()
    {
        Vector3 targetWorld =
            mineGenerator
                .GetWorldPositionFromMapCell(
                    lastKnownPlayerCell
                );


        float distance =
            Vector2.Distance(
                rb.position,
                targetWorld
            );


        return distance < 0.3f;
    }


    private void TryAttack()
    {
        if (
            Time.time <
            nextAttackTime
        )
        {
            return;
        }


        if (playerHealth == null)
        {
            return;
        }


        playerHealth.TakeDamage(
            damage
        );


        nextAttackTime =
            Time.time +
            attackCooldown;


        if (!isAttacking)
        {
            StartCoroutine(
                PlayAttackAnimation()
            );
        }
    }


    private IEnumerator PlayAttackAnimation()
    {
        isAttacking = true;


        if (
            animationController != null
        )
        {
            animationController
                .SetAttacking(true);

            animationController
                .SetMoving(false);
        }


        yield return new WaitForSeconds(
            attackAnimationDuration
        );


        isAttacking = false;


        if (
            animationController != null
        )
        {
            animationController
                .SetAttacking(false);
        }
    }


    private void SetMoving(
        bool moving)
    {
        if (
            animationController == null
        )
        {
            return;
        }


        animationController.SetMoving(
            moving &&
            !isAttacking
        );
    }


    private void StopMovement()
    {
        rb.linearVelocity =
            Vector2.zero;


        SetMoving(false);
    }


    private void ForgetPlayer()
    {
        playerDetected = false;

        lostSightTimer = 0f;

        currentPath.Clear();

        pathIndex = 0;

        StopMovement();
    }


    private void OnDisable()
    {
        StopAllCoroutines();


        if (
            animationController != null
        )
        {
            animationController
                .SetMoving(false);

            animationController
                .SetAttacking(false);
        }


        isAttacking = false;

        playerDetected = false;

        lostSightTimer = 0f;

        currentPath.Clear();
    }
}