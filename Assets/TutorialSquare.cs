using System.Collections;
using UnityEngine;

public enum TutorialInputType
{
    Down,
    Right
}

public class TutorialSquare : MonoBehaviour
{
    public int square;

    [Header("UI Settings")]
    public CanvasGroup tutorialUI;
    public Transform pulseTarget;
    public float fadeDuration = 0.3f;

    [Header("Pulse Settings")]
    public float pulseScaleMin = 1.7f;
    public float pulseScaleMax = 1.9f;
    public float pulseSpeed = 2f;

    [Header("Input Settings")]
    public TutorialInputType inputType;
    public bool primaryAudioSquare;

    private Coroutine pulseRoutine;
    public bool triggered = false;
    private bool interruptRequested = false;
    private bool waitingForResume = false;

    // 🔥 NEW: ensures audio plays only once
    private bool audioPlayed = false;

    [SerializeField] GameObject tutorial1Thing;
    [SerializeField] GameObject tutorial2Thing;

    [SerializeField] Transform lockedCamTransform;

    private AudioSource audioSrc;

    public bool gotPellets;

    private void Start()
    {
        tutorialUI.alpha = 0;
        pulseTarget.localScale = Vector3.one;
        audioSrc = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered || collision.gameObject.tag != "Pinball Ball") return;
        triggered = true;

        if (square == 0)
        {
            StartCoroutine(SlowDownTime());
        }

        if (square == 1)
        {
            Movement g = GameObject.Find("Ghost_Blinky").GetComponent<Movement>();
            StartCoroutine(SpeedUpGhost(g, 8.4f, 1f));
        }

        if (square == 2)
        {
            Movement g = GameObject.Find("Ghost_Blinky").GetComponent<Movement>();
            StartCoroutine(SpeedUpGhost(g, 7f, 1f));
            StartCoroutine(SlowDownTime());
        }

        if (square == 3 && GameObject.Find("GameManager").GetComponent<GameManager>().HasRemainingPellets())
        {
            tutorial1Thing.SetActive(true);
            tutorial2Thing.SetActive(false);
            GameObject.Find("CineCamera").GetComponent<PacCameraFollow>().MoveToPoint(lockedCamTransform.position);
        }

        if (square == 5)
        {
            Movement g = GameObject.Find("Ghost_Blinky").GetComponent<Movement>();
            StartCoroutine(SpeedUpGhost(g, 8.4f, 1f));
        }
    }

    private void Update()
    {
        if (!triggered) return;

        if (CheckInput())
        {
            HandleInterrupt();
        }
    }

    // -------------------------
    // Input detection
    // -------------------------
    private bool CheckInput()
    {
        switch (inputType)
        {
            case TutorialInputType.Down:
                return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

            case TutorialInputType.Right:
                return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        }

        return false;
    }

    // -------------------------
    // Handles speeding up time and playing audio
    // -------------------------
    private void HandleInterrupt()
    {
        if (interruptRequested) return;

        interruptRequested = true;

        // 🔥 Audio plays ONLY ONCE for this square
        if (!audioPlayed && primaryAudioSquare && audioSrc != null)
        {
            audioSrc.Play();
            audioPlayed = true;
        }

        StartCoroutine(SpeedUpTime());
    }

    // -------------------------
    // Slow down + fade UI in
    // -------------------------
    private IEnumerator SlowDownTime()
    {
        float target = 0f;
        float duration = 1f;
        float start = Time.timeScale;
        float t = 0;

        StartCoroutine(FadeCanvasGroup(tutorialUI, 0f, 1f, fadeDuration));

        pulseRoutine = StartCoroutine(PulseRoutine());

        while (t < 1)
        {
            if (interruptRequested)
                yield break;

            t += Time.unscaledDeltaTime / duration;
            Time.timeScale = Mathf.Lerp(start, target, t);
            yield return null;
        }

        waitingForResume = true;
    }

    // -------------------------
    // Speed up + fade UI out
    // -------------------------
    private IEnumerator SpeedUpTime()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        yield return StartCoroutine(FadeCanvasGroup(tutorialUI, tutorialUI.alpha, 0f, fadeDuration));

        yield return StartCoroutine(SmoothResetScale());

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

    // -------------------------
    // Smooth Reset Scale
    // -------------------------
    private IEnumerator SmoothResetScale()
    {
        Vector3 start = pulseTarget.localScale;
        Vector3 end = Vector3.one;
        float duration = 0.2f;
        float t = 0;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            pulseTarget.localScale = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        pulseTarget.localScale = end;
    }

    // -------------------------
    // CanvasGroup fading
    // -------------------------
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

    // -------------------------
    // Pulsing UI animation
    // -------------------------
    private IEnumerator PulseRoutine()
    {
        while (true)
        {
            float t = 0;

            while (t < 1)
            {
                t += Time.unscaledDeltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseScaleMin, pulseScaleMax, Mathf.SmoothStep(0, 1, t));
                pulseTarget.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }

            t = 0;

            while (t < 1)
            {
                t += Time.unscaledDeltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseScaleMax, pulseScaleMin, Mathf.SmoothStep(0, 1, t));
                pulseTarget.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }
        }
    }

    // -------------------------
    // Speed up ghost
    // -------------------------
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
