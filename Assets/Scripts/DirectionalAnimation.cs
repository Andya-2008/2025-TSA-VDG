using UnityEngine;

public class DirectionalAnimation : MonoBehaviour
{
    [Header("Sprite Sets")]
    public Sprite[] upSprites;
    public Sprite[] downSprites;
    public Sprite[] sideSprites;

    [Header("Rotations (Z degrees)")]
    public float rightRotation = 0f;
    public float leftRotation = 180f;
    public float upRotation = -90f;
    public float downRotation = 90f;

    [Header("Custom Scale")]
    public Vector3 sideScale = Vector3.one;
    public Vector3 upScale = Vector3.one;
    public Vector3 downScale = Vector3.one;

    private AnimatedSprite animatedSprite;
    private SpriteRenderer spriteRenderer;
    private Movement movement;
    private Transform visual;

    private enum AnimState { Up, Down, Side }
    private AnimState currentState;

    // 🔑 Track applied values ourselves
    private float currentRotation;
    private bool currentFlip;

    private void Awake()
    {
        visual = transform;
        animatedSprite = GetComponent<AnimatedSprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponentInParent<Movement>();
    }

    private void Update()
    {
        Vector2 dir = movement.direction;
        if (dir == Vector2.zero) return;

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0)
                SetAnim(AnimState.Up, upSprites, upRotation, upScale, false);
            else
                SetAnim(AnimState.Down, downSprites, downRotation, downScale, false);
        }
        else
        {
            if (dir.x > 0)
                SetAnim(AnimState.Side, sideSprites, rightRotation, sideScale, false);
            else
                SetAnim(AnimState.Side, sideSprites, leftRotation, sideScale, true);
        }
    }

    private void SetAnim(
        AnimState state,
        Sprite[] sprites,
        float rotation,
        Vector3 scale,
        bool flipX
    )
    {
        if (currentState == state &&
            animatedSprite.sprites == sprites &&
            currentRotation == rotation &&
            currentFlip == flipX)
            return;

        currentState = state;
        currentRotation = rotation;
        currentFlip = flipX;

        spriteRenderer.flipX = flipX;
        visual.localRotation = Quaternion.Euler(0f, 0f, rotation);
        visual.localScale = scale;

        animatedSprite.sprites = sprites;
        animatedSprite.Restart();
    }
}
