using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapGai : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Gây sát thương cho người chơi nếu vùng sát thương chạm
            Dichuyennv1 playerHealth = collision.GetComponent<Dichuyennv1>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // Gây 10 sát thương
            }
        }
    }
}
