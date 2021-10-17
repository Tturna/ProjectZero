using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public float adjacencyRange;
    public int mapZone;

    // Spawn enemy
    // Call nearby spawners to also spawn an enemy
    public int SpawnEnemy(GameObject enemy)
    {
        int spawnedEnemies = 1;

        Instantiate(enemy, transform.position, Quaternion.identity);

        // Find all nearby spawners and spawn enemies
        foreach (EnemySpawnPoint es in FindObjectsOfType<EnemySpawnPoint>())
        {
            if (es == this) continue;

            float distance = (transform.position - es.transform.position).magnitude;

            // Check if the spawn point is close enough to this one to be considered "adjacent"
            if (distance < adjacencyRange)
            {
                // Have it spawn an enemy
                spawnedEnemies++;
                es.SpawnEnemy(enemy);
            }
        }

        return spawnedEnemies;
    }
}
