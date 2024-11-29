using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask Object;
    public float attackRadius = 1f;
    Animator animator;
    Boss_Health health;
    Player player1;
    private void Start()
    {
        player1 = FindObjectOfType<Player>();
    }
    public void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack point chưa được gán.");
        }

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, Object);
        if (hit)
        {
            health = hit.GetComponent<Boss_Health>();
            if (health != null)
            {
                health.TakeDamage(player1.Damage);
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
