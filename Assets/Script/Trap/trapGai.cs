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
            NewPlayer playerHealth = collision.GetComponent<NewPlayer>();
            if (playerHealth != null)
            {
                // playerHealth.TakeDamage(10); // Gây 10 sát thương
            }
        }
    }
}
