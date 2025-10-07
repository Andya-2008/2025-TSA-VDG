using UnityEngine;

public class Flipper : MonoBehaviour
{
    [SerializeField] HingeJoint2D flipperJoint;
    [SerializeField] bool isLeft;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A) && isLeft)
        {
            flipperJoint.GetComponent<Rigidbody2D>().AddTorque(1500 * Time.deltaTime * 100);
        }

        if (Input.GetKey(KeyCode.D) && !isLeft)
        {
            flipperJoint.GetComponent<Rigidbody2D>().AddTorque(-1500 * Time.deltaTime * 100);
        }
    }
}
