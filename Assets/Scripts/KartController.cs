using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;

public class KartController : MonoBehaviour
{
    [SerializeField] private float acceleration, gravity, steering;
    [SerializeField] Rigidbody sphere;
    [SerializeField] Animator spriteVisual;

    private float speed, currentSpeed;
    private float rotate, currentRotation;
    public float Speed = 0;
    private bool canMove = true;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(sphere.transform.position.x,
            sphere.transform.position.y - 0.5f,
            sphere.transform.position.z);

        Speed = (int)Math.Floor(sphere.linearVelocity.magnitude);

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && canMove)
        {
            speed = acceleration;
        }

        if (Input.GetAxis("Horizontal") != 0 && canMove)
        {
            int dir = Input.GetAxis("Horizontal") > 0f ? 1 : -1;
            float amount = Mathf.Abs(Input.GetAxis("Horizontal"));
            Steer(dir, amount);
        }

        if (canMove) spriteVisual.SetFloat("Horizontal", Input.GetAxis("Horizontal"));

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, 12f * Time.deltaTime);
        speed = 0f;
        currentRotation = Mathf.Lerp(currentRotation, rotate, 4f * Time.deltaTime);
        rotate = 0f;
    }

    void FixedUpdate()
    {
        sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
            new Vector3(0f, transform.eulerAngles.y + currentRotation, 0f), 5f * Time.deltaTime);
    }

    void Steer(int dir, float amount)
    {
        rotate = (steering * dir) * amount;
    }

    public void RaceFinished()
    {
        canMove = false;
        speed = 0;
    }
}
