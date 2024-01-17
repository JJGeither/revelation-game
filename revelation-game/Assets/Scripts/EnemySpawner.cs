using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign the enemy prefab in the Inspector
    public string playerTag = "Player"; // Tag of the player GameObject

    private Transform playerTransform; // Reference to the player's transform

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        StartCoroutine(SpawnEnemiesRandomly());
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }

    IEnumerator SpawnEnemiesRandomly()
    {
        while (true)
        {
            // Calculate a random time interval between 20 to 60 seconds for testing purposes
            float spawnInterval = Random.Range(60f, 80f);

            yield return new WaitForSeconds(spawnInterval);

            // Spawn the enemy near the player GameObject
            SpawnEnemyNearPlayer();
        }
    }

    void SpawnEnemyNearPlayer()
    {
        if (playerTransform != null)
        {
            // Get a random position around the player within a certain radius
            Vector3 spawnPosition = RandomPositionNearPlayer(playerTransform.position, 15f);

            // Instantiate the enemy at the calculated position
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            Debug.Log("Spawned");
        }
        else
        {
            Debug.LogWarning("Player transform not found!");
        }
    }

    Vector3 RandomPositionNearPlayer(Vector3 playerPosition, float radius)
    {
        // Generate a random direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Calculate the position offset from the player position using the random direction
        Vector3 offset = new Vector3(randomDirection.x, 0f, randomDirection.y) * radius;

        // Calculate the spawn position near the player
        Vector3 spawnPosition = playerPosition + offset;

        return spawnPosition;
    }
}
