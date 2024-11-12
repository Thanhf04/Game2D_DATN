using UnityEngine;

public class DamagePlayerSkill : MonoBehaviour
{
    public int DamageSkill;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("enemy") || collision.CompareTag("Boss"))
        {
            Enemy e = collision.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(DamageSkill);
                return;
            }
            Boss_Health boss_Health = collision.GetComponent<Boss_Health>();
            if (boss_Health != null)
            {
                boss_Health.TakeDamage(DamageSkill);
            }
        }
    }
}
