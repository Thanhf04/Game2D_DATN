using System.Collections;
using Cinemachine;
using Fusion;
using Fusion.Addons;
using Fusion.Addons.Physics;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkRigidbody2D _rigidbody2D;
    private NetworkMecanimAnimator _networkAnimator;
    private PlayerRef controllingPlayer;
    private int jumpCount = 0;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isGrounded = false;

    public int Health = 100;

    public int Mana = 100;

    public int Damage = 10;

    public int maxHealth = 100;
    public int maxMana = 100;


    public float attackRange = 1f;
    private CinemachineVirtualCamera vCam;

    [SerializeField]
    public AudioSource music;
    public AudioSource playWalk;
    public AudioSource playAttack;
    public AudioSource playAttack2;
    public AudioSource playAttack_Fire1;
    public AudioSource playAttack_Fire2;
    public AudioSource playAttack_Fire3;
    public AudioSource playJump;
    private bool wasJumpPressed = false;

    void Start()
    {
        if (Object.HasInputAuthority)
        {
            vCam = FindObjectOfType<CinemachineVirtualCamera>();

            if (vCam != null)
            {
                vCam.Follow = transform;
                vCam.LookAt = transform;
            }
            Playmusic();
        }
    }

    void Playmusic()
    {
        music.Play();
        playWalk.Stop();
        playAttack.Stop();
        playAttack2.Stop();
        playAttack_Fire1.Stop();
        playAttack_Fire2.Stop();
        playAttack_Fire3.Stop();
        playJump.Stop();
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<NetworkRigidbody2D>();
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    public void SetPlayerControl(PlayerRef player)
    {
        controllingPlayer = player;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector2 velocity = data.direction * moveSpeed;
            _rigidbody2D.Rigidbody.velocity = new Vector2(
                velocity.x,
                _rigidbody2D.Rigidbody.velocity.y
            );

            if (data.isJumping && !wasJumpPressed)
            {
                if (CanJump())
                {
                    Jump();
                }
            }
            wasJumpPressed = data.isJumping;
            HandleMovement(velocity.x, data.isAttacking);
        }
    }

    private bool CanJump()
    {
        Debug.Log(jumpCount);
        return isGrounded || jumpCount < 2;
    }

    private void Jump()
    {
        _rigidbody2D.Rigidbody.velocity = new Vector2(_rigidbody2D.Rigidbody.velocity.x, 0);
        _rigidbody2D.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        playJump.Play();

        jumpCount++;
        isGrounded = false;

        // Đồng bộ trạng thái "đang nhảy"
        _networkAnimator.Animator.SetBool("isJump", true);
    }

    private void HandleMovement(float velocityX, bool attacking)
    {
        if (velocityX != 0 && !playWalk.isPlaying)
        {
            playWalk.Play();
        }
        else if (velocityX == 0 && playWalk.isPlaying)
        {
            playWalk.Stop();
        }
        _networkAnimator.Animator.SetBool("isRunning", velocityX != 0);

        if (attacking && !_networkAnimator.Animator.GetBool("isAttack"))
        {
            StartCoroutine(Attack());
        }

        FlipSprite(velocityX);
    }

    private IEnumerator Attack()
    {
        _networkAnimator.Animator.SetBool("isAttack", true);
        playAttack.Play();
        PerformAttack();
        yield return new WaitForSeconds(0.5f);
        _networkAnimator.Animator.SetBool("isAttack", false);
    }

    private void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            LayerMask.GetMask("Enemy")
        );
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(Damage);
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            // Handle player death
            Die();
        }
    }

    private void Die()
    {
        // Add player death logic here (e.g., respawn, game over)
        Debug.Log("Player has died");
    }

    public void UseMana(int manaCost)
    {
        if (Mana >= manaCost)
        {
            Mana -= manaCost;
            // Perform ability if mana is sufficient
        }
        else
        {
            Debug.Log("Not enough mana");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = true;
            _networkAnimator.Animator.SetBool("isJump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            jumpCount = 0;
            isGrounded = false;
            _networkAnimator.Animator.SetBool("isJump", true);
        }
    }

    private void FlipSprite(float velocityX)
    {
        if (velocityX > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (velocityX < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
