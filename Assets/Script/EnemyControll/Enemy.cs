using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Sức khỏe tối đa của quái
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Khởi tạo sức khỏe hiện tại
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount; // Giảm sức khỏe khi nhận sát thương

        if (currentHealth <= 0)
        {
            Die(); // Gọi hàm chết nếu sức khỏe bằng 0
        }
    }

    void Die()
    {
        // Xử lý cái chết của quái vật, như là biến mất hoặc phát animation chết
        Destroy(gameObject); // Xóa quái vật
    }
}
