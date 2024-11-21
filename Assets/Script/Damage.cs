using UnityEngine;

public class Damage : MonoBehaviour
{
    private Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.Damage); 
            }
        }
    }

    void Start()
    {
        player = GetComponentInParent<Player>();
    }
}
