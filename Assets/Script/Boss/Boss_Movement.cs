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
    [Header("SkillPoint")]
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
        if (skillCooldownTimer <= 0f && IsPlayerInAttackRange())
        {
            UseSkill();
            skillCooldownTimer = skillCooldown;
        }
        if (IsPlayerInAttackRange())
        {
            animator.SetBool("AttackBasic", true);
        }
        else if (!IsPlayerInAttackRange())
        {
            animator.SetBool("AttackBasic", false);
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

    // Hàm được gọi từ Animation Event để thực hiện skill sau khi animation hoàn tất
    void UseSkill()
    {
        SpawnAndMoveSkill(skill1, skillSpawnPoint1, 10, 1);
        SpawnAndMoveSkill(skill2, skillSpawnPoint2, 2, 3);
        SpawnAndMoveSkill(skill3, skillSpawnPoint3, 8, 1);
        SpawnAndMoveSkill(skill4, skillSpawnPoint4, 5, 1);
        SpawnAndMoveSkill(skill5, skillSpawnPoint5, 10, 1);
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

                Vector2 direction = facingRight ? Vector2.right : Vector2.left; // Nếu nhìn sang phải, skill di chuyển sang phải, ngược lại sẽ di chuyển sang trái

                // Đảo chiều trục X của skill nếu quái vật nhìn sang trái
                if (!facingRight)
                {
                    Vector3 scale = skillInstance.transform.localScale;
                    scale.x *= -1; // Đảo chiều trục X
                    skillInstance.transform.localScale = scale;
                }

                // Thiết lập vận tốc cho skill
                rbSkill.velocity = direction * bulletSpeed; // Đặt vận tốc theo hướng
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
}
