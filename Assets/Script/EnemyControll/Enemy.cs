using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Slider healthSlider;
    public int damageAmount = 1;

    // Được gọi khi đối tượng được spawn

    void Start()
    {
        UpdateHealthSlider();
    }

    void Update()
    {
        UpdateHealthSlider();
    }

    // Phương thức nhận sát thương
    public void TakeDamage(int damage)
    {
        // Giảm máu khi nhận sát thương
        currentHealth -= damage;

        // Nếu máu <= 0, quái vật chết
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthSlider();
    }

    // Cập nhật thanh máu
    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    // Xử lý cái chết của quái vật
    void Die()
    {
        Destroy(gameObject);
    }
}
