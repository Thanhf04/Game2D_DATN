using UnityEngine;

public class Damage : MonoBehaviour
{
    private Dichuyennv1 player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.damageAmount);
            }

        }
    }

    void Start()
    {
        player = GetComponentInParent<Dichuyennv1>();
        if (player == null)
        {
            Debug.LogError("Không tìm thấy NewPlayer trong đối tượng cha của Damage.");
        }
    }
    
}
