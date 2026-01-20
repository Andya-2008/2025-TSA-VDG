using UnityEngine;

[RequireComponent(typeof(AnimatedSprite))]
[RequireComponent(typeof(SpriteRenderer))]
public class DirectionalAnimation : MonoBehaviour
{
    public Sprite[] upSprites;
    public Sprite[] downSprites;
    public Sprite[] sideSprites; // used for both left & right

    private AnimatedSprite animatedSprite;
    private SpriteRenderer spriteRenderer;
    private Movement movement;

    private enum AnimState { Up, Down, Side }
    private AnimState currentState;

    private void Awake()
    {
        animatedSprite = GetComponent<AnimatedSprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GameObject.Find("Player").GetComponent<Movement>();
    }

    private void Update()
    {
        Vector2 dir = movement.direction;

        if (dir == Vector2.zero)
            return;

        // Vertical
        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0)
                SetAnimation(AnimState.Up, upSprites, false);
            else
                SetAnimation(AnimState.Down, downSprites, false);
        }
        // Horizontal
        else
        {
            bool flip = dir.x < 0;
            SetAnimation(AnimState.Side, sideSprites, flip);
        }
    }

    private void SetAnimation(AnimState state, Sprite[] sprites, bool flipX)
    {
        if (currentState == state && spriteRenderer.flipX == flipX)
            return;

        currentState = state;
        spriteRenderer.flipX = flipX;

        animatedSprite.sprites = sprites;
        animatedSprite.Restart();
    }
}