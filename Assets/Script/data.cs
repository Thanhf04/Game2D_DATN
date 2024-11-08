using System;
using System.Threading.Tasks;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // Để sử dụng Image và fillAmount

public class CharacterEditManager : MonoBehaviour
{
    // Tạm thời bỏ qua TMP_InputField cho health và feedbackText
    // public TMP_InputField healthInput;     // Input field cho máu
    public Image healthImage;      // Image hiển thị health (dưới dạng thanh trạng thái)
    public Image energyImage;      // Image hiển thị energy (dưới dạng thanh trạng thái)

    public TextMeshProUGUI goldText;       // Text hiển thị gold (dưới dạng text)
    public TMP_InputField positionXInput;  // Input field cho tọa độ X
    public TMP_InputField positionYInput;  // Input field cho tọa độ Y
    public TMP_InputField positionZInput;  // Input field cho tọa độ Z
    public TextMeshProUGUI diamondText;    // Text hiển thị kim cương

    private DatabaseReference databaseReference;
    private string username;

    private async void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Lấy tên người dùng từ PlayerPrefs
        username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không được tìm thấy trong PlayerPrefs!");
            // feedbackText.text = "Vui lòng đăng nhập trước."; // Tạm thời bỏ qua
            return; // Dừng nếu không có username
        }

        // Tải dữ liệu từ Firebase và cũng đọc lại giá trị lưu trữ từ PlayerPrefs
        await LoadCharacterData();
        LoadHealthAndEnergyFromPlayerPrefs();  // Đọc lại giá trị health và energy
    }

    private async Task LoadCharacterData()
    {
        if (databaseReference == null)
        {
            Debug.LogError("DatabaseReference là null! Vui lòng kiểm tra cài đặt Firebase.");
            return;
        }

        try
        {
            var snapshot = await databaseReference.Child("characters").Child(username).GetValueAsync();
            if (snapshot.Exists)
            {
                string jsonValue = snapshot.GetRawJsonValue();
                if (!string.IsNullOrEmpty(jsonValue))
                {
                    CharacterData characterData = JsonUtility.FromJson<CharacterData>(jsonValue);

                    if (characterData != null)
                    {
                        // Tạm thời bỏ qua việc điền vào healthInput và feedbackText
                        // healthInput.text = characterData.health.ToString();
                        goldText.text = characterData.gold.ToString();   // Hiển thị gold dưới dạng text
                        diamondText.text = characterData.diamond.ToString(); // Hiển thị kim cương dưới dạng text
                        positionXInput.text = characterData.position.x.ToString();
                        positionYInput.text = characterData.position.y.ToString();
                        positionZInput.text = characterData.position.z.ToString();

                        // Hiển thị health và energy dưới dạng thanh trạng thái
                        healthImage.fillAmount = characterData.health / 100f;   // Giả sử max health là 100
                        energyImage.fillAmount = characterData.energy / 100f;   // Giả sử max energy là 100
                    }
                    else
                    {
                        Debug.LogWarning("Dữ liệu nhân vật không hợp lệ!");
                        // feedbackText.text = "Dữ liệu nhân vật không hợp lệ!"; // Tạm thời bỏ qua
                    }
                }
                else
                {
                    // feedbackText.text = "Không tìm thấy dữ liệu nhân vật!"; // Tạm thời bỏ qua
                }
            }
            else
            {
                // feedbackText.text = "Không tìm thấy dữ liệu nhân vật!"; // Tạm thời bỏ qua
            }
        }
        catch (Exception ex)
        {
            // feedbackText.text = "Lỗi khi tải dữ liệu: " + ex.Message; // Tạm thời bỏ qua
            Debug.LogError("Lỗi khi tải dữ liệu: " + ex);
        }
    }

    public void OnSaveButtonClicked()
    {
        SaveCharacterData();
    }

    public async void SaveCharacterData()
    {
        try
        {
            // Lấy dữ liệu từ các thanh trạng thái Image
            float health = healthImage.fillAmount * 100f;  // Health lấy từ fillAmount (tính ra giá trị)
            float energy = energyImage.fillAmount * 100f;  // Energy lấy từ fillAmount (tính ra giá trị)
            int gold = int.Parse(goldText.text);            // Gold lấy từ goldText (hiển thị text)
            int diamond = int.Parse(diamondText.text);     // Diamond từ diamondText (hiển thị text)
            float positionX = float.Parse(positionXInput.text);
            float positionY = float.Parse(positionYInput.text);
            float positionZ = float.Parse(positionZInput.text);

            // Tạo đối tượng CharacterData
            CharacterData characterData = new CharacterData
            {
                username = username,
                avatar = "path/to/avatar.png", // Có thể thay đổi nếu cần
                health = health,
                energy = energy,
                gold = gold,
                diamond = diamond,
                position = new Vector3(positionX, positionY, positionZ),
                skillID = "skill_01" // Có thể thay đổi nếu cần
            };

            string jsonData = JsonUtility.ToJson(characterData);
            var task = databaseReference.Child("characters").Child(username).SetRawJsonValueAsync(jsonData);
            await task;

            // Lưu giá trị fillAmount của Health và Energy vào PlayerPrefs
            SaveHealthAndEnergyToPlayerPrefs();

            // feedbackText.text = "Dữ liệu nhân vật đã được cập nhật thành công!"; // Tạm thời bỏ qua
        }
        catch (FormatException ex)
        {
            // feedbackText.text = "Dữ liệu đầu vào không hợp lệ!"; // Tạm thời bỏ qua
            Debug.LogError("Lỗi định dạng đầu vào: " + ex);
        }
        catch (Exception ex)
        {
            // feedbackText.text = "Lỗi khi lưu dữ liệu: " + ex.Message; // Tạm thời bỏ qua
            Debug.LogError("Lỗi khi lưu dữ liệu: " + ex);
        }
    }

    // Lưu giá trị health và energy vào PlayerPrefs
    private void SaveHealthAndEnergyToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("HealthFillAmount", healthImage.fillAmount);
        PlayerPrefs.SetFloat("EnergyFillAmount", energyImage.fillAmount);
        PlayerPrefs.Save();
    }

    // Tải lại giá trị health và energy từ PlayerPrefs
    private void LoadHealthAndEnergyFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("HealthFillAmount"))
        {
            float savedHealth = PlayerPrefs.GetFloat("HealthFillAmount");
            healthImage.fillAmount = savedHealth;
        }

        if (PlayerPrefs.HasKey("EnergyFillAmount"))
        {
            float savedEnergy = PlayerPrefs.GetFloat("EnergyFillAmount");
            energyImage.fillAmount = savedEnergy;
        }
    }
}
