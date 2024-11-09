using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Sức khỏe tối đa của quái
    public Slider HealthSlider;

    void Start()
    {
        HealthSlider.value = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        maxHealth -= amount; // Giảm sức khỏe khi nhận sát thương

        if (maxHealth <= 0)
        {
            Die(); // Gọi hàm chết nếu sức khỏe bằng 0
        }
        HealthSlider.value = maxHealth;
    }

    void Die()
    {
        // Xử lý cái chết của quái vật, như là biến mất hoặc phát animation chết
        Destroy(gameObject); // Xóa quái vật
    }

    internal void TakeDamage(float damage)
    {
        throw new NotImplementedException();
    }
}
