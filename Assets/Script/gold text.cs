using System; // Để sử dụng Exception và các kiểu dữ liệu như Convert
using System.Collections.Generic; // Để sử dụng Dictionary
using Firebase.Database;
using TMPro;
using UnityEngine;

public class Player3 : MonoBehaviour
{
    public TextMeshProUGUI goldText; // Text để hiển thị gold

    private DatabaseReference databaseReference;
    private string username;

    private void Start()
    {
        username = PlayerPrefs.GetString("username", ""); // Lấy username từ PlayerPrefs
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không hợp lệ!");
            return;
        }

        // Lấy dữ liệu nhân vật từ Firebase
        LoadCharacterData();
    }

    private async void LoadCharacterData()
    {
        try
        {
            var snapshot = await databaseReference
                .Child("characters")
                .Child(username)
                .GetValueAsync();

            if (snapshot.Exists)
            {
                var characterData = snapshot.Value as Dictionary<string, object>;

                if (characterData != null && characterData.ContainsKey("gold"))
                {
                    // Cập nhật UI cho gold (Sử dụng float.TryParse để an toàn)
                    if (float.TryParse(characterData["gold"].ToString(), out float gold))
                    {
                        goldText.text = "Gold: " + gold.ToString();
                    }
                    else
                    {
                        Debug.LogError("Giá trị gold không hợp lệ.");
                    }
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy thông tin gold trong dữ liệu nhân vật.");
                }
            }
            else
            {
                Debug.LogError("Dữ liệu nhân vật không tồn tại trên Firebase.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Có lỗi khi tải dữ liệu nhân vật: " + ex.Message);
        }
    }

    public async void UpdateGold(float newGoldValue)
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không hợp lệ.");
            return;
        }

        // Cập nhật giá trị gold trên Firebase
        try
        {
            await databaseReference
                .Child("characters")
                .Child(username)
                .Child("gold")
                .SetValueAsync(newGoldValue);
            Debug.Log("Gold đã được cập nhật trên Firebase.");

            // Sau khi cập nhật Firebase, cập nhật lại UI
            goldText.text = "Gold: " + newGoldValue.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Có lỗi khi cập nhật gold: " + ex.Message);
        }
    }
}
