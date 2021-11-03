using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public float adjacencyRange;
    public int mapZone;

    // Spawn enemy
    public void SpawnEnemy(GameObject enemy)
    {
        Instantiate(enemy, transform.position, Quaternion.identity);
        Debug.Log("Spawning...");
    }

    // Call nearby spawners to spawn an enemy
    public int SpawnNearby(GameObject enemy)
    {
        int spawnedEnemies = 0;

        // Find all nearby spawners and spawn enemies
        foreach (EnemySpawnPoint es in FindObjectsOfType<EnemySpawnPoint>())
        {
            if (es == this) continue;

            // Check if the spawn point is in an unlocked zone
            if (!GameManager.unlockedZones.Contains(es.mapZone)) continue;

            // Check if the spawn point is close enough to this one to be considered "adjacent"
            float distance = (transform.position - es.transform.position).magnitude;

            if (distance < adjacencyRange)
            {
                // Have it spawn an enemy
                Debug.Log("Spawning nearby...");
                spawnedEnemies++;
                es.SpawnEnemy(enemy);
            }
        }

        return spawnedEnemies;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.DrawWireSphere(transform.position, adjacencyRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Zone")
        {
            mapZone = collision.gameObject.GetComponent<MapZone>().zoneIndex;
            gameObject.GetComponent<Rigidbody2D>().simulated = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
