using System.Collections;
using UnityEngine;

public class Boss_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform[] patrolPoints;

    [Header("Skill Settings")]
    public float skillCooldown = 3f;
    public GameObject skill1;
    public GameObject skill2;
    public GameObject skill3;
    public GameObject skill4;
    public GameObject skill5;

    [Header("Skill Spawn Points")]
    public Transform skillSpawnPoint1;
    public Transform skillSpawnPoint2;
    public Transform skillSpawnPoint3;
    public Transform skillSpawnPoint4;
    public Transform skillSpawnPoint5;

    public float attackRange = 5f;
    public float chaseRange = 10f;

    private Animator animator;
    private int currentPatrolIndex = 0;
    private float skillCooldownTimer;
    private bool facingRight = true;

    private Transform player;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isAttacking) return;
        if (IsPlayerInChaseRange() && !IsPlayerInAttackRange())
        {
            ChasePlayer();
        }
        else if (!IsPlayerInChaseRange())
        {
            Patrol();
        }
        skillCooldownTimer -= Time.deltaTime;
        if (skillCooldownTimer <= 0f && IsPlayerInAttackRange())
        {
            UseSkill();
            skillCooldownTimer = skillCooldown;
        }
        if (IsPlayerInAttackRange())
        {
            StartCoroutine(StartAttack()); // Gọi tấn công
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = targetPoint.position - transform.position;
        animator.SetBool("Walk", true);
        if (animator.GetBool("Walk"))
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            if (direction.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && facingRight)
            {
                Flip();
            }
            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }

    private bool IsPlayerInChaseRange()
    {
        return player != null && Vector2.Distance(transform.position, player.position) < chaseRange;
    }

    private bool IsPlayerInAttackRange()
    {
        return player != null && Vector2.Distance(transform.position, player.position) < attackRange;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            animator.SetBool("Walk", true);
            if (player.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            else if (player.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
        }
    }

    void UseSkill()
    {
        int randomSkill = Random.Range(1, 6);
        switch (randomSkill)
        {
            case 1:
                SpawnAndMoveSkill(skill1, skillSpawnPoint1, 2, 2.5f);
                break;
            case 2:
                SpawnAndMoveSkill(skill2, skillSpawnPoint2, 4, 2.5f);
                break;
            case 3:
                SpawnAndMoveSkill(skill3, skillSpawnPoint3, 3, 2.5f);
                break;
            case 4:
                SpawnAndMoveSkill(skill4, skillSpawnPoint4, 5, 2.5f);
                break;
            case 5:
                SpawnAndMoveSkill(skill5, skillSpawnPoint5, 2, 2.5f);
                break;
        }
    }
    void SpawnAndMoveSkill(GameObject skillPrefab, Transform spawnPoint, float bulletSpeed, float skillLifetime)
    {
        if (skillPrefab != null && spawnPoint != null)
        {
            GameObject skillInstance = Instantiate(skillPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody2D rbSkill = skillInstance.GetComponent<Rigidbody2D>();

            if (rbSkill != null)
            {
                Vector2 direction = facingRight ? Vector2.right : Vector2.left;
                if (!facingRight)
                {
                    Vector3 scale = skillInstance.transform.localScale;
                    scale.x *= -1;
                    skillInstance.transform.localScale = scale;
                }
                rbSkill.velocity = direction * bulletSpeed;
            }
            Destroy(skillInstance, skillLifetime);
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private IEnumerator StartAttack()
    {
        {
            if (isAttacking) yield break; // Ngăn không cho gọi nhiều lần
            isAttacking = true;
            animator.SetBool("Walk", false);
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(6f);
            animator.SetBool("Idle", true);
            isAttacking = false;
        }
    }

}
