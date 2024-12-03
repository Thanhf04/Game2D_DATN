using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider; // Thanh sức khỏe
    public int damageAmount = 1;
    public GameObject prefabsItem; // Vật phẩm rơi ra
    public event Action OnDeath;
    public EnemyRespawn respawnManager; // Quản lý respawn

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            UpdateHealthSlider();
        }

        // Tìm EnemyRespawn nếu chưa được gán
        if (respawnManager == null)
        {
            respawnManager = FindObjectOfType<EnemyRespawn>(); 
        }
    }

    // Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();  // Gọi Die khi chết
        }
        UpdateHealthSlider();  // Cập nhật thanh sức khỏe
    }

    // Cập nhật thanh sức khỏe
    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Đặt lại sức khỏe
    public void ResetHealth()
    {
        currentHealth = 100;
        Debug.Log("hoi mau ne");
        UpdateHealthSlider();
    }

    // Quái vật chết
    private void Die()
    {
        DropItem();  // Rơi vật phẩm khi chết
        if (OnDeath != null)
        {
            OnDeath(); // Gọi sự kiện OnDeath nếu có
        }
        if (respawnManager != null)
        {
            respawnManager.OnDeath(this); // Quản lý respawn
        }
        gameObject.SetActive(false); // Ẩn quái vật khi chết
    }

    // Rơi vật phẩm
    public void DropItem()
    {
        if (prefabsItem != null)
        {
            Instantiate(prefabsItem, transform.position, Quaternion.identity);
        }
    }

    // Khi quái vật được kích hoạt lại
    public void Respawn()
    {
        gameObject.SetActive(true); // Kích hoạt lại quái vật
        ResetHealth(); // Đặt lại sức khỏe
        UpdateHealthSlider(); // Cập nhật lại thanh sức khỏe nếu cần
    }
}
