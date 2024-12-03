using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Slider healthSlider;
    public int damageAmount = 1;
    public GameObject prefabsItem;
    private Dichuyennv1 player;
    private NPCQuest npcQuest; // Tham chiếu đến NPCQuest

    void Start()
    {
        npcQuest = FindObjectOfType<NPCQuest>(); // Tìm NPCQuest trong scene
        UpdateHealthSlider();
    }

    void Update()
    {
        player = FindObjectOfType<Dichuyennv1>();
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
        if (npcQuest != null)
        {
            npcQuest.KillMonster(); // Gọi hàm KillMonster trong NPCQuest khi quái vật chết
        }
        Destroy(gameObject); // Xóa con quái vật
        DropItem();
    }

    public void DropItem()
    {
        if (prefabsItem != null)
        {
            Instantiate(prefabsItem, transform.position, Quaternion.identity); // Thả item khi quái chết
        }
    }
}
