using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public TextMeshProUGUI timeText;

    private int count;
    private int pickupsCollected;
    private int totalPickups = 20;

    private float movementX;
    private float movementY;

    public float speed = 5f;  // Initial speed; you can change this directly from Unity Inspector
    private float lastValidSpeed;  // To store the last valid speed value (before the speed increase)
    private float timeRemaining = 90f;
    private bool isGameOver = false;

    public GameObject restartButton;  // Reference to Restart button
    public GameObject exitButton;     // Reference to Exit button

    private AudioSource playerAudioSource;
    public AudioClip pickUpSound;
    public AudioClip dontPickUpSound;

    private AudioSource mainCameraAudioSource;

    private bool speedIncreased = false;  // To track if speed was increased

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        pickupsCollected = 0;

        lastValidSpeed = speed;  // Set the last valid speed to initial speed
        SetCountText();
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        restartButton.SetActive(false);
        exitButton.SetActive(false);

        StartCoroutine(TimerCountdown());

        playerAudioSource = GetComponent<AudioSource>();
        if (playerAudioSource == null)
            playerAudioSource = gameObject.AddComponent<AudioSource>();

        mainCameraAudioSource = Camera.main.GetComponent<AudioSource>();
        if (mainCameraAudioSource == null)
            mainCameraAudioSource = Camera.main.gameObject.AddComponent<AudioSource>();
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

        if (pickupsCollected == totalPickups && !isGameOver)
        {
            isGameOver = true;
            winTextObject.SetActive(count >= 15);
            loseTextObject.SetActive(count < 15);

            restartButton.SetActive(true);
            exitButton.SetActive(true);

            // Deactivate the player GameObject to make it disappear
            gameObject.SetActive(false);

            StopMainCameraAudio();
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
        if (isGameOver) return;

        if (other.gameObject.CompareTag("PickUp"))
        {
            PlaySound(pickUpSound);
            other.gameObject.SetActive(false);
            count++;
            pickupsCollected++;

            // Check if the player has collected 2 correct pickups
            if (pickupsCollected % 2 == 0)
            {
                // If speed was not already increased, increase it
                if (!speedIncreased)
                {
                    lastValidSpeed = speed;  // Save the current speed as the last valid speed
                    IncreasePlayerSpeed();
                    speedIncreased = true;  // Mark that speed has been increased
                }
            }

            SetCountText();
        }
        else if (other.gameObject.CompareTag("DontPickUp"))
        {
            PlaySound(dontPickUpSound);
            other.gameObject.SetActive(false);
            count--;
            pickupsCollected++;

            // Reset speed to the last valid speed if player collects a wrong pickup after speed increase
            if (speedIncreased)
            {
                ResetPlayerSpeedToLastValid();
                speedIncreased = false;  // Reset the flag
            }

            SetCountText();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            playerAudioSource.PlayOneShot(clip);
    }

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
            isGameOver = true;
            loseTextObject.SetActive(true);
            restartButton.SetActive(true);
            exitButton.SetActive(true);
            gameObject.SetActive(false);
            StopMainCameraAudio();
        }
    }

    private void StopMainCameraAudio()
    {
        if (mainCameraAudioSource != null)
            mainCameraAudioSource.Stop();
    }

    // Function to increase player speed
    private void IncreasePlayerSpeed()
    {
        speed += 2f;  // Increase speed by 2; adjust this value as needed
        Debug.Log("Player Speed Increased! Current Speed: " + speed);
    }

    // Function to reset player speed to last valid speed
    private void ResetPlayerSpeedToLastValid()
    {
        speed = lastValidSpeed;  // Revert speed to the last valid speed
        Debug.Log("Player Speed Reset to Last Valid Speed! Current Speed: " + speed);
    }

    public void RestartGame()
    {
        Debug.Log("Restart button clicked"); // For testing
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked"); // For testing
        Application.Quit();
    }
}
