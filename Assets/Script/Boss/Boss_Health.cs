using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss_Health : MonoBehaviour
{
    [SerializeField]
    private Slider HealthBoss;

    [SerializeField]
    private GameObject Boss;
    public int maxHealth = 100;
    public int currentHealth;
    Animator animator;
    public GameObject prefabsItem;
    public GameObject PanelSkillBoss;
    Dichuyennv1 Player1;
    public static bool isPanelKillDeathBoss = false;
    void Start()
    {
        HealthBoss.maxValue = maxHealth;
        HealthBoss.value = maxHealth;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        Player1 = FindObjectOfType<Dichuyennv1>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
        Destroy(gameObject, 2f);
        PanelSkillBoss.SetActive(true);
        isPanelKillDeathBoss = true;
        SceneManager.LoadScene(6);
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
        isPanelKillDeathBoss = false;
    }

    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Hit", false);
    }
}
