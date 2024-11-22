using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    public int Health = 100;
    public int Mana = 100;
    public int Damage = 10;

    public int MaxHealth = 100;
    public int MaxMana = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateHealth(int value)
    {
        Health = Mathf.Clamp(Health + value, 0, MaxHealth);
    }

    public void UpdateMana(int value)
    {
        Mana = Mathf.Clamp(Mana + value, 0, MaxMana);
    }

    public void UpdateDamage(int value)
    {
        Damage += value;
    }
}
