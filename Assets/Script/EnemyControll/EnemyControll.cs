using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float start, end;
    public bool isRight;
    public float speed;
    public float attackRange = 2.0f; // Phạm vi tấn công
    public float chaseRange = 5.0f; // Phạm vi đuổi theo

    private GameObject player; // Người chơi
    private Animator animator; // Animator của quái

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>(); // Lấy Animator từ quái
    }

    void Update()
    {
        var quai = transform.position.x;
        var playerPosition = player.transform.position;

        // Kiểm tra nếu người chơi đang trong phạm vi đuổi theo hoặc phạm vi tấn công
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer(); // Tấn công khi ở gần người chơi
        }
        else if (distanceToPlayer <= chaseRange)
        {
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
        // Di chuyển giữa start và end
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
            transform.Translate(Vector3.right * speed * Time.deltaTime,0);
        }
        else
        {
            v.x = -Mathf.Abs(v.x);
            transform.Translate(Vector3.left * speed * Time.deltaTime,0);
        }
        transform.localScale = v;
    }

    void ChasePlayer(Vector3 playerPosition)
    {
        // Đuổi theo người chơi
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
        // Kích hoạt animation tấn công khi ở gần người chơi
        animator.SetBool("isAttacking", true);
    }

    private void OnDrawGizmos()
    {
        // Vẽ phạm vi đuổi theo và phạm vi tấn công để dễ kiểm tra trong Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Vẽ vòng tròn phạm vi tấn công

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange); // Vẽ vòng tròn phạm vi đuổi theo
    }
}
