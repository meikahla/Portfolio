using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning of enemy waves in the game. It handles the spawning interval, wave progression, 
/// and tracks spawned enemies for each wave.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 5f; // Time between waves
    [SerializeField] private float spawnInterval = 0.5f; // Interval between each enemy spawn in a wave
    [SerializeField] private List<WaveConfig> waveConfigs; // Configurations for different waves
    [SerializeField] private Camera mainCamera; // Reference to the main camera for spawn positioning

    private float countdown = 2f; // Countdown timer before the next wave starts
    private int currentWaveIndex = 0; // Tracks the current wave index
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // List to track spawned enemies

    private void Awake()
    {
        // Ensure there's only one instance of WaveManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (countdown <= 0f)
        {
            // Start the next wave if available
            if (currentWaveIndex < waveConfigs.Count)
            {
                StartCoroutine(SpawnWave(waveConfigs[currentWaveIndex]));
                currentWaveIndex++;
            }

            // Reset countdown to wait before the next wave
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;
    }

    /// <summary>
    /// Spawns all enemies in the current wave.
    /// </summary>
    /// <param name="waveConfig">The configuration of the wave to spawn.</param>
    /// <returns>Coroutine for spawning enemies with intervals.</returns>
    private IEnumerator SpawnWave(WaveConfig waveConfig)
    {
        for (int i = 0; i < waveConfig.enemyCount; i++)
        {
            SpawnEnemy(waveConfig);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Spawns a single enemy based on the given wave configuration.
    /// </summary>
    /// <param name="waveConfig">The wave configuration to select an enemy type.</param>
    private void SpawnEnemy(WaveConfig waveConfig)
    {
        // Get a random spawn position and side
        (Vector3 spawnPosition, SpawnSide spawnSide, bool flipSprite) = GetRandomSpawnPosition();

        // Select a random enemy type from the wave configuration
        EnemyType enemyType = waveConfig.enemyTypes[Random.Range(0, waveConfig.enemyTypes.Count)];

        // Instantiate the selected enemy
        GameObject enemy = Instantiate(enemyType.enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(enemy);

        // Initialize enemy properties
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.Initialize(enemyType.health, enemyType.damage, enemyType.speed, enemyType.goldDrop);

        // Set the enemy's animation layer and flip the sprite if necessary
        Animator animator = enemy.GetComponent<Animator>();
        SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();

        switch (spawnSide)
        {
            case SpawnSide.Up:
                animator.SetLayerWeight(animator.GetLayerIndex("Up"), 1);
                break;
            case SpawnSide.Down:
                animator.SetLayerWeight(animator.GetLayerIndex("Down"), 1);
                break;
            case SpawnSide.Side:
                animator.SetLayerWeight(animator.GetLayerIndex("Side"), 1);
                break;
        }

        // Flip the sprite if the enemy spawns on the left
        if (flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// Gets a random spawn position outside of the camera's view, either above, below, or to the sides.
    /// </summary>
    /// <returns>A tuple containing the spawn position, spawn side, and whether the sprite should be flipped.</returns>
    private (Vector3, SpawnSide, bool) GetRandomSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        SpawnSide spawnSide = SpawnSide.Side;
        bool flipSprite = false;

        // Get the camera's boundaries
        float screenX = Random.Range(0, Screen.width);
        float screenY = Random.Range(0, Screen.height);

        Vector3 randomScreenPoint = new Vector3(screenX, screenY, mainCamera.nearClipPlane);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(randomScreenPoint);

        // Decide to spawn either above, below, left, or right outside the camera's view
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // Spawn above the screen
                spawnPosition = new Vector3(worldPosition.x, worldPosition.y + mainCamera.orthographicSize + 1, 0);
                spawnSide = SpawnSide.Down; // Set animation layer to Down
                break;
            case 1: // Spawn below the screen
                spawnPosition = new Vector3(worldPosition.x, worldPosition.y - mainCamera.orthographicSize - 1, 0);
                spawnSide = SpawnSide.Up; // Set animation layer to Up
                break;
            case 2: // Spawn to the right of the screen
                spawnPosition = new Vector3(worldPosition.x + mainCamera.aspect * mainCamera.orthographicSize + 1, worldPosition.y, 0);
                spawnSide = SpawnSide.Side; // Set animation layer to Side
                break;
            case 3: // Spawn to the left of the screen
                spawnPosition = new Vector3(worldPosition.x - mainCamera.aspect * mainCamera.orthographicSize - 1, worldPosition.y, 0);
                spawnSide = SpawnSide.Side; // Set animation layer to Side
                flipSprite = true; // Flip the sprite for left spawns
                break;
        }

        return (spawnPosition, spawnSide, flipSprite);
    }
}

/// <summary>
/// Defines the type of enemy to spawn with various attributes such as health, damage, and speed.
/// </summary>
[System.Serializable]
public class EnemyType
{
    public GameObject enemyPrefab; // The enemy prefab to instantiate
    public float health; // The health of the enemy
    public float damage; // The damage dealt by the enemy
    public float speed; // The speed of the enemy
    public int goldDrop; // The amount of gold dropped by the enemy upon death
}

/// <summary>
/// Configurations for a wave, including the number of enemies and types of enemies to spawn.
/// </summary>
[System.Serializable]
public class WaveConfig
{
    public int enemyCount; // Number of enemies to spawn in this wave
    public List<EnemyType> enemyTypes; // Types of enemies to spawn in this wave
}

/// <summary>
/// Enum representing the spawn sides: Up, Down, and Side.
/// </summary>
public enum SpawnSide
{
    Up,      // Spawn above the screen
    Down,    // Spawn below the screen
    Side     // Spawn to the left or right of the screen
}
