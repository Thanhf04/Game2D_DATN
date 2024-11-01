using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour

{
    private Dichuyennv1 dichuyennv1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu đối tượng va chạm là quái vật
        if (other.CompareTag("enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(dichuyennv1.damageAmount); // Gọi phương thức gây sát thương
                Debug.Log("gay damage ne");
            }
        }
    }
    void Start() {
    dichuyennv1 = GetComponent<Dichuyennv1>(); // Khởi tạo dichuyennv1
}
}
