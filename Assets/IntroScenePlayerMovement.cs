using UnityEngine;

public class IntroScenePlayerMovement : MonoBehaviour
{
    private IntroSceneManager gameManager;
    private float speed = 5f;

    void Start()
    {
        gameManager = FindFirstObjectByType<IntroSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        transform.position += new Vector3(moveX * speed, 0, 0) * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("ArcadeDoorknob"))
        {
            if (gameManager != null)
            {
                gameManager.SetTouchingDoor(true);
            }
        }
    }
}
