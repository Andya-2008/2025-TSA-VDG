using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LayerFlickerGlitch : MonoBehaviour
{
    [Header("Layer Groups")]
    [Tooltip("Usually your 'normal' single layer")]
    public LayerMask singleLayerMask;

    [Tooltip("Your glitch stack (5 layers, etc.)")]
    public LayerMask multiLayerMask;

    [Header("Flicker Timing")]
    public float minFlickerInterval = 0.04f;
    public float maxFlickerInterval = 0.12f;

    private Camera cam;
    private Coroutine flickerRoutine;
    private LayerMask originalMask;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        originalMask = cam.cullingMask;
    }

    // -------------------------------
    // PUBLIC CALL
    // -------------------------------
    public void CallLayerGlitch(float duration)
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        flickerRoutine = StartCoroutine(FlickerLayers(duration));
    }

    private IEnumerator FlickerLayers(float duration)
    {
        float timer = 0f;
        bool useMulti = false;

        while (timer < duration)
        {
            // Toggle which group is active
            cam.cullingMask = useMulti ? multiLayerMask : singleLayerMask;
            useMulti = !useMulti;

            float wait = Random.Range(minFlickerInterval, maxFlickerInterval);
            timer += wait;
            yield return new WaitForSeconds(wait);
        }

        // Restore original camera layers
        cam.cullingMask = multiLayerMask;
        flickerRoutine = null;
    }
}