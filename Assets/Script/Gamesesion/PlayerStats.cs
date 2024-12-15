using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    // Các chỉ số của người chơi
    public int hp = 0;
    public int mana = 0;
    public float exp = 0f;
    public float damage = 10f;
    public int gold = 100;

    // Các chỉ số tối đa
    public int maxHp = 100;
    public int maxMana = 100;
    public float maxExp = 100f;

    // Đảm bảo dữ liệu được giữ lại khi chuyển scene
    public static PlayerStats Instance;

    // nhiệm vụ
    public bool hasShownCongratulation = false;
    Dichuyennv1 dichuyennv1;

    private void Awake()
    {
        // Singleton pattern để duy trì một instance duy nhất của PlayerStats
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ object khi chuyển scene
        }
        else
        {
            Destroy(gameObject); // Hủy nếu đã có instance
        }
    }

    // Cập nhật các chỉ số và slider
    public void UpdateStats(int newHp, int newMana, float newExp, float newDamage, int newGold)
    {
        hp = newHp;
        mana = newMana;
        exp = newExp;
        damage = newDamage;
        gold = newGold;
    }

    // Lưu các chỉ số vào PlayerPrefs
    public void SaveStats()
    {
        PlayerPrefs.SetInt("HP", hp);
        PlayerPrefs.SetInt("Mana", mana);
        PlayerPrefs.SetFloat("Exp", exp);
        PlayerPrefs.SetFloat("Damage", damage);
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("MaxHP", maxHp); // Lưu maxHp
        PlayerPrefs.SetInt("MaxMana", maxMana); // Lưu maxMana
        PlayerPrefs.SetFloat("MaxExp", maxExp); // Lưu maxExp
        PlayerPrefs.SetInt("HasShownCongratulation", hasShownCongratulation ? 1 : 0);
        PlayerPrefs.Save(); // Lưu ngay lập tức
    }

    // Tải các chỉ số từ PlayerPrefs
    public void LoadStats()
    {
        hp = PlayerPrefs.GetInt("HP", hp); // Nếu không có, sử dụng giá trị mặc định
        mana = PlayerPrefs.GetInt("Mana", mana);
        exp = PlayerPrefs.GetFloat("Exp", exp);
        damage = PlayerPrefs.GetFloat("Damage", damage);
        gold = PlayerPrefs.GetInt("Gold", gold);

        maxHp = PlayerPrefs.GetInt("MaxHP", maxHp); // Tải maxHp
        maxMana = PlayerPrefs.GetInt("MaxMana", maxMana); // Tải maxMana
        maxExp = PlayerPrefs.GetFloat("MaxExp", maxExp); // Tải maxExp
        hasShownCongratulation = PlayerPrefs.GetInt("HasShownCongratulation", 0) == 1;
    }
}
