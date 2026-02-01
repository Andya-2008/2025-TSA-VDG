using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Refs")]
    public Camera gameplayCamera;      // used ONLY for WorldToScreenPoint
    public RectTransform uiLine;
    public Transform ball;

    [Header("Level Control")]
    public int rotatingSplitLevel = 2;

    [Header("Detection")]
    public float deadZonePixels = 1.5f;
    public float rearmPixels = 12f;
    public float cooldown = 0.08f;

    [Header("State")]
    public bool isBall = true;

    // internals
    Vector2 A, B, P;
    int prevSide = 0;
    bool armed = true;
    float nextArmTime = 0f;

    GameManager gm;

    void Awake()
    {
        if (!gameplayCamera) gameplayCamera = Camera.main;
        var gmo = GameObject.Find("GameManager");
        if (gmo) gm = gmo.GetComponent<GameManager>();
    }

    void Update()
    {
        if (!ball || !uiLine || !gameplayCamera) return;

        // LEVEL 1: original behavior (static line, logic only)
        if (gm != null && gm.level == 1)
        {
            RunStaticSplit();
            return;
        }

        // LEVEL 2: rotating split, RT-safe
        if (gm != null && gm.level == rotatingSplitLevel)
        {
            RunRotatingSplit();
        }
    }

    // ---------------- LEVEL 1 ----------------
    void RunStaticSplit()
    {
        GetUILineScreenEndpoints(uiLine, out A, out B);
        EvaluateCrossing(A, B);
    }

    // ---------------- LEVEL 2 ----------------
    void RunRotatingSplit()
    {
        // identical math, but assumes line is rotating
        GetUILineScreenEndpoints(uiLine, out A, out B);
        EvaluateCrossing(A, B);
    }

    // ---------------- CORE LOGIC ----------------
    void EvaluateCrossing(Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float abLen = Mathf.Max(ab.magnitude, 0.0001f);

        P = gameplayCamera.WorldToScreenPoint(ball.position);

        float signedDist = Cross(ab, P - a) / abLen;
        float absDist = Mathf.Abs(signedDist);

        int side = absDist <= deadZonePixels ? 0 : (signedDist > 0 ? 1 : -1);

        if (armed && prevSide != 0 && side != 0 && side != prevSide)
        {
            bool enteringPacman = side > 0;
            SwitchPacManBall(enteringPacman);

            armed = false;
            nextArmTime = Time.time + cooldown;
        }

        if (!armed && Time.time >= nextArmTime && absDist >= rearmPixels)
            armed = true;

        if (side != 0)
            prevSide = side;
    }

    // ---------------- HELPERS ----------------
    static float Cross(Vector2 u, Vector2 v)
    {
        return u.x * v.y - u.y * v.x;
    }

    static void GetUILineScreenEndpoints(RectTransform rt, out Vector2 a, out Vector2 b)
    {
        Rect r = rt.rect;
        bool horizontal = r.width >= r.height;

        Vector3 center = r.center;
        Vector3 half = horizontal
            ? Vector3.right * r.width * 0.5f
            : Vector3.up * r.height * 0.5f;

        Vector3 wA = rt.TransformPoint(center - half);
        Vector3 wB = rt.TransformPoint(center + half);

        Canvas canvas = rt.GetComponentInParent<Canvas>();
        Camera uiCam = canvas ? canvas.worldCamera : null;

        a = RectTransformUtility.WorldToScreenPoint(uiCam, wA);
        b = RectTransformUtility.WorldToScreenPoint(uiCam, wB);
    }

    // ---------------- MODE SWITCH ----------------
    public void SwitchPacManBall(bool pacmanMode)
    {
        var rb = ball.GetComponent<Rigidbody2D>();
        var pac = ball.GetComponent<Pacman>();
        var pin = ball.GetComponent<Ball>();
        var move = ball.GetComponent<Movement>();

        if (pacmanMode)
        {
            if (pin) pin.enabled = false;
            if (pac) pac.enabled = true;
            if (move) move.enabled = true;

            if (rb)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            isBall = false;
        }
        else
        {
            if (pac) pac.enabled = false;
            if (move) move.enabled = false;
            if (pin) pin.enabled = true;

            if (rb)
                rb.gravityScale = 1.5f;

            isBall = true;
        }
    }
}
