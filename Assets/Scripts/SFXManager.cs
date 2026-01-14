using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private static SFXManager _instance;
    public static SFXManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<SFXManager>();
            return _instance;
        }
    }

    [Header("SFX Sources")]
    [SerializeField] private List<AudioSource> sfx = new List<AudioSource>();

    [Header("Combo Settings")]
    public float pitchStep = 0.1f;      // pitch increase per combo
    public float maxPitch = 2.0f;       // clamp max pitch
    public float comboResetTime = 1.0f; // seconds before combo resets

    private float comboTimer = 0f;
    private int comboCount = 0;

    // Track fade coroutines per AudioSource (prevents conflicts)
    private Dictionary<AudioSource, Coroutine> fadeRoutines =
        new Dictionary<AudioSource, Coroutine>();

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Reset combo if timer expires
        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                comboCount = 0;
        }
    }

    /* ---------------------------------------------------------
     * NORMAL SFX
     * --------------------------------------------------------- */

    /// <summary>
    /// Plays an SFX with optional cutoff + fade.
    /// </summary>
    /// <param name="sfxIndex">Index in the SFX list</param>
    /// <param name="cutOffMs">
    /// Time (ms) before fade begins. Use -1 for no cutoff.
    /// </param>
    /// <param name="fadeOutMs">
    /// Fade duration in ms (only used if cutoff is enabled)
    /// </param>
    public void PlaySFX(int sfxIndex, float cutOffMs = -1f, float fadeOutMs = 100f)
    {
        if (!IsValidIndex(sfxIndex)) return;

        AudioSource source = sfx[sfxIndex];

        // 🔒 Prevent replay if already playing
        if (source.isPlaying)
            return;

        ResetFadeIfNeeded(source);

        source.Play();

        if (cutOffMs > 0)
        {
            fadeRoutines[source] = StartCoroutine(
                FadeOutAndStop(
                    source,
                    cutOffMs / 1000f,
                    fadeOutMs / 1000f
                )
            );
        }
    }

    /* ---------------------------------------------------------
     * COMBO SFX
     * --------------------------------------------------------- */

    /// <summary>
    /// Plays an SFX with combo pitch scaling and optional cutoff.
    /// </summary>
    public void PlayComboSFX(int sfxIndex, float cutOffMs = -1f, float fadeOutMs = 80f)
    {
        if (!IsValidIndex(sfxIndex)) return;

        comboTimer = comboResetTime;
        comboCount++;

        float newPitch = 1f + pitchStep * comboCount;
        newPitch = Mathf.Clamp(newPitch, 1f, maxPitch);

        AudioSource source = sfx[sfxIndex];

        ResetFadeIfNeeded(source);

        source.pitch = newPitch;
        //source.volume = 1f;
        source.Play();

        if (cutOffMs > 0)
        {
            fadeRoutines[source] = StartCoroutine(
                FadeOutAndStop(
                    source,
                    cutOffMs / 1000f,
                    fadeOutMs / 1000f
                )
            );
        }
    }

    /* ---------------------------------------------------------
     * COMBO CONTROL
     * --------------------------------------------------------- */

    public void ResetCombo()
    {
        comboCount = 0;
        comboTimer = 0f;
    }

    /* ---------------------------------------------------------
     * INTERNAL HELPERS
     * --------------------------------------------------------- */

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < sfx.Count && sfx[index] != null;
    }

    private void ResetFadeIfNeeded(AudioSource source)
    {
        if (fadeRoutines.TryGetValue(source, out Coroutine routine))
        {
            StopCoroutine(routine);
            fadeRoutines.Remove(source);
        }
    }

    private IEnumerator FadeOutAndStop(
        AudioSource source,
        float delay,
        float fadeDuration
    )
    {
        yield return new WaitForSeconds(delay);

        float startVolume = source.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
        fadeRoutines.Remove(source);
    }
}
