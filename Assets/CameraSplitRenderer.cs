using UnityEngine;
using UnityEngine.UI;

public class CameraSplitRenderer : MonoBehaviour
{
    [Header("Inputs")]
    public RawImage output;
    public RectTransform splitLine;

    public RenderTexture ballRT;
    public RenderTexture pacmanRT;

    [Header("Reveal")]
    public float blendWidthPixels = 40f;

    private Material _mat;

    void Awake()
    {
        _mat = output.material;
        _mat.SetTexture("_BallTex", ballRT);
        _mat.SetTexture("_PacmanTex", pacmanRT);
    }

    void Update()
    {
        if (!splitLine) return;

        GetLineScreenEndpoints(splitLine, out Vector2 A, out Vector2 B);

        Vector2 An = new Vector2(A.x / Screen.width, A.y / Screen.height);
        Vector2 Bn = new Vector2(B.x / Screen.width, B.y / Screen.height);

        _mat.SetVector("_LineA", An);
        _mat.SetVector("_LineB", Bn);
    }

    static void GetLineScreenEndpoints(RectTransform rt, out Vector2 a, out Vector2 b)
    {
        Rect r = rt.rect;
        bool horizontal = Mathf.Abs(r.width) >= Mathf.Abs(r.height);

        Vector3 center = r.center;
        Vector3 halfDir = horizontal ? Vector3.right * r.width * 0.5f
                                     : Vector3.up * r.height * 0.5f;

        Vector3 wA = rt.TransformPoint(center - halfDir);
        Vector3 wB = rt.TransformPoint(center + halfDir);

        Canvas canvas = rt.GetComponentInParent<Canvas>();
        Camera uiCam = canvas ? canvas.worldCamera : null;

        a = RectTransformUtility.WorldToScreenPoint(uiCam, wA);
        b = RectTransformUtility.WorldToScreenPoint(uiCam, wB);
    }
}