using UnityEngine;

public class IntroSceneCameraFollow : MonoBehaviour
{
    // Camera follow script for intro scene --- copilot: write a basic camera follow script that follows the player object

    [SerializeField] private Transform player;
    [SerializeField] private Vector2 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Follow the player position with an offset
        if (player != null)
        {
            Vector2 newPos = new Vector2(player.position.x + offset.x, player.position.y + offset.y);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
    }
}
