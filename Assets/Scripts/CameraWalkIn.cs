using UnityEngine;

public class CameraWalkIn : MonoBehaviour
{
    [Header("Forward Motion (Zoom)")]
    public float zoomSpeed = 2.5f;
    public float targetSize = 3.4f;
    public float easePower = 2f; // higher = smoother ease-in

    [Header("Footstep Bob")]
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 6f;
    public float rollAmplitude = 0.35f; // subtle camera tilt

    private Camera cam;
    private float startSize;
    private float startY;
    private bool walking;
    private float walkProgress; // 0 → 1

    void Start()
    {
        cam = GetComponent<Camera>();
        startSize = cam.orthographicSize;
        startY = transform.localPosition.y;
    }

    void Update()
    {
        if (!walking) return;

        // Progress toward target (ease-in)
        walkProgress += Time.deltaTime * zoomSpeed;
        float t = Mathf.Clamp01(walkProgress);
        float easedT = Mathf.Pow(t, easePower);

        // Zoom in (walk forward)
        cam.orthographicSize = Mathf.Lerp(startSize, targetSize, easedT);

        // Footstep bob
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        float roll = Mathf.Sin(Time.time * bobFrequency) * rollAmplitude;

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            startY + bob,
            transform.localPosition.z
        );

        transform.localRotation = Quaternion.Euler(0, 0, roll);

        // Stop cleanly
        if (t >= 1f)
        {
            walking = false;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void StartWalking()
    {
        walking = true;
        walkProgress = 0f;
    }
}