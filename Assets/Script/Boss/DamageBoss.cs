using UnityEngine;

public class DamageBoss : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask Player;
    public int damageBoss;
    public void Attack()
    {

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, Player);

        if (hit)
        {
            NewPlayer playerHealth = hit.GetComponent<NewPlayer>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageBoss);
                Debug.Log("dame");
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
