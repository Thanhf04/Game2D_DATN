using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask Boss;
    public float attackRadius = 1f;
    Animator animator;

    public void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack point chưa được gán.");
        }

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, Boss);
        if (hit)
        {
            Boss_Health bossHealth = hit.GetComponent<Boss_Health>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(10);
            }
            else
            {
                Debug.LogWarning("Đối tượng không có Boss_Health.");
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

    }

}
