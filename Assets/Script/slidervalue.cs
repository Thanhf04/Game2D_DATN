using UnityEngine;
using UnityEngine.UI;  // Để sử dụng Slider
using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;

public class PlayerDataTest12 : MonoBehaviour
{
    public Slider healthSlider;  // Slider cho Health
    public Slider energySlider;  // Slider cho Energy

    private DatabaseReference databaseReference;
    private string username;

    private void Start()
    {
        // Kiểm tra và khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError("Firebase initialization failed: " + task.Result);
            }
        });

        // Lấy username từ PlayerPrefs (hoặc gán cứng nếu muốn)
        username = PlayerPrefs.GetString("username", "defaultUser");

        // Lắng nghe sự thay đổi giá trị của các thanh slider
        healthSlider.onValueChanged.AddListener(OnHealthSliderValueChanged);
        energySlider.onValueChanged.AddListener(OnEnergySliderValueChanged);

        // Tải giá trị ban đầu từ Firebase khi game bắt đầu
        LoadSliderValueFromFirebase("HealthCurrent");
        LoadSliderValueFromFirebase("EnergyCurrent");

        // Tải min và max value của các slider từ Firebase khi game bắt đầu
        LoadSliderValueFromFirebase("HealthMin");
        LoadSliderValueFromFirebase("HealthMax");
        LoadSliderValueFromFirebase("EnergyMin");
        LoadSliderValueFromFirebase("EnergyMax");
    }

    private void OnHealthSliderValueChanged(float value)
    {
        // Mỗi khi giá trị healthSlider thay đổi, lưu vào Firebase
        SaveSliderValueToFirebase("HealthCurrent", value);
    }

    private void OnEnergySliderValueChanged(float value)
    {
        // Mỗi khi giá trị energySlider thay đổi, lưu vào Firebase
        SaveSliderValueToFirebase("EnergyCurrent", value);
    }

    // Lưu giá trị của slider vào Firebase
    private async void SaveSliderValueToFirebase(string field, float value)
    {
        if (string.IsNullOrEmpty(username) || databaseReference == null)
        {
            Debug.LogError("SaveSliderValueToFirebase failed: Username is empty or Firebase is not initialized.");
            return;
        }

        // Thêm debug để thông báo giá trị chuẩn bị được lưu vào Firebase
        Debug.Log($"Attempting to save {field} with value {value} to Firebase...");

        try
        {
            // Lưu giá trị của thanh slider vào Firebase
            await databaseReference.Child("players").Child(username).Child(field).SetValueAsync(value);

            // Thêm debug để xác nhận đã lưu thành công
            Debug.Log($"{field} successfully saved to Firebase with value: {value}");
        }
        catch (System.Exception ex)
        {
            // Nếu có lỗi khi lưu dữ liệu, log ra console
            Debug.LogError($"Error saving {field} to Firebase: {ex.Message}");
        }
    }

    // Tải lại giá trị từ Firebase và cập nhật thanh slider
    private async void LoadSliderValueFromFirebase(string field)
    {
        if (string.IsNullOrEmpty(username) || databaseReference == null)
        {
            Debug.LogError("LoadSliderValueFromFirebase failed: Username is empty or Firebase is not initialized.");
            return;
        }

        // Thêm debug để thông báo giá trị đang được tải từ Firebase
        Debug.Log($"Attempting to load {field} from Firebase...");

        try
        {
            var snapshot = await databaseReference.Child("players").Child(username).Child(field).GetValueAsync();

            if (snapshot.Exists)
            {
                // Lấy giá trị từ Firebase và cập nhật thanh slider
                float value = float.Parse(snapshot.Value.ToString());
                Debug.Log($"{field} loaded from Firebase with value: {value}");

                // Cập nhật giá trị slider
                if (field == "HealthCurrent")
                {
                    healthSlider.value = value;
                }
                else if (field == "EnergyCurrent")
                {
                    energySlider.value = value;
                }
                else if (field == "HealthMin")
                {
                    healthSlider.minValue = value;
                }
                else if (field == "HealthMax")
                {
                    healthSlider.maxValue = value;
                }
                else if (field == "EnergyMin")
                {
                    energySlider.minValue = value;
                }
                else if (field == "EnergyMax")
                {
                    energySlider.maxValue = value;
                }
            }
            else
            {
                Debug.LogWarning($"{field} does not exist in Firebase for user {username}.");
            }
        }
        catch (System.Exception ex)
        {
            // Nếu có lỗi khi tải dữ liệu, log ra console
            Debug.LogError($"Error loading {field} from Firebase: {ex.Message}");
        }
    }
}
