using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float maxSpeed = 100f;
    public ParticleSystem crossEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector3.ClampMagnitude(GetComponent<Rigidbody2D>().linearVelocity, maxSpeed);
        
    }
}
