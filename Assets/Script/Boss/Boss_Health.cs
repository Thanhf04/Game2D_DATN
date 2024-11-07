using UnityEngine;
using UnityEngine.UI;

public class Boss_Health : MonoBehaviour
{
    [SerializeField] private Slider HealthBoss;
    [SerializeField] private GameObject Boss;
    public int maxHealth = 100;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        HealthBoss.maxValue = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHealth < 0)
        {
            Die();
        }
        animator.SetBool("Death", false);
        HealthBoss.value = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        if (maxHealth > 0)
        {
            return;
        }
        maxHealth -= damage;
    }
    public void Die()
    {
        animator.SetBool("Death", true);
        Destroy(Boss, 2f);
    }
}
