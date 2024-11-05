using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthFill; // Để tham chiếu đến Image của HealthFill
    public float maxHealth = 100f; // Máu tối đa
    private float currentHealth; // Máu hiện tại

    void Start()
    {
        currentHealth = maxHealth; // Khởi tạo máu hiện tại
        UpdateHealthBar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            IncreaseHealth(10); // Tăng 10 máu
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DecreaseHealth(10); // Giảm 10 máu
        }
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Không vượt quá máu tối đa
        }
        UpdateHealthBar();
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0; // Không vượt quá 0 máu
        }
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthFill.fillAmount = currentHealth / maxHealth; // Cập nhật thanh máu
    }
}
