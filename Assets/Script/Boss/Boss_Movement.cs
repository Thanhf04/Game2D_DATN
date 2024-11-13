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

    private bool isAttacking = false;  // Kiểm tra xem quái vật có đang tấn công hay không

    void Start()
    {
        animator = GetComponent<Animator>();
        skillCooldownTimer = skillCooldown;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (IsPlayerInChaseRange())
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        skillCooldownTimer -= Time.deltaTime;

        // Kiểm tra cooldown skill và tấn công nếu có thể
        if (skillCooldownTimer <= 0f && IsPlayerInAttackRange())
        {
            UseSkill();
            skillCooldownTimer = skillCooldown;
        }

        if (IsPlayerInAttackRange())
        {
            // Tấn công chỉ khi cooldown đã hết và animation chưa được phát
            animator.SetBool("AttackBasic", true);
            isAttacking = true;
            StartCoroutine(StartAttack());
            StartCoroutine(ResetAttackAnimation());
        }
        else if (!IsPlayerInAttackRange())
        {
            // Tắt animation tấn công khi không còn trong phạm vi tấn công
            animator.SetBool("AttackBasic", false);
            isAttacking = false;
        }


    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        Vector2 direction = targetPoint.position - transform.position;

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

    // Hàm để sử dụng một skill ngẫu nhiên khi gặp người chơi
    void UseSkill()
    {
        int randomSkill = Random.Range(1, 6); // Chọn ngẫu nhiên từ skill 1 đến skill 5

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

    // Hàm để khởi tạo và thiết lập hướng di chuyển cho mỗi skill
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
    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(2f);
        animator.SetBool("AttackBasic", false);
    }
    private IEnumerator StartAttack()
    {
        animator.SetBool("AttackBasic", true);
        yield return new WaitForSeconds(1f);
    }
}
