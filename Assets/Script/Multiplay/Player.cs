using System.Collections;
using Fusion;
using Fusion.Addons.Physics;
using Fusion.Addons;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkRigidbody2D _rigidbody2D;
    private NetworkMecanimAnimator _networkAnimator; // NetworkMecanimAnimator replaces Animator reference

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool isGrounded = false;

    // Attack variables
    public float attackRange = 1f;
    public int attackDamage = 10;

    // Roll variables
    public float rollSpeed = 15f;
    public float rollDuration = 0.5f;
    private bool isRolling = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<NetworkRigidbody2D>();
        _networkAnimator = GetComponent<NetworkMecanimAnimator>(); // Initialize NetworkMecanimAnimator
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // Movement and jump handling
            Vector2 velocity = data.direction * moveSpeed;
            _rigidbody2D.Rigidbody.velocity = new Vector2(
                velocity.x,
                _rigidbody2D.Rigidbody.velocity.y
            );

            if (data.isJumping && isGrounded)
            {
                Jump();
            }

            // Handle roll input
            HandleRolling(data.isRolling);

            // Sync movement and animation states
            HandleMovement(velocity.x, data.isAttacking);
        }
    }

    private void Jump()
    {
        _rigidbody2D.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void HandleRolling(bool isRollingInput)
    {
        if (isRollingInput && !isRolling) // Start rolling when input is detected
        {
            StartCoroutine(Roll());
        }
    }

    private IEnumerator Roll()
    {
        // Start roll
        isRolling = true;

        // Set roll animation
        _networkAnimator.Animator.SetBool("isRoll", isRolling);

        // Apply rolling speed in the direction the player is facing
        Vector2 rollDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        _rigidbody2D.Rigidbody.velocity = new Vector2(
            rollDirection.x * rollSpeed,
            _rigidbody2D.Rigidbody.velocity.y
        );

        // Roll duration
        yield return new WaitForSeconds(rollDuration);

        // End roll and reset velocity
        isRolling = false;
        _networkAnimator.Animator.SetBool("isRoll", isRolling); // Reset roll animation
        _rigidbody2D.Rigidbody.velocity = new Vector2(0, _rigidbody2D.Rigidbody.velocity.y);
    }

    private void HandleMovement(float velocityX, bool attacking)
    {
        // Update Animator based on movement
        _networkAnimator.Animator.SetBool("isRunning", velocityX != 0);

        // Attack animation state
        if (attacking && !_networkAnimator.Animator.GetBool("isAttack"))
        {
            StartCoroutine(Attack());
        }

        // Flip the sprite to face the movement direction
        FlipSprite(velocityX);
    }

    private IEnumerator Attack()
    {
        _networkAnimator.Animator.SetBool("isAttack", true);

        // Simulate attack behavior (e.g., detecting enemies)
        PerformAttack();

        // Reset attack state after animation finishes
        yield return new WaitForSeconds(0.5f); // Adjust this to match your attack animation length
        _networkAnimator.Animator.SetBool("isAttack", false);
    }

    private void PerformAttack()
    {
        // Here you would detect enemies within range and apply damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            LayerMask.GetMask("Enemy")
        );
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = true;
            _networkAnimator.Animator.SetBool("isJump", false); // Stop jump animation on landing
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = false;
            _networkAnimator.Animator.SetBool("isJump", true); // Start jump animation on leaving ground
        }
    }

    // Flip the sprite based on movement direction
    private void FlipSprite(float velocityX)
    {
        if (velocityX > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Face right
        }
        else if (velocityX < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Face left
        }
    }

    // Debugging: Draw the attack range for visualization in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
