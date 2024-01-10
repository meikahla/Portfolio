using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    WaveController waveController;

    Transform tower;

    int currentWave;
    bool isSpawn;
    bool waveDone;

    [SerializeField]
    [Tooltip("Time for each wave")]
    float nextWaveTime;

    float _spawnRange = 10.0f;

    private void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        waveController = WaveController.Instance;
        nextWaveTime = waveController.timeBeforeWave;
        currentWave = 0;
        waveDone = true;
    }

    private void Update()
    {
        if(isSpawn)
        {
            return;
        }
        else
        {
            if (waveDone)
            {
                if(Time.time > nextWaveTime)
                {
                    SpawnWave();
                }
            }

        }
    }

    void SpawnWave()
    {
        if(currentWave < waveController.enemyWave.Length)
        {
            var wave = waveController.enemyWave[currentWave];

            StartCoroutine(SpawnEnemiesWithDelay(wave));
            waveDone = false;
            currentWave++;
        }
        else
        {
            isSpawn = true;
            Debug.Log("All wave have spawned");
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        //float spawnX = Random.Range(-_spawnRange, _spawnRange);
        //Vector2 _spawnPos = new Vector2(spawnX, Camera.main.orthographicSize + 1);
        //Instantiate(enemyPrefab, _spawnPos, Quaternion.identity);

        // Calculate a random angle
        float randomAngle = Random.Range(0f, 360f);

        // Convert angle to radians
        float angleInRadians = randomAngle * Mathf.Deg2Rad;

        // Calculate spawn position outside the specified radius
        float spawnX = tower.position.x + _spawnRange * Mathf.Cos(angleInRadians);
        float spawnY = tower.position.y + _spawnRange * Mathf.Sin(angleInRadians);

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

        // Instantiate and position the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    IEnumerator SpawnEnemiesWithDelay(WaveController.EnemyInWave wave)
    {
        for (int i = 0; i < wave.enemyType.Length; i++)
        {
            for (int j = 0; j < wave.enemyCount; j++)
            {
                SpawnEnemy(wave.enemyType[i]);
                yield return new WaitForSeconds(waveController.delayBetweenEnemy);
            }
        }
        waveDone = true;
        nextWaveTime = Time.time + waveController.timeBeforeWave;
    }
}
