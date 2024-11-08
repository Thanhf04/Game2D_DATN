using UnityEngine;

public class Damage : MonoBehaviour
{
    private Dichuyennv1 dichuyennv1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu đối tượng va chạm là quái vật
        if (other.CompareTag("enemy"))
        {
            Debug.Log("Player đã va chạm với quái vật!");

            // Kiểm tra và lấy đối tượng Enemy
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(dichuyennv1.damageAmount); // Gây sát thương
                Debug.Log("Gây damage cho quái vật!");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy component Enemy trên đối tượng va chạm!");
            }
        }
    }

    void Start()
    {
        // Lấy component Dichuyennv1 từ đối tượng cha (player)
        dichuyennv1 = GetComponentInParent<Dichuyennv1>();

        // Kiểm tra nếu không tìm thấy Dichuyennv1
        if (dichuyennv1 == null)
        {
            Debug.LogError("Không tìm thấy Dichuyennv1 trên đối tượng cha!");
        }
    }
}
