using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Sức khỏe tối đa của quái
    public int currentHealth;
    public Slider HealthSlider;
    Dichuyennv1 dichuyennv1;
    public GameObject prefabsItem;

    void Start()
    {
        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = maxHealth;
        currentHealth = maxHealth;
        dichuyennv1 = FindObjectOfType<Dichuyennv1>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount; // Giảm sức khỏe khi nhận sát thương

        if (currentHealth <= 0)
        {
            Die(); // Gọi hàm chết nếu sức khỏe bằng 0


        }
        HealthSlider.value = currentHealth;
    }

    void Die()
    {
        if (dichuyennv1 != null)
        {
            dichuyennv1.LevelSlider(50);
        }
        DropItem();
        // Xử lý cái chết của quái vật, như là biến mất hoặc phát animation chết
        Destroy(gameObject); // Xóa quái vật
    }
    void DropItem()
    {
        if (prefabsItem != null)
        {
            Instantiate(prefabsItem, transform.position, Quaternion.identity);
        }
    }
    internal void TakeDamage(float damage)
    {
        throw new NotImplementedException();
    }
}
