using UnityEngine;
using UnityEngine.UI;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Refs")]
    public Camera gameplayCamera;         // camera rendering the ball
    public RectTransform uiLine;          // the UI line in your Canvas
    public Transform ball;                // assign OR auto-find by tag (below)

    [Header("Auto-Find (optional)")]
    public bool autoFindBall = true;
    public string ballTag = "Pinball Ball";
    public float reacquireEvery = 0.5f;

    [Header("Debug Options")]
    [Range(0.5f, 8f)] public float deadZonePixels = 1.5f; // jitter guard
    public bool drawGizmos = true;
    public bool logPerFrame = false;      // spammy; off by default

    // internals
    private int _prevSide = 0;            // +1, -1, 0 = unknown/inside deadzone
    private Vector2 _A, _B, _P;           // screen-space endpoints + ball
    private float _nextReacquireAt;
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

        // 2) Ball in screen space
        _P = gameplayCamera.WorldToScreenPoint(ball.position);

        // 3) Signed distance (pixels) to the infinite line
        Vector2 ab = _B - _A;
        float signedDist = Cross(ab, _P - _A) / Mathf.Max(ab.magnitude, 0.0001f);

        // 4) Side with a dead zone
        int side = Mathf.Abs(signedDist) <= deadZonePixels ? 0 : (signedDist > 0f ? 1 : -1);

        // 5) Crossing detection
        if (_prevSide != 0 && side != 0 && side != _prevSide)
        {
            string dir = (_prevSide == 1 && side == -1) ? "A?B" : "B?A";
            Debug.Log($"[SplitLineCrossDebug] CROSS {dir} at dist={signedDist:F2}px  ball={_P}  lineA={_A}  lineB={_B}");

            //ball.GetComponent<Ball>().crossEffect.Play();
            if (dir == "A?B")
            {
                SwitchPacManBall(1);
            }
            else
            {
                SwitchPacManBall(0);
            }
        }

        if (side != 0) _prevSide = side;
        if (logPerFrame) Debug.Log($"[SplitLineCrossDebug] side={side}  dist(px)={signedDist:F2}");
    }

    void LateUpdate()
    {
        if (!drawGizmos || !gameplayCamera || !ball) return;

        float zBallScreen = gameplayCamera.WorldToScreenPoint(ball.position).z;
        Vector3 wA = gameplayCamera.ScreenToWorldPoint(new Vector3(_A.x, _A.y, zBallScreen));
        Vector3 wB = gameplayCamera.ScreenToWorldPoint(new Vector3(_B.x, _B.y, zBallScreen));
        Vector3 wP = ball.position;

        Debug.DrawLine(wA, wB, Color.black);                          // the UI line in world
        Debug.DrawLine(wP, ClosestPointOnSegment(wA, wB, wP), Color.magenta); // perpendicular
    }

    // ---------- helpers ----------
    void TryAcquireBall()
    {
        if (!autoFindBall) return;
        if (ball && ball.gameObject.activeInHierarchy) return;
        if (Time.unscaledTime < _nextReacquireAt) return;
        _nextReacquireAt = Time.unscaledTime + reacquireEvery;

        var go = GameObject.FindWithTag(ballTag);
        if (go) { ball = go.transform; _prevSide = 0; }
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

   public void SwitchPacManBall(int num)
    {
        //Pacman switches into ball
        if(num == 1)
        {
            ball.GetComponent<Pacman>().enabled = false;
            ball.GetComponent<Movement>().enabled = false;
            ball.GetComponent<Ball>().enabled = true;
            ball.gameObject.layer = 7;
            ball.GetComponent<CircleCollider2D>().radius = .8f;
        }   
        //Ball switches to pacman
        else
        {
            ball.GetComponent<Pacman>().enabled = true;
            ball.GetComponent<Movement>().enabled = true;
            ball.GetComponent<Ball>().enabled = false;
            ball.gameObject.layer = 6;
            ball.GetComponent<CircleCollider2D>().radius = .4f;
        }
    }
}