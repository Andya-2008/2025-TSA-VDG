using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private KartController kart;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI raceStatusText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private RacingGameController gameController;

    private bool stillRunning = true;
    private float displayedSpeed;
    private float displayedPower;

    private float speedTarget;
    private float powerTarget;

    private float displayedSpeedMultiplier = 2.5f;
    private float fluctuationStrength = 0.8f; // 80% fluctuation
    private float smoothSpeed = 5f; // speed of lerp smoothing

    void Start()
    {
        displayedSpeed = kart.Speed * displayedSpeedMultiplier;
        speedTarget = displayedSpeed;
        speedText.text = displayedSpeed + " mph";

        displayedPower = kart.currentSpeed / kart.acceleration;
        powerTarget = displayedPower;
        powerSlider.value = displayedPower;

        raceStatusText.text = " ";
    }

    void Update()
    {
        // Smooth speed fluctuation
        speedTarget = kart.Speed * displayedSpeedMultiplier * Random.Range(1f - fluctuationStrength, 1f + fluctuationStrength);
        displayedSpeed = Mathf.Lerp(displayedSpeed, speedTarget, Time.deltaTime * smoothSpeed);
        speedText.text = Mathf.FloorToInt(displayedSpeed) + " mph";

        // Smooth power fluctuation
        powerTarget = Mathf.Clamp01((kart.currentSpeed / kart.acceleration) * Random.Range(1f - fluctuationStrength, 1f + fluctuationStrength));
        displayedPower = Mathf.Lerp(displayedPower, powerTarget, Time.deltaTime * smoothSpeed);
        powerSlider.value = displayedPower;

        // Time display
        if (stillRunning)
        {
            float time = Time.timeSinceLevelLoad;
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000);
            timeText.text = string.Format("{0:0}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    public void finishRace()
    {
        raceStatusText.text = "Race Finished!";
        raceStatusText.gameObject.SetActive(true); // ensure it's visible
        stillRunning = false;
        StartCoroutine(fadeLapTextForever());
    }

    private IEnumerator fadeLapTextForever()
    {
        Color originalColor = raceStatusText.color;
        float fadeSpeed = 2f; // how fast it fades in/out

        while (true) // loop forever
        {
            // Alpha oscillates smoothly between 0 and 1
            float alpha = (Mathf.Sin(Time.time * fadeSpeed * Mathf.PI) + 1f) / 2f;
            raceStatusText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null; // wait until next frame
        }
    }

    public void lapDisplay()
    {
        raceStatusText.text = "Laps remaining: " + (3 - gameController.getLapsCompleted());
        raceStatusText.gameObject.SetActive(true); // make sure text is enabled
        StartCoroutine(fadeLapText());
    }

    private IEnumerator fadeLapText()
    {
        float blinkDuration = 3f; // time for the in/out blinking
        float fadeSpeed = 2f;     // how fast it fades in/out
        float fadeOutDuration = 1f; // final fade-out duration

        float elapsed = 0f;
        Color originalColor = raceStatusText.color;

        // Smooth in/out blinking
        while (elapsed < blinkDuration)
        {
            float alpha = (Mathf.Sin(elapsed * fadeSpeed * Mathf.PI) + 1f) / 2f;
            raceStatusText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Smooth fade out at the end
        float fadeElapsed = 0f;
        Color startColor = raceStatusText.color;
        while (fadeElapsed < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, fadeElapsed / fadeOutDuration);
            raceStatusText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure completely invisible
        raceStatusText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        raceStatusText.gameObject.SetActive(false); // optional: hide the object
    }


}
