using UnityEngine;

public class ArcadeDoorknob : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            IntroSceneManager manager = FindFirstObjectByType<IntroSceneManager>();
            if (manager != null)
            {
                manager.SetTouchingDoor(true);
            }
        }
    }
}
