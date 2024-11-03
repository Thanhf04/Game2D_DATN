using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageEnemy : MonoBehaviour
{
    public int damageAmount = 10; // Sát thương gây ra khi va chạm

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Dichuyennv1 playerHealth = other.GetComponent<Dichuyennv1>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount); // Gây sát thương cho người chơi
            }
        }
    }
}
