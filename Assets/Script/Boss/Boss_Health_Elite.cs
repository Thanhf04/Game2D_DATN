using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Health_Elite : MonoBehaviour
{
    [SerializeField]
    private Slider HealthBoss;

    [SerializeField]
    public int maxHealth = 100;
    public int currentHealth;
    Animator animator;
    public GameObject prefabsItem;
    public GameObject PanelSkillBoss;
    Dichuyennv1 Player1;
    public static bool isDeath = false;
    public static bool isPanelKillBossElite = false;
    // Tham chiếu đến script nhiệm vụ
    public NPCQuestSkill2 npcQuestskill2;

    void Start()
    {
        HealthBoss.maxValue = maxHealth;
        HealthBoss.value = maxHealth;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        Player1 = FindObjectOfType<Dichuyennv1>();

        // Tìm script nhiệm vụ nếu chưa được gán
        if (npcQuestskill2 == null)
        {
            npcQuestskill2 = FindObjectOfType<NPCQuestSkill2>();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= (int)damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        animator.SetBool("Hit", true);
        StartCoroutine(ResetHitAnimation());
        HealthBoss.value = currentHealth;
    }

    public void Die()
    {
        Player1.LevelSlider(100);
        animator.SetBool("Death", true);
        isDeath = true;
        // Cập nhật nhiệm vụ khi boss bị tiêu diệt
        if (CompareTag("Boss") && npcQuestskill2 != null)
        {
            npcQuestskill2.DefeatFireEnemy();
        }
        Destroy(gameObject, 2f);
        PanelSkillBoss.SetActive(true);
        isPanelKillBossElite = true;
        DropItem();
    }

    public void DropItem()
    {
        if (prefabsItem != null)
        {
            Instantiate(prefabsItem, transform.position, Quaternion.identity);
        }
    }

    public void ClosePanel()
    {
        PanelSkillBoss.SetActive(false);
        isPanelKillBossElite = false;
    }

    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Hit", false);
    }
}
