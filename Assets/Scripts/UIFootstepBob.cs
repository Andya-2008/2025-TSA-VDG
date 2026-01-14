using UnityEngine;

public class UIFootstepBob : MonoBehaviour
{
    public float bobAmplitude = 8f;
    public float bobFrequency = 6f;
    public float rollAmplitude = 0.4f;

    private RectTransform rect;
    private Vector2 startPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        float roll = Mathf.Sin(Time.time * bobFrequency) * rollAmplitude;

        rect.anchoredPosition = startPos + Vector2.up * bob;
        rect.localRotation = Quaternion.Euler(0, 0, roll);
    }
}
