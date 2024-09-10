using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour, IDataPersistence
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;
    AudioManager audioManager;
    private DialogueSystem dialogueSystem;

    [Space]
    [Header("Stats")]
    [SerializeField] float speed = 10;
    [SerializeField] float jumpForce = 50;
    [SerializeField] float slideSpeed = 5;
    [SerializeField] float wallJumpLerp = 10;

    [SerializeField] private float dashDistance = 5f; // Khoảng cách dash
    [SerializeField] private float dashDuration = 0.3f; // Thời gian dash
    private Vector2 dashDirection;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    public bool canDoubleJump;
    private bool groundTouch;
    private bool hasDashed;
    public bool isFalling = false;
    private bool movementEnabled = true;

    // Hướng nhìn của nhân vật
    public int side = 1;

    // Biến tham chiếu đến hệ thống hạt cho hiệu ứng
    [Space]
    [Header("ParticleSystem")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
        dialogueSystem = FindObjectOfType<DialogueSystem>();

    }

    public void LoadData(GameData data)
    {
        Vector3 loadPosition = data.lastCheckpointPosition;
        if (loadPosition == Vector3.zero)
        {
            // Nếu chưa đi qua checkpoint, khởi tạo ở vị trí ban đầu
            loadPosition = this.transform.position;
        }
        this.transform.position = loadPosition;

    }

    public void SaveData(GameData data)
    {

    }

    public void DisableMovement()
    {
        movementEnabled = false;
        canMove = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

    // New function to enable character movement
    public void EnableMovement()
    {
        movementEnabled = true;
        canMove = true;
        rb.isKinematic = false;
    }

    void Update()
    {
        if (!movementEnabled)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal"); // Lấy giá trị thô của các trục đầu vào.
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        // Kiểm tra xem dialogue có đang active không
        if (dialogueSystem != null && dialogueSystem.IsDialogueActive())
        {
            // Nếu dialogue đang active, dừng mọi chuyển động
            rb.velocity = Vector3.zero;
            anim.SetHorizontalMovement(0, 0, 0); // Anim = Idle
            return;
        }

        // Hàm di chuyển
        Walk(dir);

        anim.SetHorizontalMovement(x, y, rb.velocity.y);  // Cập nhật animation theo hướng di chuyển và tốc độ.

        // Check hướng nhìn khi giữ nút LeftShift
        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if (side != coll.wallSide)
                anim.Flip(side * -1);
            wallGrab = true;
            wallSlide = false;
        }

        // Check wallGrab khi nhả nút LeftShift
        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        // Check trượt tường
        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
        {
            wallSlide = false;
        }

        // Check Wall Grab khi đang trên tường
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            //if(x > 0.2f || x < -0.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            //float speedModifier = y > 0 ? 0.5f : 1; // Đặt điều kiện leo tường chậm hơn
            //rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        // Check better jump
        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            canDoubleJump = true; // Reset double jump khi đang ở trên mặt đất
            GetComponent<BetterJumping>().enabled = true;
        }

        // Check nhảy

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            // trên mặt đất 
            if (coll.onGround)
            {
                Jump(Vector2.up, false);
            }
            else if (canDoubleJump) // Double jump khi đã nhảy từ trước
            {
                Jump(Vector2.up, false);
                canDoubleJump = false; // Không cho phép double jump nữa
            }
            // trên tường
            if (coll.onWall && !coll.onGround)
                WallJump();
        }


        // Check lướt
        if (Input.GetButtonDown("Fire1") && (!hasDashed))
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        // Check chạm đất
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        // Hiệu ứng nhảy
        WallParticle(y);

        // Thoát hàm update nếu
        if (wallGrab || wallSlide || !canMove)
            return;

        // Đổi hướng nhìn nhân vật
        if (x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }

        // Cập nhật trạng thái rơi
        isFalling = rb.velocity.y < 0 && !coll.onGround;

        // Nếu không trên tường
        if (!coll.onWall)
        {
            wallGrab = false;
        }
    }

    // Xử lý hướng nhìn khi chạm đất
    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    // Xử lý Dash
    private void Dash(float x, float y)
    {
        // Kiểm tra Dialogue
        if (dialogueSystem != null && dialogueSystem.IsDialogueActive())
            return;

        // Thêm âm thanh
        audioManager.PlaySFX(audioManager.dash);

        if (hasDashed) return;

        hasDashed = true;
        anim.SetTrigger("dash");

        // Chuẩn hóa vector hướng dash
        dashDirection = new Vector2(x, y).normalized;

        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        float startTime = Time.time;
        Vector2 startPosition = rb.position;
        Vector2 endPosition = startPosition + dashDirection * dashDistance;

        isDashing = true;

        StartCoroutine(GroundDash());
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        GetComponent<BetterJumping>().enabled = false;

        dashParticle.Play();

        while (Time.time < startTime + dashDuration)
        {
            float t = (Time.time - startTime) / dashDuration;
            rb.MovePosition(Vector2.Lerp(startPosition, endPosition, t));
            yield return null;
        }

        rb.MovePosition(endPosition);


        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        isDashing = false;
        dashParticle.Stop();

    }

    // Xử lý Dash trên mặt đất
    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(0.3f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        audioManager.PlaySFX(audioManager.jump);
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side = side * -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        // Tạo hướng tường dựa trên vị trí tường
        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        // Tính toán tốc độ nhảy tường
        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    // Xử lý trượt tường
    private void WallSlide()
    {
        // Check hướng tường với hướng nhìn
        if (coll.wallSide != side)
            anim.Flip(side * -1);

        // Nếu không di chuyển được thì thoát hàm
        if (!canMove)
            return;

        // Check bám tường
        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }

        // Cập nhật vận tốc theo hướng trượt
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    // Xử lý di chuyển
    private void Walk(Vector2 dir)
    {
        // Kiểm tra Dialogue
        if (!movementEnabled || !canMove || (dialogueSystem != null && dialogueSystem.IsDialogueActive()))
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }

    }

    // Xử lý nhảy
    private void Jump(Vector2 dir, bool wall)
    {
        // Kiểm tra Dialogue
        if (dialogueSystem != null && dialogueSystem.IsDialogueActive())
            return;

        // Thêm âm thanh
        audioManager.PlaySFX(audioManager.jump);

        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1); // Cập nhật particle theo hướng nhìn
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle; // Chọn partical khi ở trên tường hoặc không

        // Đặt vận tốc y thành 0 và cộng thêm vận tốc dựa trên hướng nhảy và jumpForce
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        particle.Play();

        // Nếu không phải nhảy từ tường, cho phép double jump
        if (!wall)
            canDoubleJump = true;
    }

    // Vô hiệu hóa di chuyển 
    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    public void ResetDash()
    {
        hasDashed = false;
        isDashing = false;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main; // Lấy thuộc tính main của partical

        // Check tường
        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

}