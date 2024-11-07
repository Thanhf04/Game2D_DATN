using UnityEngine;

public class DamageBoss : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask Player;
    public void Attack()
    {

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, Player);

        if (hit)
        {
            Dichuyennv1 playerHealth = hit.GetComponent<Dichuyennv1>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

}
