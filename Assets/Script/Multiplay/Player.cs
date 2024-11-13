using System.Collections;
using Fusion;
using Fusion.Addons;
using Fusion.Addons.Physics;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkRigidbody2D _rigidbody2D;
    private NetworkMecanimAnimator _networkAnimator;

     private PlayerRef controllingPlayer;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool isGrounded = false;

    // Health, mana, and damage
    [Networked]
    public int Health { get; set; } = 100;

    [Networked]
    public int Mana { get; set; } = 50;

    [Networked]
    public int Damage { get; set; } = 10;

    public float attackRange = 1f;
    public float rollSpeed = 15f;
    public float rollDuration = 0.5f;
    private bool isRolling = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<NetworkRigidbody2D>();
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    public void SetPlayerControl(PlayerRef player)
    {
        controllingPlayer = player; // Gán người chơi điều khiển
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

            if (data.isJumping && isGrounded)
            {
                Jump();
            }

            HandleRolling(data.isRolling);
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
        if (isRollingInput && !isRolling)
        {
            StartCoroutine(Roll());
        }
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        _networkAnimator.Animator.SetBool("isRoll", isRolling);

        Vector2 rollDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        _rigidbody2D.Rigidbody.velocity = new Vector2(
            rollDirection.x * rollSpeed,
            _rigidbody2D.Rigidbody.velocity.y
        );

        yield return new WaitForSeconds(rollDuration);

        isRolling = false;
        _networkAnimator.Animator.SetBool("isRoll", isRolling);
        _rigidbody2D.Rigidbody.velocity = new Vector2(0, _rigidbody2D.Rigidbody.velocity.y);
    }

    private void HandleMovement(float velocityX, bool attacking)
    {
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
