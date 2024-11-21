using UnityEngine;

public class DamageSkill : MonoBehaviour
{
    public int damageSkill;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerHealth = collision.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageSkill);
            }
        }
    }

}
