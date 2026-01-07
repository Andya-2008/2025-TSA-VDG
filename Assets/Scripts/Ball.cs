using UnityEngine;
using UnityEngine.Playables;

public class Ball : MonoBehaviour
{
    [SerializeField] float maxSpeed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector3.ClampMagnitude(GetComponent<Rigidbody2D>().linearVelocity, maxSpeed);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with " + collision.gameObject.tag);
        if(collision.gameObject.tag.Equals("Bouncy"))
        {
            Debug.Log("Bounce");
            collision.gameObject.GetComponent<PlayableDirector>().Play();
        }
    }
}
    