using System;
using System.Threading.Tasks;  // Thêm dòng này
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Player1 : MonoBehaviour
{
    public Image healthBar;            // Image hiển thị thanh health
    public Image energyBar;            // Image hiển thị thanh energy
    public TextMeshProUGUI goldText;   // Hiển thị gold
    public TextMeshProUGUI diamondText; // Hiển thị diamond
    public TextMeshProUGUI expText;    // Hiển thị exp
    public TextMeshProUGUI enemyPointsText; // Hiển thị điểm enemy
    public TextMeshProUGUI levelText;  // Hiển thị level
    public TextMeshProUGUI damageText; // Hiển thị damage (sát thương)

    private DatabaseReference databaseReference;
    private string username;

    private void Start()
    {
        username = PlayerPrefs.GetString("username", "");  // Lấy username từ PlayerPrefs
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        LoadCharacterData();
    }

    private async void LoadCharacterData()
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không hợp lệ.");
            return;
        }

        var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
        if (snapshot.Exists)
        {
            var characterData = snapshot.Value as Dictionary<string, object>;
            if (characterData != null)
            {
                float healthMax = Convert.ToSingle(characterData["healthMax"]);
                float healthCurrent = Convert.ToSingle(characterData["healthCurrent"]);
                float energyMax = Convert.ToSingle(characterData["energyMax"]);
                float energyCurrent = Convert.ToSingle(characterData["energyCurrent"]);

                // Cập nhật thanh health
                healthBar.fillAmount = healthCurrent / healthMax;

                // Cập nhật thanh energy
                energyBar.fillAmount = energyCurrent / energyMax;

                goldText.text = "Gold: " + characterData["gold"].ToString();
                diamondText.text = "Diamond: " + characterData["diamond"].ToString();
                expText.text = "EXP: " + characterData["exp"].ToString();
                enemyPointsText.text = "Enemy Points: " + characterData["enemyPoints"].ToString();
                levelText.text = "Level: " + characterData["level"].ToString();
                damageText.text = "Damage: " + characterData["damage"].ToString();
            }
        }
        else
        {
            Debug.LogError("Dữ liệu nhân vật không tồn tại.");
        }
    }

    // Hàm này được gọi khi nhân vật nhận sát thương
    public async void TakeDamage(float damage)
    {
        var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
        if (snapshot.Exists)
        {
            var characterData = snapshot.Value as Dictionary<string, object>;
            float healthCurrent = Convert.ToSingle(characterData["healthCurrent"]);
            healthCurrent -= damage;

            if (healthCurrent <= 0)
            {
                healthCurrent = 0;
                Debug.Log("Nhân vật đã chết!");
            }

            // Cập nhật trực tiếp vào Firebase
            await databaseReference.Child("characters").Child(username).Child("healthCurrent").SetValueAsync(healthCurrent);

            // Cập nhật thanh health
            healthBar.fillAmount = healthCurrent / Convert.ToSingle(characterData["healthMax"]);

            // Cập nhật lại UI Health
            TextMeshProUGUI healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
            healthText.text = "Health: " + healthCurrent;

            // Lưu lại giá trị mới vào Firebase
            await SaveCharacterData(characterData);  // Lưu lại các thay đổi
        }
    }

    // Hàm này được gọi khi nhân vật nhận năng lượng (energy)
    public async void UseEnergy(float energyUsed)
    {
        var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
        if (snapshot.Exists)
        {
            var characterData = snapshot.Value as Dictionary<string, object>;
            float energyCurrent = Convert.ToSingle(characterData["energyCurrent"]);
            energyCurrent -= energyUsed;

            if (energyCurrent < 0)
                energyCurrent = 0;

            // Cập nhật trực tiếp vào Firebase
            await databaseReference.Child("characters").Child(username).Child("energyCurrent").SetValueAsync(energyCurrent);

            // Cập nhật thanh energy
            energyBar.fillAmount = energyCurrent / Convert.ToSingle(characterData["energyMax"]);

            // Cập nhật lại UI Energy
            TextMeshProUGUI energyText = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
            energyText.text = "Energy: " + energyCurrent;

            // Lưu lại giá trị mới vào Firebase
            await SaveCharacterData(characterData);  // Lưu lại các thay đổi
        }
    }

    // Lưu lại dữ liệu nhân vật lên Firebase
    private async Task SaveCharacterData(Dictionary<string, object> characterData)
    {
        // Cập nhật các giá trị mới trong characterData
        characterData["healthCurrent"] = healthBar.fillAmount * Convert.ToSingle(characterData["healthMax"]);
        characterData["energyCurrent"] = energyBar.fillAmount * Convert.ToSingle(characterData["energyMax"]);

        try
        {
            // Lưu lại thông tin vào Firebase
            await databaseReference.Child("characters").Child(username).SetValueAsync(characterData);
            Debug.Log("Dữ liệu đã được lưu lên Firebase.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Có lỗi khi lưu dữ liệu lên Firebase: " + ex.Message);
        }
    }
}
