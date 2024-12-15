using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float start; // Điểm bắt đầu tuần tra
    public float end; // Điểm kết thúc tuần tra
    public bool isRight;
    public float speed;
    public float attackRange = 2.0f;
    public float chaseRange = 5.0f;

    private GameObject player;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                return;
            }
        }

        float quai = transform.position.x;
        Vector3 playerPosition = player.transform.position;

        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer(playerPosition);
        }
        else
        {
            animator.SetBool("isAttacking", false);
            Patrol(quai);
        }
    }

    public void SetPatrolRange(float patrolStart, float patrolEnd)
    {
        start = patrolStart;
        end = patrolEnd;
    }

    void Patrol(float quai)
    {
        if (quai < start)
        {
            isRight = true;
        }
        if (quai > end)
        {
            isRight = false;
        }

        Vector2 v = transform.localScale;
        if (isRight)
        {
            v.x = Mathf.Abs(v.x);
            transform.Translate(Vector3.right * speed * Time.deltaTime, 0);
        }
        else
        {
            v.x = -Mathf.Abs(v.x);
            transform.Translate(Vector3.left * speed * Time.deltaTime, 0);
        }
        transform.localScale = v;
    }

    void ChasePlayer(Vector3 playerPosition)
    {
        animator.SetBool("isAttacking", false);
        Vector2 v = transform.localScale;

        if (playerPosition.x > transform.position.x)
        {
            isRight = true;
        }
        else
        {
            isRight = false;
        }

        if (isRight)
        {
            v.x = Mathf.Abs(v.x);
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            v.x = -Mathf.Abs(v.x);
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        transform.localScale = v;
    }

    void AttackPlayer()
    {
        animator.SetBool("isAttacking", true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(start, transform.position.y, 0),
            new Vector3(end, transform.position.y, 0)
        );
    }
}
