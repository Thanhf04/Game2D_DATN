using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask Boss;
    public float attackRadius = 1f;
    Animator animator;
    Boss_Health health;

    public void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack point chưa được gán.");
        }

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, Boss);
        if (hit)
        {
            health = hit.GetComponent<Boss_Health>();
            if (health != null)
            {
                health.TakeDamage(10);
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
