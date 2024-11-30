using UnityEngine;

public class DamageSkill : MonoBehaviour
{
    public int damageSkill;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            NewPlayer playerHealth = collision.GetComponent<NewPlayer>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageSkill);
            }
        }
    }

}
