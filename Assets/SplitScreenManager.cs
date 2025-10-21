using System.Collections.Generic;
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

    // ------- Ghost (shared) -------
    [Header("Ghosts")]
    [Tooltip("Add your 4 ghost Transforms here.")]
    public List<Transform> ghosts = new List<Transform>();
    [SerializeField] PhysicsMaterial2D ghostMat;       // Pac-mode material for ghosts
    [SerializeField] PhysicsMaterial2D ghostBallMat;   // Ball-mode material for ghosts

    // ---- internals (player) ----
    private Vector2 _A, _B, _P;            // screen-space endpoints and ball
    private int _prevSide = 0;             // +1, -1, 0=unknown or in deadzone
    private float _nextReacquireAt;
    private bool _armed = true;            // flip-flop guard
    private float _nextArmTime = 0f;       // time-based guard
    private float _lastAbsDist = Mathf.Infinity; // for logs
    public bool ghostBall;
    // ---- per-ghost internals ----
    private class GhostState
    {
        public int prevSide = 0;
        public bool armed = true;
        public float nextArmTime = 0f;
        public float lastAbsDist = Mathf.Infinity;
    }
    private List<GhostState> _ghostStates = new List<GhostState>();

    void EnsureGhostStateSize()
    {
        // keep states list aligned with ghosts list
        while (_ghostStates.Count < ghosts.Count) _ghostStates.Add(new GhostState());
        while (_ghostStates.Count > ghosts.Count) _ghostStates.RemoveAt(_ghostStates.Count - 1);
    }

    void OnValidate()
    {
        if (gameplayCamera == null) gameplayCamera = Camera.main;
    }

    void Update()
    {
        TryAcquireBall();
        if (!gameplayCamera || !uiLine) return;

        // endpoints of UI line once per frame
        GetUILineScreenEndpoints(uiLine, out _A, out _B);
        Vector2 ab = _B - _A;
        float abLen = Mathf.Max(ab.magnitude, 0.0001f);

        // ---------- PLAYER (ball) ----------
        if (ball)
        {
            _P = gameplayCamera.WorldToScreenPoint(ball.position);
            float signedDist = Cross(ab, _P - _A) / abLen;
            float absDist = Mathf.Abs(signedDist);
            int side = absDist <= deadZonePixels ? 0 : (signedDist > 0f ? 1 : -1);

            if (_armed && _prevSide != 0 && side != 0 && side != _prevSide)
            {
                string dir = (_prevSide == 1 && side == -1) ? "A→B" : "B→A";
                Debug.Log($"[SplitLineCrossDebug:PLAYER] CROSS {dir}   dist={signedDist:F2}px  p={_P}");

                if (dir == "A→B")
                    SwitchPacManBall(enteringSide: -1, pacmanMode: false); // bottom => Ball
                else
                    SwitchPacManBall(enteringSide: +1, pacmanMode: true);  // top => Pac

                _armed = false;
                _nextArmTime = Time.time + Mathf.Max(0f, cooldown);
            }

            if (!_armed && Time.time >= _nextArmTime && absDist >= rearmPixels)
                _armed = true;

            if (side != 0) _prevSide = side;

            if (logPerFrame && Mathf.Abs(absDist - _lastAbsDist) > 0.05f)
            {
                Debug.Log($"[SplitLineCrossDebug:PLAYER] side={side} |dist|={absDist:F2}px armed={_armed}");
                _lastAbsDist = absDist;
            }
        }

        // ---------- GHOSTS (independent per-ghost) ----------
        EnsureGhostStateSize();

        for (int i = 0; i < ghosts.Count; i++)
        {
            var g = ghosts[i];
            if (!g) continue;
            var gs = _ghostStates[i];

            Vector2 Pg = gameplayCamera.WorldToScreenPoint(g.position);
            float signedDistG = Cross(ab, Pg - _A) / abLen;
            float absDistG = Mathf.Abs(signedDistG);
            int sideG = absDistG <= deadZonePixels ? 0 : (signedDistG > 0f ? 1 : -1);

            if (gs.armed && gs.prevSide != 0 && sideG != 0 && sideG != gs.prevSide)
            {
                string dirG = (gs.prevSide == 1 && sideG == -1) ? "A→B" : "B→A";
                Debug.Log($"[SplitLineCrossDebug:GHOST{i}] CROSS {dirG}  dist={signedDistG:F2}px  p={Pg}");

                bool toPac = (dirG != "A→B"); // B→A = top => Pac-mode; A→B = bottom => Ball-mode
                SwitchGhostPacBall(g, toPac);

                gs.armed = false;
                gs.nextArmTime = Time.time + Mathf.Max(0f, cooldown);
            }

            if (!gs.armed && Time.time >= gs.nextArmTime && absDistG >= rearmPixels)
                gs.armed = true;

            if (sideG != 0) gs.prevSide = sideG;

            if (logPerFrame && Mathf.Abs(absDistG - gs.lastAbsDist) > 0.05f)
            {
                Debug.Log($"[SplitLineCrossDebug:GHOST{i}] side={sideG} |dist|={absDistG:F2}px armed={gs.armed}");
                gs.lastAbsDist = absDistG;
            }
        }
    }

    void LateUpdate()
    {
        if (!drawGizmos || !gameplayCamera) return;

        // choose any valid anchor for z
        Transform anchor = ball;
        if (!anchor)
        {
            for (int i = 0; i < ghosts.Count; i++) { if (ghosts[i]) { anchor = ghosts[i]; break; } }
            if (!anchor) return;
        }

        float zScreen = gameplayCamera.WorldToScreenPoint(anchor.position).z;
        Vector3 wA = gameplayCamera.ScreenToWorldPoint(new Vector3(_A.x, _A.y, zScreen));
        Vector3 wB = gameplayCamera.ScreenToWorldPoint(new Vector3(_B.x, _B.y, zScreen));
        Debug.DrawLine(wA, wB, Color.black); // UI line in world

        if (ball)
        {
            Vector3 wP = ball.position;
            Debug.DrawLine(wP, ClosestPointOnSegment(wA, wB, wP), Color.magenta);
        }

        for (int i = 0; i < ghosts.Count; i++)
        {
            var g = ghosts[i];
            if (!g) continue;
            Vector3 wG = g.position;
            Debug.DrawLine(wG, ClosestPointOnSegment(wA, wB, wG), Color.cyan);
        }
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

    // ---------------- switching (PLAYER) ----------------
    // pacmanMode = true  -> enable Pacman controls (top)
    // pacmanMode = false -> enable Ball physics (bottom)
    public void SwitchPacManBall(int enteringSide, bool pacmanMode)
    {
        if (crossEffect) crossEffect.Play();

        var rb = ball ? ball.GetComponent<Rigidbody2D>() : null;
        var col = ball ? ball.GetComponent<CircleCollider2D>() : null;
        var move = ball ? ball.GetComponent<Movement>() : null;
        var pac = ball ? ball.GetComponent<Pacman>() : null;
        var pin = ball ? ball.GetComponent<Ball>() : null;

        if (pacmanMode)
        {
            // Ball -> Pacman
            if (pin) pin.enabled = false;
            if (pac) pac.enabled = true;
            if (move) move.enabled = true;

            if (ball) ball.gameObject.layer = 6;
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

            if (move && forceUpOnBallToPacman)
                move.SetDirectionOnChange(Vector2.up);

            isBall = false;
        }
        else
        {
            // Pacman -> Ball
            if (pac) pac.enabled = false;
            if (move) move.enabled = false;
            if (pin) pin.enabled = true;

            if (ball) ball.gameObject.layer = 7;
            if (col)
            {
                col.radius = .8f;
                col.sharedMaterial = ballMat;
            }
            if (rb) rb.gravityScale = 1.5f;

            isBall = true;
        }
    }

    // ---------------- switching (GHOST) ----------------
    // toPacmanMode = true  -> enable Ghost Pac/AI mode (top)
    // toPacmanMode = false -> enable Ball physics for the ghost (bottom)
    public void SwitchGhostPacBall(Transform g, bool toPacmanMode)
    {
        if (ghostBall)
        {
            if (!g) return;
            if (crossEffect) crossEffect.Play();

            var rb = g.GetComponent<Rigidbody2D>();
            var colC = g.GetComponent<CircleCollider2D>();
            var col = (Collider2D)colC ?? g.GetComponent<Collider2D>();
            var move = g.GetComponent<Movement>();   // optional
            var ghostAI = g.GetComponent<Ghost>();   // rename to your ghost AI script if different
            var ghostCH = g.GetComponent<GhostChase>();
            var ghostSC = g.GetComponent<GhostScatter>();
            var ghostH = g.GetComponent<GhostHome>();
            var pin = g.GetComponent<Ball>();       // pinball behaviour

            if (toPacmanMode)
            {
                // Ball -> Ghost (Pac) mode
                if (pin) pin.enabled = false;
                if (ghostAI) ghostAI.enabled = true;
                if (ghostCH) ghostCH.enabled = false;
                if (ghostSC) ghostSC.enabled = true;
                if (ghostH) ghostH.enabled = false;
                if (move) move.enabled = true;

                g.gameObject.layer = 12;

                if (colC) colC.radius = 0.5f;
                if (col) col.sharedMaterial = ghostMat;

                if (rb)
                {
                    rb.gravityScale = 0f;
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
                move.SetDirectionOnChange(Vector2.up);
            }
            else
            {

                if (!ghostH.enabled)
                {
                    // Ghost (Pac) -> Ball mode
                    if (pin) pin.enabled = true;
                    if (ghostAI) ghostAI.enabled = false;
                    if (ghostCH) ghostCH.enabled = false;
                    if (ghostSC) ghostSC.enabled = false;
                    if (ghostH) ghostH.enabled = false;
                    if (move) move.enabled = false;

                    g.gameObject.layer = 7;

                    if (colC) colC.radius = 0.8f;
                    if (col) col.sharedMaterial = ghostBallMat;

                    if (rb) rb.gravityScale = 1.5f;
                }
            }
        }
    }
}
