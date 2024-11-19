using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float start,
        end;
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
            player = GameObject.FindWithTag("Player"); // Tìm lại player nếu player chưa được spawn
            if (player == null)
            {
                return; // Nếu không tìm thấy player, không làm gì cả
            }
        }

        // Quái vật đã tìm thấy người chơi, tiếp tục xử lý logic sau đây
        float quai = transform.position.x;
        Vector3 playerPosition = player.transform.position;

        // Kiểm tra nếu người chơi đang trong phạm vi đuổi theo hoặc phạm vi tấn công
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        if (distanceToPlayer <= attackRange)
        {
            Debug.Log("Tấn công người chơi");
            AttackPlayer(); // Tấn công khi ở gần người chơi
        }
        else if (distanceToPlayer <= chaseRange)
        {
            Debug.Log("Đuổi theo người chơi");
            ChasePlayer(playerPosition); // Đuổi theo người chơi khi còn xa
        }
        else
        {
            animator.SetBool("isAttacking", false); // Ngừng animation tấn công khi người chơi ra khỏi phạm vi
            Patrol(quai); // Tiếp tục di chuyển tuần tra nếu người chơi không trong phạm vi
        }
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
    }
}
