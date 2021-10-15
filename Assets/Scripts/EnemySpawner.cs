using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int threatLevel;
    [SerializeField] float roundGap;
    [SerializeField] int[] enemiesPerWave;

    // Store all spawn points
    EnemySpawnPoint[] spawnPoints;

    // All enemy types
    public GameObject[] enemyPrefabs;

    Player player;
    float timer;
    bool roundActive;
    bool enemiesSpawned;
    int enemiesKilled;

    void Start()
    {
        // Find all the existing spawn points
        spawnPoints = FindObjectsOfType<EnemySpawnPoint>();

        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        // Calculate round gap time and start next round after round gap time is up
        if (!roundActive)
        {
            timer += Time.deltaTime;

            if (timer >= roundGap)
            {
                timer = 0f;
                NextRound();
            }
        }
        // Handle enemy spawning while a round is active
        else if (!enemiesSpawned)
        {
            for (int i = 0; i < enemiesPerWave[Mathf.Clamp(threatLevel - 1, 0, enemiesPerWave.Length)]; i++)
            {
                EnemySpawnPoint es = spawnPoints[i % spawnPoints.Length];

                // Spawn enemies in the current zone and adjacent ones.
                if (es.mapZone == player.currentMapZone)
                {
                    i += es.SpawnEnemy(enemyPrefabs[1]) - 1;
                }
                Debug.Log(i);
            }

            enemiesSpawned = true;
        }
    }

    void NextRound()
    {
        threatLevel++;
        enemiesKilled = 0;
        roundActive = true;
    }

    void EndRound()
    {
        roundActive = false;
        enemiesSpawned = false;
    }

    public void AddTally()
    {
        enemiesKilled++;

        // If all the enemies in this wave have been killed, end the round
        if (enemiesKilled >= enemiesPerWave[threatLevel - 1])
        {
            EndRound();
        }
    }
}
