using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MineGenerator mineGenerator;
    [SerializeField] private Transform player;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int region2EnemyCount = 3;
    [SerializeField] private int region3EnemyCount = 5;

    [SerializeField] private float minimumDistanceFromPlayer = 8f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private static int spawnCallCounter = 0;


    public void SpawnEnemies()
    {
        spawnCallCounter++;

        Debug.Log(
            $"SpawnEnemies call #{spawnCallCounter} | " +
            $"Spawner: {gameObject.name} | " +
            $"ID: {GetInstanceID()} | " +
            $"Frame: {Time.frameCount}"
        );

        ClearEnemies();

        SpawnEnemiesInRegion(
            2,
            basicEnemyPrefab,
            region2EnemyCount
        );

        SpawnEnemiesInRegion(
            3,
            heavyEnemyPrefab,
            region3EnemyCount
        );
    }


    private void SpawnEnemiesInRegion(
        int region,
        GameObject enemyPrefab,
        int amount)
    {
        Debug.Log(
            $"Spawn regija {region}, " +
            $"traženi broj neprijatelja: {amount}"
        );

        if (enemyPrefab == null)
        {
            return;
        }

        List<Vector2Int> possibleCells =
            mineGenerator.GetEmptyCellsInRegion(
                region
            );

        int spawned = 0;
        int attempts = 0;

        while (
            spawned < amount &&
            attempts < 500 &&
            possibleCells.Count > 0
        )
        {
            attempts++;

            int randomIndex =
                Random.Range(
                    0,
                    possibleCells.Count
                );

            Vector2Int cell =
                possibleCells[randomIndex];

            possibleCells.RemoveAt(
                randomIndex
            );

            Vector3 worldPosition =
                mineGenerator
                    .GetWorldPositionFromMapCell(
                        cell
                    );

            float distance =
                Vector2.Distance(
                    player.position,
                    worldPosition
                );

            if (
                distance <
                minimumDistanceFromPlayer
            )
            {
                continue;
            }

            GameObject enemy =
                Instantiate(
                    enemyPrefab,
                    worldPosition,
                    Quaternion.identity
                );

            Debug.Log(
                $"STVOREN ENEMY: {enemy.name} | " +
                $"spawned = {spawned + 1}/{amount}"
            );

            EnemyAI enemyAI =
                enemy.GetComponent<EnemyAI>();

            if (enemyAI != null)
            {
                enemyAI.SetReferences(
                    player,
                    mineGenerator
                );
            }

            // Zapamti stvorenog enemyja.
            spawnedEnemies.Add(enemy);

            // Povećaj broj spawnanih enemyja.
            spawned++;
        }
    }


    private void ClearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();
    }
}