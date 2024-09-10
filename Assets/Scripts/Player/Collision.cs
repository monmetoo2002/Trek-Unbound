using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]

    public float collisionGroundRadius = 0.25f;
    public float normalWallRaycastLength = 0.25f;
    public float extendedWallRaycastLength = 0.5f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    private Movement movement;

    void Start()
    {
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionGroundRadius, groundLayer);

        float currentWallRaycastLength = (Input.GetButton("Fire3") || movement.isFalling) ? extendedWallRaycastLength : normalWallRaycastLength;

        RaycastHit2D hitRight = Physics2D.Raycast((Vector2)transform.position + rightOffset, Vector2.right, currentWallRaycastLength, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast((Vector2)transform.position + leftOffset, Vector2.left, currentWallRaycastLength, groundLayer);

        onRightWall = hitRight.collider != null;
        onLeftWall = hitLeft.collider != null;
        onWall = onRightWall || onLeftWall;

        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionGroundRadius);

        float currentWallRaycastLength = (movement != null && (Input.GetButton("Fire3") || movement.isFalling)) ? extendedWallRaycastLength : normalWallRaycastLength;

        Gizmos.DrawRay((Vector2)transform.position + rightOffset, Vector2.right * currentWallRaycastLength);
        Gizmos.DrawRay((Vector2)transform.position + leftOffset, Vector2.left * currentWallRaycastLength);
    }
}