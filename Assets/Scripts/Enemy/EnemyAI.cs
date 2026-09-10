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

    [Header("Pathfinding")]
    [SerializeField] private float pathRefreshInterval = 0.3f;
    [SerializeField] private float waypointReachedDistance = 0.15f;

    [Header("Attack")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private Transform player;
    private PlayerHealth playerHealth;
    private MineGenerator mineGenerator;

    private Rigidbody2D rb;

    private List<Vector2Int> currentPath =
        new List<Vector2Int>();

    private int pathIndex;

    private float pathRefreshTimer;
    private float nextAttackTime;

    private bool playerDetected;

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


        // Enemy prvi put detektira Playera
        // samo ako ga stvarno vidi.
        if (canSeePlayer)
        {
            playerDetected = true;

            UpdateLastKnownPlayerPosition();
        }


        // Nikada nije vidio Playera.
        if (!playerDetected)
        {
            currentPath.Clear();
            return;
        }


        // Ako ga trenutno vidi i dovoljno je blizu,
        // napada ga.
        if (
            canSeePlayer &&
            distanceToPlayer <= attackRange
        )
        {
            currentPath.Clear();

            rb.linearVelocity = Vector2.zero;

            TryAttack();

            return;
        }


        pathRefreshTimer -=
            Time.fixedDeltaTime;


        if (
            pathRefreshTimer <= 0f ||
            currentPath.Count == 0
        )
        {
            RecalculatePath();

            pathRefreshTimer =
                pathRefreshInterval;
        }


        FollowPath();


        // Ako je Enemy stigao do zadnje pozicije
        // na kojoj je vidio Playera,
        // ali ga tamo više ne vidi,
        // prestaje ga tražiti.
        if (
            !canSeePlayer &&
            HasReachedLastKnownPosition()
        )
        {
            playerDetected = false;

            currentPath.Clear();
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
            mineGenerator.TryGetMapCellFromWorldPosition(
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
            !mineGenerator.TryGetMapCellFromWorldPosition(
                transform.position,
                out Vector2Int enemyCell
            )
        )
        {
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
                new Dictionary<Vector2Int, Vector2Int>();


        queue.Enqueue(start);
        visited.Add(start);


        while (queue.Count > 0)
        {
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
                in directions)
            {
                Vector2Int neighbour =
                    current + direction;


                if (
                    visited.Contains(neighbour)
                )
                {
                    continue;
                }


                if (
                    !mineGenerator.IsWalkableCell(
                        neighbour
                    )
                )
                {
                    continue;
                }


                visited.Add(neighbour);

                cameFrom[neighbour] =
                    current;

                queue.Enqueue(neighbour);
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
            return;
        }


        Vector3 waypointWorld =
            mineGenerator.GetWorldPositionFromMapCell(
                currentPath[pathIndex]
            );


        Vector2 targetPosition =
            new Vector2(
                waypointWorld.x,
                waypointWorld.y
            );


        Vector2 newPosition =
            Vector2.MoveTowards(
                rb.position,
                targetPosition,
                moveSpeed *
                Time.fixedDeltaTime
            );


        rb.MovePosition(newPosition);


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


    private bool HasReachedLastKnownPosition()
    {
        Vector3 targetWorld =
            mineGenerator.GetWorldPositionFromMapCell(
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
        if (Time.time < nextAttackTime)
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
    }
}