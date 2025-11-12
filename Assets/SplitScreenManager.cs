using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Refs")]
    public Camera gameplayCamera;           // camera rendering the ball
    public RectTransform uiLine;            // the UI line in your Canvas
    public Transform ball;                  // optional: auto-found by tag below

    [Header("Auto-Find (optional)")]
    public bool autoFindBall = true;
    public float reacquireEvery = 0.5f;

    [Header("Debug")]
    [Range(0.5f, 8f)] public float deadZonePixels = 1.5f; // small jitter guard
    public bool drawGizmos = true;
    public bool logPerFrame = false;

    [Header("Flip-flop Guard (Hysteresis)")]
    [Tooltip("After a crossing, require moving at least this many pixels away from the line before another crossing can register.")]
    public float rearmPixels = 12f;
    [Tooltip("Optional minimum time between crossings.")]
    public float cooldown = 0.08f;

    [Header("Behavior")]
    [Tooltip("When switching Ball -> Pacman, force initial direction up.")]
    public bool forceUpOnBallToPacman = true;

    [SerializeField] PhysicsMaterial2D ballMat;
    [SerializeField] PhysicsMaterial2D pacManMat;

    public ParticleSystem crossEffect;

    public bool isBall;

    // ---- internals ----
    private Vector2 _A, _B, _P;            // screen-space endpoints and ball
    private int _prevSide = 0;             // +1, -1, 0=unknown or in deadzone
    private float _nextReacquireAt;
    private bool _armed = true;            // flip-flop guard
    private float _nextArmTime = 0f;       // time-based guard
    private float _lastAbsDist = Mathf.Infinity; // for logs

    void OnValidate()
    {
        if (gameplayCamera == null) gameplayCamera = Camera.main;
    }

    void Update()
    {
        TryAcquireBall();
        if (!ball || !gameplayCamera || !uiLine) return;

        // 1) UI line endpoints in screen space
        GetUILineScreenEndpoints(uiLine, out _A, out _B);
        Vector2 ab = _B - _A;
        float abLen = Mathf.Max(ab.magnitude, 0.0001f);

        // 2) Ball in screen space
        _P = gameplayCamera.WorldToScreenPoint(ball.position);

        // 3) Signed distance (pixels) of the center to the infinite line
        float signedDist = Cross(ab, _P - _A) / abLen;
        float absDist = Mathf.Abs(signedDist);

        // 4) Determine which side we're on (with a tiny dead zone)
        int side = absDist <= deadZonePixels ? 0 : (signedDist > 0f ? 1 : -1);

        // 5) Crossing detection with flip-flop guard
        if (_armed && _prevSide != 0 && side != 0 && side != _prevSide)
        {
            string dir = (_prevSide == 1 && side == -1) ? "A→B" : "B→A";
            Debug.Log($"[SplitLineCrossDebug] CROSS {dir} at dist={signedDist:F2}px  ball={_P}  lineA={_A}  lineB={_B}");

            // Switch modes based on which side we entered:
            // Convention: side = +1 is one side (e.g., top), -1 is the other (bottom).
            if (dir == "A→B")
                SwitchPacManBall(enteringSide: -1, pacmanMode: false); // entering bottom -> Ball mode
            else
                SwitchPacManBall(enteringSide: +1, pacmanMode: true);  // entering top -> Pacman mode

            // Disarm until we move away enough (and cooldown time expires)
            _armed = false;
            _nextArmTime = Time.time + Mathf.Max(0f, cooldown);
        }

        // Re-arm logic: once we’re far enough away from the line (hysteresis)
        if (!_armed)
        {
            if (Time.time >= _nextArmTime && absDist >= rearmPixels)
                _armed = true;
        }

        if (side != 0) _prevSide = side;
        if (logPerFrame && Mathf.Abs(absDist - _lastAbsDist) > 0.05f)
        {
            Debug.Log($"[SplitLineCrossDebug] side={side}  |dist|={absDist:F2}px  armed={_armed}");
            _lastAbsDist = absDist;
        }
    }

    void LateUpdate()
    {
        if (!drawGizmos || !gameplayCamera || !ball) return;

        float zBallScreen = gameplayCamera.WorldToScreenPoint(ball.position).z;
        Vector3 wA = gameplayCamera.ScreenToWorldPoint(new Vector3(_A.x, _A.y, zBallScreen));
        Vector3 wB = gameplayCamera.ScreenToWorldPoint(new Vector3(_B.x, _B.y, zBallScreen));
        Vector3 wP = ball.position;

        Debug.DrawLine(wA, wB, Color.black);                             // UI line in world
        Debug.DrawLine(wP, ClosestPointOnSegment(wA, wB, wP), Color.magenta); // perpendicular to line
    }

    // ---------------- helpers ----------------
    void TryAcquireBall()
    {
        if (!autoFindBall) return;
        if (ball && ball.gameObject.activeInHierarchy) return;
        if (Time.unscaledTime < _nextReacquireAt) return;
        _nextReacquireAt = Time.unscaledTime + reacquireEvery;

        var go = ball;
        if (go)
        {
            ball = go.transform;
            _prevSide = 0;       // reset state on acquire
            _armed = true;
        }
    }

    static void GetUILineScreenEndpoints(RectTransform rt, out Vector2 a, out Vector2 b)
    {
        Rect r = rt.rect;
        bool horizontal = Mathf.Abs(r.width) >= Mathf.Abs(r.height);
        Vector3 center3 = new Vector3(r.center.x, r.center.y, 0f);
        Vector3 halfDir = horizontal ? Vector3.right * (r.width * 0.5f)
                                     : Vector3.up * (r.height * 0.5f);
        Vector3 localA = center3 - halfDir;
        Vector3 localB = center3 + halfDir;

        Vector3 wA = rt.TransformPoint(localA);
        Vector3 wB = rt.TransformPoint(localB);

        Canvas canvas = rt.GetComponentInParent<Canvas>();
        Camera uiCam = canvas ? canvas.worldCamera : null;

        a = RectTransformUtility.WorldToScreenPoint(uiCam, wA);
        b = RectTransformUtility.WorldToScreenPoint(uiCam, wB);
    }

    static float Cross(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;

    static Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(p - a, ab) / Mathf.Max(ab.sqrMagnitude, 1e-6f);
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }

    // ---------------- switching ----------------
    // pacmanMode = true  -> enable Pacman controls (top)
    // pacmanMode = false -> enable Ball physics (bottom)
    public void SwitchPacManBall(int enteringSide, bool pacmanMode)
    {
        crossEffect.Play();
        var rb = ball.GetComponent<Rigidbody2D>();
        var col = ball.GetComponent<CircleCollider2D>();
        var move = ball.GetComponent<Movement>();
        var pac = ball.GetComponent<Pacman>();
        var pin = ball.GetComponent<Ball>();

        if (pacmanMode)
        {
            // Ball -> Pacman
            if (pin) pin.enabled = false;
            if (pac) pac.enabled = true;
            if (move) move.enabled = true;

            ball.gameObject.layer = 6;
            if (col)
            { 
                col.radius = .5f;
                col.sharedMaterial = pacManMat;
            }
            
            if (rb)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;            // remove any downward bounce
                rb.angularVelocity = 0f;
            }
            move.switched = true;
            move.SetDirectionOnChange(Vector2.up);
            isBall = false;
        }
        else
        {
            // Pacman -> Ball
            if (pac) pac.enabled = false;
            if (move) move.enabled = false;
            if (pin) pin.enabled = true;

            ball.gameObject.layer = 7;
            if (col)
            {
                col.radius = .8f;
                col.sharedMaterial = ballMat;
            }
            if (rb) rb.gravityScale = 1.5f;

            // NOTE: Do NOT force direction here (your original requirement)
            isBall = true;
        }
    }
}