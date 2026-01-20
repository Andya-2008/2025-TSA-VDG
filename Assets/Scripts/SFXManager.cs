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
    public float pitchStep = 0.1f;
    public float maxPitch = 2.0f;
    public float comboResetTime = 1.0f;

    private float comboTimer = 0f;
    private int comboCount = 0;

    private bool sfxMuted = false;

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
        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                comboCount = 0;
        }
    }

    /* ---------------------------------------------------------
     * PUBLIC MUTE CONTROL
     * --------------------------------------------------------- */

    public void TurnOffSFX(bool mute)
    {
        sfxMuted = mute;

        if (sfxMuted)
        {
            foreach (AudioSource source in sfx)
            {
                if (source == null) continue;
                source.Stop();
            }

            foreach (var routine in fadeRoutines.Values)
            {
                StopCoroutine(routine);
            }
            fadeRoutines.Clear();
        }
    }

    public bool IsSFXMuted()
    {
        return sfxMuted;
    }

    /* ---------------------------------------------------------
     * NORMAL SFX
     * --------------------------------------------------------- */

    public void PlaySFX(int sfxIndex, float cutOffMs = -1f, float fadeOutMs = 100f)
    {
        if (sfxMuted) return;
        if (!IsValidIndex(sfxIndex)) return;

        AudioSource source = sfx[sfxIndex];

        if (source.isPlaying)
            return;

        ResetFadeIfNeeded(source);
        source.pitch = 1f;
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

    public void PlayComboSFX(int sfxIndex, float cutOffMs = -1f, float fadeOutMs = 80f)
    {
        if (sfxMuted) return;
        if (!IsValidIndex(sfxIndex)) return;

        comboTimer = comboResetTime;
        comboCount++;

        float newPitch = 1f + pitchStep * comboCount;
        newPitch = Mathf.Clamp(newPitch, 1f, maxPitch);

        AudioSource source = sfx[sfxIndex];

        ResetFadeIfNeeded(source);
        source.pitch = newPitch;
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