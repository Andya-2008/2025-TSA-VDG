using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class PacCameraFOV : MonoBehaviour
{
    [Header("Orthographic Zoom Settings")]
    public float defaultSize = 5f;
    public float cinematicSpeed = 2f;
    public bool useUnscaledTime = true;

    private Camera cam;
    private Coroutine zoomRoutine;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;
    }

    // ----------------------------------------------
    // PUBLIC FUNCTIONS YOU CAN CALL FROM ANY SCRIPT
    // ----------------------------------------------

    /// <summary>
    /// Smoothly zoom the camera to a target orthographic size.
    /// Smaller = zoom in, larger = zoom out.
    /// </summary>
    public void ZoomTo(float targetSize)
    {
        StartZoomRoutine(targetSize);
    }

    /// <summary>
    /// Smoothly return to the default orthographic size.
    /// </summary>
    public void ResetZoom()
    {
        StartZoomRoutine(defaultSize);
    }

    // ----------------------------------------------
    // INTERNAL LOGIC
    // ----------------------------------------------

    private void StartZoomRoutine(float targetSize)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomRoutine(targetSize));
    }

    private IEnumerator ZoomRoutine(float targetSize)
    {
        float startSize = cam.orthographicSize;
        float t = 0f;

        while (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += delta * cinematicSpeed;

            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        cam.orthographicSize = targetSize;
    }
}