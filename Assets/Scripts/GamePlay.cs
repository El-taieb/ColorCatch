using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Import this to use SceneManager

public class GamePlay : MonoBehaviour
{
    public GameObject PickupOriginal;
    public GameObject PickupContainer;

    public GameObject DontPickup;
    public GameObject DontPickupContainer;

    private List<Vector3> usedPositions = new List<Vector3>(); // To track used positions
    private const float ObjectHeight = 0.5f;  // The height above the ground

    void Start()
    {
        CreatePickup(20);  // Create 20 pickups and 20 dontpicks
    }

    void CreatePickup(int pickupNum)
    {
        for (int i = 0; i < pickupNum; i++)
        {
            // Randomize position for Pickup and ensure it's not a conflict
            Vector3 randomPickupPos = GetRandomPosition();
            randomPickupPos.y = ObjectHeight;  // Set the height to 0.5
            GameObject PickupClone = Instantiate(PickupOriginal, randomPickupPos, PickupOriginal.transform.rotation);
            PickupClone.transform.parent = PickupContainer.transform;
            PickupClone.name = "Pickup" + (i + 1);

            // Randomize position for DontPickup and ensure it's not a conflict
            Vector3 randomDontPickupPos = GetRandomPosition();
            randomDontPickupPos.y = ObjectHeight;  // Set the height to 0.5
            GameObject DontPickupClone = Instantiate(DontPickup, randomDontPickupPos, DontPickup.transform.rotation);
            DontPickupClone.transform.parent = DontPickupContainer.transform;
            DontPickupClone.name = "DontPickup" + (i + 1);
        }
    }

    // Function to get a random position and check for conflicts with existing objects
    Vector3 GetRandomPosition()
    {
        Vector3 randomPos;
        bool validPosition = false;

        // Try to find a position that's not used
        do
        {
            randomPos = new Vector3(Random.Range(-18f, 18f), 0f, Random.Range(-18f, 18f));  // Random position range
            validPosition = !IsPositionUsed(randomPos);  // Check if the position is already used
        } while (!validPosition);

        usedPositions.Add(randomPos); // Add the valid position to the list of used positions
        return randomPos;
    }

    // Function to check if the random position is already used
    bool IsPositionUsed(Vector3 position)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(usedPosition, position) < 1f)  // If the distance between positions is less than 1 unit, it's considered used
            {
                return true;
            }
        }
        return false;
    }

    // Function to restart the game by reloading the current scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Function to exit the game
    public void ExitGame()
    {
        Application.Quit();
    }
}
