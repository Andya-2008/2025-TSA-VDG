using UnityEngine;

public class BallShoot : MonoBehaviour
{
    [SerializeField] Transform ballStartPos;
    [SerializeField] GameObject ball;
    [SerializeField] float shootSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newBall = Instantiate(ball, ballStartPos.position, Quaternion.identity);
            newBall.GetComponent<Rigidbody2D>().AddForce(transform.up * shootSpeed * 100);
        }
    }
}
