
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth  = 100;
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


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
