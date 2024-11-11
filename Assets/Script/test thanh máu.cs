using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthFill; // Để tham chiếu đến Image của HealthFill
    private Dichuyennv1 dichuyennv1; 

    void Start()
    {
        dichuyennv1 = GetComponentInParent<Dichuyennv1>();
        UpdateHealthBar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            IncreaseHealth(10); // Tăng 10 máu
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DecreaseHealth(10); // Giảm 10 máu
        }
    }

    public void IncreaseHealth(int amount)
    {

        dichuyennv1.currentHealth += amount;
        if (dichuyennv1.currentHealth > dichuyennv1.maxHealth)
        {
            dichuyennv1.currentHealth = dichuyennv1.maxHealth; // Không vượt quá máu tối đa
        }
        UpdateHealthBar();
    }

    public void DecreaseHealth(int amount)
    {
        dichuyennv1.currentHealth -= amount;
        if (dichuyennv1.currentHealth < 0)
        {
            dichuyennv1.currentHealth = 0; // Không vượt quá 0 máu
        }
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthFill.fillAmount = dichuyennv1.currentHealth / dichuyennv1.maxHealth; // Cập nhật thanh máu
    }
}
