using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int threatLevel;
    [SerializeField] float roundGap;
    [SerializeField] float enemySpawnGap;
    [SerializeField] int[] enemiesPerWave;

    // Store all spawn points
    EnemySpawnPoint[] spawnPoints;

    // All enemy types
    public GameObject[] enemyPrefabs;

    Player player;
    float timer;
    bool roundActive;
    bool haveEnemiesSpawned;
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
        else if (!haveEnemiesSpawned)
        {
            haveEnemiesSpawned = true;
            StartCoroutine(SpawnEnemies());
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
        haveEnemiesSpawned = false;
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesPerWave[Mathf.Clamp(threatLevel - 1, 0, enemiesPerWave.Length)]; i++)
        {
            EnemySpawnPoint es = spawnPoints[i % spawnPoints.Length];

            // Spawn enemies in the current zone and adjacent ones.
            if (es.mapZone == player.currentMapZone)
            {
                // Choose enemy type
                // Array order determines chance of selection
                int idx = 0;
                if (enemyPrefabs.Length > 1)
                {
                    float[] r = new float[enemyPrefabs.Length];
                    r[0] = 60;
                    r[1] = 40;

                    // Random 0-100
                    float roll = Random.Range(0, 100);

                    for (int n = 0; n < r.Length; n++)
                    {
                        if (n > 0) r[n] = r[n] / 2;

                        // Calculate selection
                        roll -= r[n];
                        if (roll <= 0)
                        {
                            idx = n;
                            Debug.Log(idx);
                            break;
                        }

                        if (n + 1 < r.Length - 1) r[n + 1] = r[n];
                    }
                }

                es.SpawnEnemy(enemyPrefabs[idx]); // Spawn enemy
                i += es.SpawnNearby(enemyPrefabs[idx]); // Spawn enemies in nearby zones as well
            }
            else continue;

            yield return new WaitForSeconds(enemySpawnGap);
        }
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
