using UnityEngine;

public class UIWalkIn : MonoBehaviour
{
    [Header("Zoom (Forward Motion)")]
    public float targetScale = 1.15f;
    public float duration = 1.5f;
    public float easePower = 2.5f;

    private Vector3 startScale;
    private float elapsed;
    private bool walking;

    void Awake()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        if (!walking) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        float eased = Mathf.Pow(t, easePower);

        transform.localScale = Vector3.Lerp(
            startScale,
            startScale * targetScale,
            eased
        );

        if (t >= 1f)
            walking = false;
    }

    public void StartWalking()
    {
        elapsed = 0f;
        walking = true;
    }
}