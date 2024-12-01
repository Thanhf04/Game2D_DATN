using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Slider healthSlider;
    public int damageAmount = 1;
    public GameObject prefabsItem;
    NewPlayer player;

    // Được gọi khi đối tượng được spawn

    void Start()
    {
        UpdateHealthSlider();
    }

    void Update()
    {
        player = FindObjectOfType<NewPlayer>();
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
            healthSlider.interactable = false;
        }
    }

    void Die()
    {
        if (player != null)
        {
            player.LevelSlider(50);
            Debug.Log("Gọi LevelSlider thành công");
        }
        else
        {
            Debug.LogError("Không thể gọi LevelSlider vì player là null");
        }
        Destroy(gameObject);
        DropItem();


    }
    public void DropItem()
    {
        if (prefabsItem != null)
        {
            Instantiate(prefabsItem, transform.position, Quaternion.identity);
        }
    }
}
