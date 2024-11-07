using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask Boss;
    public float attackRadius = 1f;
    Animator animator;

    public void Attack()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, Boss);
        if (hit)
        {
            hit.GetComponent<Boss_Health>().TakeDamage(10);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

    }

}
