using UnityEngine;
using System.Collections;

public class PacCameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float followSpeed = 5f;

    [Header("Cinematic Settings")]
    public float cinematicMoveSpeed = 3f;

    private bool followingPlayer = true;
    private Coroutine camRoutine;

    private void Start()
    {
        if (player == null)
            player = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        if (followingPlayer)
        {
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, -10f);
            transform.position = targetPos;
        }
    }

    // ----------------------------------------------
    // PUBLIC FUNCTIONS YOU CAN CALL FROM ANY SCRIPT
    // ----------------------------------------------

    /// <summary>
    /// Move the camera to a fixed world position and stop following the player.
    /// </summary>
    public void MoveToPoint(Vector3 point)
    {
        if (camRoutine != null)
            StopCoroutine(camRoutine);

        followingPlayer = false;
        camRoutine = StartCoroutine(MoveCamera(point));
    }

    /// <summary>
    /// Resume following the player with a cinematic move back.
    /// </summary>
    public void ReturnToPlayer()
    {
        if (camRoutine != null)
            StopCoroutine(camRoutine);

        followingPlayer = false;
        camRoutine = StartCoroutine(MoveCamera(
            new Vector3(player.position.x, player.position.y, -10f),
            onComplete: () => followingPlayer = true
        ));
    }

    // ----------------------------------------------
    // Smooth camera movement coroutine
    // ----------------------------------------------
    private IEnumerator MoveCamera(Vector3 destination, System.Action onComplete = null)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            t += Time.unscaledDeltaTime * cinematicMoveSpeed;
            transform.position = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        transform.position = destination;

        onComplete?.Invoke();
    }
}