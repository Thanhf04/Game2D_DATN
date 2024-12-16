using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int maxMana = 100;
    public int currentMana = 100;
    public float expMax = 100;
    public float expCurrent = 0;
    public int damageAmount = 10;
    // public int level = 1;
    public int upgradePoints = 5;

    private void Awake()
    {
        int numbersession = FindObjectsOfType<GameSession>().Length;
        if (numbersession > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadGameSession();
    }

    void Update()
    {
        healthSlider.value = (float)currentHealth / maxHealth;
        manaSlider.value = (float)currentMana / maxMana;
        expSlider.value = expCurrent / expMax;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        // Cập nhật thanh máu trong UI (nếu có)
        healthSlider.value = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        // Reset health and mana to initial values when the player dies
        currentHealth = maxHealth;
        currentMana = maxMana;
        // Optionally, reset experience, upgrade points, or other values
        expCurrent = 0;
        upgradePoints = 5; // Reset upgrade points or keep it based on your game logic

        // Optionally, display a "Game Over" message or reset other game elements
        Debug.Log("Player has died. Health and Mana have been reset.");
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void GainMana(int manaAmount)
    {
        currentMana += manaAmount;
        if (currentMana > maxMana)
            currentMana = maxMana;
    }

    // public void GainExperience(float exp)
    // {
    //     expCurrent += exp;
    //     if (expCurrent >= expMax)
    //     {
    //         LevelUp();
    //     }
    // }

    // private void LevelUp()
    // {
    //     level++;
    //     upgradePoints++;
    //     expCurrent -= expMax;
    //     expMax *= 1.2f;
    // }

    public void SaveGameSession()
    {
        PlayerPrefs.SetInt("CurrentHealth", currentHealth);
        PlayerPrefs.SetInt("CurrentMana", currentMana);
        PlayerPrefs.SetFloat("ExpCurrent", expCurrent);
        PlayerPrefs.SetInt("DamageAmount", damageAmount);
        PlayerPrefs.SetInt("UpgradePoints", upgradePoints);
        // PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.Save();
    }

    public void LoadGameSession()
    {
        currentHealth = PlayerPrefs.GetInt("CurrentHealth", maxHealth);
        currentMana = PlayerPrefs.GetInt("CurrentMana", maxMana);

        expCurrent = PlayerPrefs.GetFloat("ExpCurrent", 0);
        damageAmount = PlayerPrefs.GetInt("DamageAmount", 10);
        upgradePoints = PlayerPrefs.GetInt("UpgradePoints", 5);
        // level = PlayerPrefs.GetInt("Level", 1);
        healthSlider.value = currentHealth;
        
    }

    void OnApplicationQuit()
    {
        SaveGameSession();
    }
}
