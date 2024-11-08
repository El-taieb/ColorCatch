using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCollectible : MonoBehaviour
{
    public GameObject collectiblePrefab; // Reference to the collectible prefab
    public int numberOfCollectibles = 10; // Number of collectibles to spawn
    public Vector3 spawnAreaMin = new Vector3(-10f, 0f, -10f); // Minimum spawn position (adjust as needed)
    public Vector3 spawnAreaMax = new Vector3(10f, 0f, 10f); // Maximum spawn position (adjust as needed)
    public float minDistanceFromPlayer = 2f; // Minimum distance the collectible can spawn from the player

    public GameObject player; // Reference to the player object
    public Color[] colors; // Array to hold different colors for the collectibles
    private Renderer collectibleRenderer; // Reference to the renderer of the collectible

    // Sound effects
    public AudioClip correctSound; // Sound for correct color collection
    public AudioClip incorrectSound; // Sound for incorrect color collection
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SpawnCollectibles();
    }

    void SpawnCollectibles()
    {
        for (int i = 0; i < numberOfCollectibles; i++)
        {
            // Try to find a valid position that is not too close to the player
            Vector3 spawnPosition = GetRandomPosition();

            // Instantiate the collectible at the calculated spawn position
            GameObject collectible = Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);

            // Get the renderer component to change the color
            collectibleRenderer = collectible.GetComponent<Renderer>();

            // Randomly select a color from the colors array
            collectibleRenderer.material.color = colors[Random.Range(0, colors.Length)];

            // Optionally, you can assign each collectible a tag or name for identification
            collectible.name = "ColorCollectible";
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 spawnPosition;
        bool validPosition = false;

        // Try to get a valid spawn position that is not too close to the player
        while (!validPosition)
        {
            spawnPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                0f, // Ensure it spawns at the same Y position (adjust as needed)
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            // Check if the position is too close to the player
            if (Vector3.Distance(spawnPosition, player.transform.position) >= minDistanceFromPlayer)
            {
                validPosition = true;
            }
        }

        return spawnPosition;
    }

    // This function should be triggered by the player's collision with the collectible
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if the color of the collectible matches the target color
            if (collectibleRenderer.material.color == PlayerScore.Instance.targetColor)
            {
                // Correct color collected
                PlayerScore.Instance.AddScore(1);
                PlaySound(correctSound);
            }
            else
            {
                // Incorrect color collected
                PlayerScore.Instance.AddScore(-1);
                PlaySound(incorrectSound);
            }

            // Destroy the collectible after being collected
            Destroy(gameObject);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

