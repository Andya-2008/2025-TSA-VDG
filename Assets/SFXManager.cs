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

    [SerializeField] List<AudioSource> sfx = new List<AudioSource>();

    [Header("Combo Settings")]
    public float pitchStep = 0.1f;      // how much pitch increases per combo hit
    public float maxPitch = 2.0f;       // highest allowed pitch
    public float comboResetTime = 1.0f; // seconds before combo resets

    private float comboTimer = 0f;
    private int comboCount = 0;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Reset combo if time runs out
        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                comboCount = 0;
            }
        }
    }

    // Normal SFX (unchanged)
    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex < 0 || sfxIndex >= sfx.Count) return;
        sfx[sfxIndex].pitch = 1f;
        sfx[sfxIndex].Play();
    }

    // 🔥 NEW: Combo Sound
    public void PlayComboSFX(int sfxIndex)
    {
        if (sfxIndex < 0 || sfxIndex >= sfx.Count) return;

        comboTimer = comboResetTime;
        comboCount++;

        float newPitch = 1f + pitchStep * comboCount;
        newPitch = Mathf.Clamp(newPitch, 1f, maxPitch);

        sfx[sfxIndex].pitch = newPitch;
        sfx[sfxIndex].Play();
    }

    // Optional: Force combo reset manually
    public void ResetCombo()
    {
        comboCount = 0;
    }
}