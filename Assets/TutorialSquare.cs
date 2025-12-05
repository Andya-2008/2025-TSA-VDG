using System.Collections;
using UnityEngine;

public class TutorialSquare : MonoBehaviour
{
    public int square;

    [Header("UI Settings")]
    public CanvasGroup tutorialUI;        // Assign in Inspector
    public Transform pulseTarget;         // The text or panel that should pulse
    public float fadeDuration = 0.3f;

    // Pulse settings
    public float pulseScaleMin = 1.7f;
    public float pulseScaleMax = 1.9f;
    public float pulseSpeed = 2f;

    private Coroutine pulseRoutine;
    private bool triggered = false;
    private bool waitingForResume = false;

    private bool interruptRequested = false;

    private void Start()
    {
        tutorialUI.alpha = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        triggered = true;

        if (square == 0)
        {
            GameObject.Find("Ghost_Blinky").GetComponent<Ghost>().ActivateInTutorial();
            StartCoroutine(SlowDownTime());
        }

        if (square == 1)
        {
            Movement g = GameObject.Find("Ghost_Blinky").GetComponent<Movement>();
            StartCoroutine(SpeedUpGhost(g, 8.4f, 1f));
        }
    }

    private void Update()
    {
        // If the player presses Down or S at ANY moment:
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!interruptRequested && triggered)
            {
                interruptRequested = true;

                // immediately start speeding up
                StartCoroutine(SpeedUpTime());
                GetComponent<AudioSource>().Play();
            }
        }
    }

    // ------------------------------
    // Slow down & fade UI in
    // ------------------------------
    private IEnumerator SlowDownTime()
    {
        float target = 0.05f;
        float duration = 1f;
        float start = Time.timeScale;
        float t = 0;

        // Fade UI in
        StartCoroutine(FadeCanvasGroup(tutorialUI, 0f, 1f, fadeDuration));

        // Start pulsing animation
        pulseRoutine = StartCoroutine(PulseRoutine());

        while (t < 1)
        {
            // IF USER INTERRUPTS → EXIT IMMEDIATELY
            if (interruptRequested)
                yield break;

            t += Time.unscaledDeltaTime / duration;
            Time.timeScale = Mathf.Lerp(start, target, t);
            yield return null;
        }

        waitingForResume = true;
    }

    // ------------------------------
    // Speed up & fade UI out
    // ------------------------------
    private IEnumerator SpeedUpTime()
    {
        // Stop pulse animation
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        // Fade UI OUT
        StartCoroutine(FadeCanvasGroup(tutorialUI, tutorialUI.alpha, 0f, fadeDuration));

        // Reset scale smoothly
        pulseTarget.localScale = Vector3.one;

        float target = 1f;
        float duration = .1f;
        float start = Time.timeScale;
        float t = 0;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            Time.timeScale = Mathf.Lerp(start, target, t);
            yield return null;
        }

        Time.timeScale = 1f;

        interruptRequested = false;
        waitingForResume = false;
    }

    // ------------------------------
    // CanvasGroup fade
    // ------------------------------
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float t = 0;
        cg.alpha = start;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            cg.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        cg.alpha = end;
    }

    // ------------------------------
    // Text pulsing effect
    // ------------------------------
    private IEnumerator PulseRoutine()
    {
        while (true)
        {
            float t = 0;

            // Scale UP
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseScaleMin, pulseScaleMax, Mathf.SmoothStep(0, 1, t));
                pulseTarget.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }

            t = 0;

            // Scale DOWN
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseScaleMax, pulseScaleMin, Mathf.SmoothStep(0, 1, t));
                pulseTarget.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }
        }
    }

    // ------------------------------
    // Ghost speed-up coroutine
    // ------------------------------
    private IEnumerator SpeedUpGhost(Movement ghost, float targetSpeed, float duration)
    {
        float start = ghost.speed;
        float t = 0;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            ghost.speed = Mathf.Lerp(start, targetSpeed, t);
            yield return null;
        }

        ghost.speed = targetSpeed;
    }
}