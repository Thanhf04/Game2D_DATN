using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsManager : MonoBehaviour
{
    // Các biến liên quan đến máu, mana, và sát thương
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;
    public Text healthText;
    public Text manaText;
    public Text damaText;
    public Text levelText;
    public Text pointsText;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textExp;

    // Các biến về stats
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMana = 100;
    public int currentMana;
    public int damageAmount = 10;
    public int level = 1;
    public float expMax = 100;
    public float expCurrent = 0;
    public int upgradePoints = 5;

    // Các button tăng/giảm stats
    public Button increaseHealthButton;
    public Button decreaseHealthButton;
    public Button increaseManaButton;
    public Button decreaseManaButton;
    public Button increaseDamethButton;
    public Button decreaseDamethButton;

    private void Start()
    {
        // Gán các sự kiện cho các nút
        increaseHealthButton.onClick.AddListener(IncreaseHealth);
        decreaseHealthButton.onClick.AddListener(DecreaseHealth);
        increaseManaButton.onClick.AddListener(IncreaseMana);
        decreaseManaButton.onClick.AddListener(DecreaseMana);
        increaseDamethButton.onClick.AddListener(IncreaseDamage);
        decreaseDamethButton.onClick.AddListener(DecreaseDamage);

        // Khởi tạo các giá trị ban đầu cho stats
        currentHealth = maxHealth;
        currentMana = maxMana;
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;
        expSlider.maxValue = expMax;
        UpdateStatsText();
    }

    // Các phương thức để tăng/giảm stats
    void IncreaseHealth()
    {
        if (upgradePoints > 0)
        {
            maxHealth += 100;
            currentHealth = Mathf.Min(maxHealth, currentHealth + 100);
            healthSlider.maxValue = maxHealth;
            upgradePoints--;
            UpdateStatsText();
        }
        else
        {
            ShowNotification("Bạn đã hết điểm nâng cấp!");
        }
    }

    void DecreaseHealth()
    {
        if (currentHealth > 1 && upgradePoints < level + 5)
        {
            maxHealth = Mathf.Max(100, maxHealth - 100);
            healthSlider.maxValue = maxHealth;
            currentHealth = Mathf.Clamp(currentHealth - 100, 1, maxHealth);
            upgradePoints++;
            UpdateStatsText();
        }
    }

    void IncreaseMana()
    {
        if (upgradePoints > 0)
        {
            maxMana += 100;
            currentMana = Mathf.Min(maxMana, currentMana + 50);
            manaSlider.maxValue = maxMana;
            upgradePoints--;
            UpdateStatsText();
        }
        else
        {
            ShowNotification("Bạn đã hết điểm nâng cấp!");
        }
    }

    void DecreaseMana()
    {
        if (currentMana > 1 && upgradePoints < level + 5)
        {
            maxMana = Mathf.Max(100, maxMana - 100);
            manaSlider.maxValue = maxMana;
            currentMana = Mathf.Clamp(currentMana - 100, 1, maxMana);
            upgradePoints++;
            UpdateStatsText();
        }
    }

    void IncreaseDamage()
    {
        if (upgradePoints > 0)
        {
            damageAmount += 10;
            upgradePoints--;
            UpdateStatsText();
        }
        else
        {
            ShowNotification("Bạn đã hết điểm nâng cấp!");
        }
    }

    void DecreaseDamage()
    {
        if (damageAmount > 0 && upgradePoints < level + 5)
        {
            damageAmount -= 10;
            upgradePoints++;
            UpdateStatsText();
        }
    }

    // Cập nhật thông tin các chỉ số
    void UpdateStatsText()
    {
        healthText.text = ((maxHealth - 100) / 100).ToString();
        manaText.text = ((maxMana - 100) / 100).ToString();
        damaText.text = ((damageAmount - 10) / 10).ToString();
        levelText.text = "Level: " + level;
        pointsText.text = "Points: " + upgradePoints;
        textLevel.SetText("Lv" + level);
        textExp.SetText(expCurrent + "%");
    }

    // Hiển thị thông báo
    void ShowNotification(string message)
    {
        // You can use any UI text field or a custom notification pop-up for this.
        Debug.Log(message); // Just log the message for now.
    }

    // Cập nhật thanh tiến trình exp
    public void LevelSlider(float amount)
    {
        expCurrent += amount;
        textExp.SetText(expCurrent + "%");
        if (expCurrent >= expMax)
        {
            expCurrent = 0;
            textExp.SetText(expCurrent + "%");
            level++;
            textLevel.SetText("Lv" + level);
            upgradePoints++;
            UpdateStatsText();
        }
        expSlider.value = expCurrent;
    }
}
