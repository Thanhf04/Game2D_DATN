using System;
using System.Threading.Tasks;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class CharacterEditManager : MonoBehaviour
{
    public TMP_InputField healthInput;     // Input field cho máu
    public TMP_InputField energyInput;     // Input field cho năng lượng
    public TMP_InputField goldInput;       // Input field cho vàng
    public TMP_InputField diamondInput;    // Input field cho kim cương
    public TMP_InputField positionXInput;  // Input field cho tọa độ X
    public TMP_InputField positionYInput;  // Input field cho tọa độ Y
    public TMP_InputField positionZInput;  // Input field cho tọa độ Z
    public TextMeshProUGUI feedbackText;   // Text để hiển thị thông báo

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
            feedbackText.text = "Vui lòng đăng nhập trước.";
            return; // Dừng nếu không có username
        }

        await LoadCharacterData(); // Gọi phương thức để tải dữ liệu
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
                        // Điền dữ liệu vào input fields
                        healthInput.text = characterData.health.ToString();
                        energyInput.text = characterData.energy.ToString();
                        goldInput.text = characterData.gold.ToString();
                        diamondInput.text = characterData.diamond.ToString();
                        positionXInput.text = characterData.position.x.ToString();
                        positionYInput.text = characterData.position.y.ToString();
                        positionZInput.text = characterData.position.z.ToString();
                    }
                    else
                    {
                        Debug.LogWarning("Dữ liệu nhân vật không hợp lệ!");
                        feedbackText.text = "Dữ liệu nhân vật không hợp lệ!";
                    }
                }
                else
                {
                    feedbackText.text = "Không tìm thấy dữ liệu nhân vật!";
                }
            }
            else
            {
                feedbackText.text = "Không tìm thấy dữ liệu nhân vật!";
            }
        }
        catch (Exception ex)
        {
            feedbackText.text = "Lỗi khi tải dữ liệu: " + ex.Message;
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
            // Lấy dữ liệu từ input fields
            float health = float.Parse(healthInput.text);
            float energy = float.Parse(energyInput.text);
            int gold = int.Parse(goldInput.text);
            int diamond = int.Parse(diamondInput.text);
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

            feedbackText.text = "Dữ liệu nhân vật đã được cập nhật thành công!";
        }
        catch (FormatException ex)
        {
            feedbackText.text = "Dữ liệu đầu vào không hợp lệ!";
            Debug.LogError("Lỗi định dạng đầu vào: " + ex);
        }
        catch (Exception ex)
        {
            feedbackText.text = "Lỗi khi lưu dữ liệu: " + ex.Message;
            Debug.LogError("Lỗi khi lưu dữ liệu: " + ex);
        }
    }
}
