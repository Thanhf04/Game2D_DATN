using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : NetworkBehaviour
{
    [Networked] public int maxHealth { get; set; } = 100;
    [Networked] public int currentHealth { get; set; } = 100;
    public Slider healthSlider;
    public int damageAmount = 1;

    // Được gọi khi đối tượng được spawn
    public override void Spawned()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Phương thức nhận sát thương
    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)  // Chỉ Host mới thay đổi giá trị Networked properties
        {
            // Giảm máu khi nhận sát thương
            currentHealth -= damage;

            // Nếu máu <= 0, quái vật chết
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
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
        if (Object.HasStateAuthority)
        {
            // Despawn quái vật khi chết
            Runner.Despawn(Object);
        }
    }
}
