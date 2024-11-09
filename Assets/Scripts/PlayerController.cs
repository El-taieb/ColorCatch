using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject loseTextObject;  // Reference for the lose text
    public TextMeshProUGUI timeText;   // Reference to display the timer

    private int count; // To track the score
    private int pickupsCollected; // To track the number of pickups collected
    private int totalPickups = 20; // The total number of pickups in the game

    private float movementX;
    private float movementY;

    public float speed = 0;

    private float timeRemaining = 90f; // Set the initial time to 90 seconds (1:30 min)
    private bool isGameOver = false;  // To track if the game is over

    private AudioSource playerAudioSource; // Reference to player's AudioSource
    public AudioClip pickUpSound;  // Sound for pick up
    public AudioClip dontPickUpSound;  // Sound for don't pick up

    private AudioSource mainCameraAudioSource;  // Reference to main camera's AudioSource

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        pickupsCollected = 0;  // Initialize pickups collected to 0

        SetCountText();
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);  // Hide lose text initially

        // Start the timer countdown
        StartCoroutine(TimerCountdown());

        // Get the AudioSource component attached to the player
        playerAudioSource = GetComponent<AudioSource>();

        if (playerAudioSource == null)
        {
            playerAudioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not present
        }

        // Get the AudioSource component attached to the main camera
        mainCameraAudioSource = Camera.main.GetComponent<AudioSource>();

        if (mainCameraAudioSource == null)
        {
            mainCameraAudioSource = Camera.main.gameObject.AddComponent<AudioSource>(); // Add AudioSource if not present on main camera
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        // Check win or lose only if all pickups are collected
        if (pickupsCollected == totalPickups && !isGameOver)
        {
            isGameOver = true; // Stop any further updates
            if (count >= 15)
            {
                winTextObject.SetActive(true);
                loseTextObject.SetActive(false);
            }
            else
            {
                loseTextObject.SetActive(true);
                winTextObject.SetActive(false);
            }
            gameObject.SetActive(false);  // Make player disappear after checking result

            StopMainCameraAudio(); // Stop the main camera audio when game is over
        }
    }

    private void FixedUpdate()
    {
        if (!isGameOver)
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;  // Ignore if the game is over

        if (other.gameObject.CompareTag("PickUp"))
        {
            PlaySound(pickUpSound);  // Play the pick-up sound
            other.gameObject.SetActive(false);  // Disable the PickUp object
            count = count + 1;  // Increase count by 1 for PickUp
            pickupsCollected++;  // Increase the number of pickups collected
            SetCountText();  // Update the count display
        }
        else if (other.gameObject.CompareTag("DontPickUp"))
        {
            PlaySound(dontPickUpSound);  // Play the dont-pick-up sound
            other.gameObject.SetActive(false);  // Disable the DontPickup object
            count = count - 1;  // Decrease count by 1 for DontPickup
            pickupsCollected++;  // Increase the number of pickups collected
            SetCountText();  // Update the count display
        }
    }

    // Function to play sound on the PickUp or DontPickUp objects
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            Debug.Log("Playing sound!");
            playerAudioSource.PlayOneShot(clip);  // Play the assigned audio clip
        }
        else
        {
            Debug.Log("No AudioClip assigned to PlaySound!");
        }
    }

    // Timer countdown function
    private IEnumerator TimerCountdown()
    {
        while (timeRemaining > 0 && !isGameOver)
        {
            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return null;
        }

        if (!isGameOver)
        {
            // Timer has run out, trigger loss
            isGameOver = true;
            loseTextObject.SetActive(true);  // Show lose text
            winTextObject.SetActive(false);  // Hide win text
            gameObject.SetActive(false);  // Make player disappear

            StopMainCameraAudio(); // Stop the main camera audio when game is over
        }
    }

    // Function to stop main camera audio when the game ends
    private void StopMainCameraAudio()
    {
        if (mainCameraAudioSource != null)
        {
            mainCameraAudioSource.Stop();  // Stop the audio playing on the main camera
        }
    }
}