using UnityEngine;

public class DamageSkill : MonoBehaviour
{
    public int damageSkill;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Dichuyennv1 playerHealth = collision.GetComponent<Dichuyennv1>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageSkill);
            }
        }
    }

}
