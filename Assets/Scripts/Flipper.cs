using System.Collections;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HingeJoint2D joint;
    [SerializeField] private bool isLeft;

    [Header("State")]
    public bool canFlip = true;

    [Header("Torque Settings")]
    public float gameplayTorque = 1500f;
    public float tutorialTorque = 1100f;

    [Header("Tutorial Flip")]
    public float tutorialFlipDuration = 0.06f;

    [Header("Stability")]
    public float maxAngularVelocity = 1200f;
    public float angularDamping = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = joint.GetComponent<Rigidbody2D>();

        // Enforce stable defaults
        //rb.gravityScale = 0f;
        rb.angularDamping = angularDamping;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        if (!canFlip) return;

        if (isLeft && Input.GetKey(KeyCode.A))
            ApplyGameplayTorque(+gameplayTorque);

        if (!isLeft && Input.GetKey(KeyCode.D))
            ApplyGameplayTorque(-gameplayTorque);
    }

    // =========================
    // GAMEPLAY FLIP (HELD INPUT)
    // =========================
    private void ApplyGameplayTorque(float torque)
    {
        rb.WakeUp();
        rb.AddTorque(torque * Time.deltaTime * 100f);
    }

    // =========================
    // TUTORIAL FLIP (ONE-SHOT)
    // =========================
    public void ForceFlip()
    {
        StopAllCoroutines();
        StartCoroutine(ForceFlipRoutine());
    }

    private IEnumerator ForceFlipRoutine()
    {
        rb.WakeUp();

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 1f; // allow physics to run

        float elapsed = 0f;

        while (elapsed < tutorialFlipDuration)
        {
            float torque = isLeft ? tutorialTorque : -tutorialTorque;
            rb.AddTorque(torque * Time.deltaTime * 100f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = originalTimeScale;
    }

    // =========================
    // STABILITY CLAMP
    // =========================
    void FixedUpdate()
    {
        if (Mathf.Abs(rb.angularVelocity) > maxAngularVelocity)
        {
            rb.angularVelocity =
                Mathf.Sign(rb.angularVelocity) * maxAngularVelocity;
        }
    }
}