using UnityEngine;

public class Flipper : MonoBehaviour
{
    [SerializeField] private HingeJoint2D joint;
    [SerializeField] private bool isLeft;
    [SerializeField] public bool canFlip = true;

    [Header("Motor Settings")]
    public float motorSpeed = 800f;
    public float motorTorque = 1500f;

    void Update()
    {
        if (!canFlip) return;

        if (isLeft && Input.GetKeyDown(KeyCode.A))
            Flip();

        if (!isLeft && Input.GetKeyDown(KeyCode.D))
            Flip();

        joint = GetComponent<HingeJoint2D>();
    }

    public void Flip()
    {
        joint.GetComponent<Rigidbody2D>().WakeUp();

        JointMotor2D motor = joint.motor;
        motor.maxMotorTorque = motorTorque;
        motor.motorSpeed = isLeft ? motorSpeed : -motorSpeed;

        joint.motor = motor;
        joint.useMotor = true;
    }

    public void Release()
    {
        joint.useMotor = false;
    }
}
