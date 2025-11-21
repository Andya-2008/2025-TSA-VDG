using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDistanceFade : MonoBehaviour
{
    public Transform target;        // Player / Camera
    public float maxDistance = 10f; // Distance where audio is silent
    public float minDistance = 0f;  // Distance where audio is full volume

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Auto-assign camera if not set
        if (target == null && Camera.main != null)
            target = Camera.main.transform;
    }

    void Update()
    {
        if (target == null) return;

        // Distance between object and player/camera
        float dist = Vector3.Distance(transform.position, target.position);

        // If closer than minDistance → full volume
        if (dist <= minDistance)
        {
            audioSource.volume = 1f;
            return;
        }

        // If farther than maxDistance → silent
        if (dist >= maxDistance)
        {
            audioSource.volume = 0f;
            return;
        }

        // Otherwise fade linearly between 1 → 0
        float t = (dist - minDistance) / (maxDistance - minDistance);

        audioSource.volume = 1f - t;
    }
}