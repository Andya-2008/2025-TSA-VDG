using System.Collections;
using UnityEngine;

public class GlitchFlickerController : MonoBehaviour
{
    [Header("Assign Your Glitch Scripts Here")]
    public Behaviour corruptedVramEffect;   // ShaderEffect_CorruptedVram
    public Behaviour bleedingColorsEffect;  // ShaderEffect_BleedingColors

    [Header("Flicker Settings")]
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.15f;

    private Coroutine glitchRoutine;

    private void Start()
    {
        // Make sure effects start OFF
        if (corruptedVramEffect != null) corruptedVramEffect.enabled = false;
        if (bleedingColorsEffect != null) bleedingColorsEffect.enabled = false;
    }

    // -------------------------------
    // PUBLIC CALL FROM ANYWHERE:
    // CallGlitch( timeInSeconds );
    // -------------------------------
    public void CallGlitch(float flickerTime)
    {
        if (glitchRoutine != null)
            StopCoroutine(glitchRoutine);

        glitchRoutine = StartCoroutine(FlickerGlitches(flickerTime));
    }

    // The flickering behaviour
    private IEnumerator FlickerGlitches(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Toggle glitch ON
            SetGlitchState(true);
            yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));

            // Toggle glitch OFF
            SetGlitchState(false);
            yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));

            timer += Random.Range(minFlickerInterval, maxFlickerInterval) * 2f;
        }

        // Make sure glitches end OFF
        SetGlitchState(false);
        glitchRoutine = null;
    }

    private void SetGlitchState(bool state)
    {
        if (corruptedVramEffect != null) corruptedVramEffect.enabled = state;
        if (bleedingColorsEffect != null) bleedingColorsEffect.enabled = state;
    }
}