using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class SliderMinMaxData : MonoBehaviour
{
    public Slider healthSlider;  // Slider cho health
    public Slider energySlider;  // Slider cho energy

    private DatabaseReference databaseReference;
    private string username;

    private void Start()
    {
        // Kiểm tra và khởi tạo Firebase
        InitializeFirebase();

        // Lấy username từ PlayerPrefs
        username = PlayerPrefs.GetString("username", "");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is empty. Cannot load slider data.");
            return;
        }

        // Đăng ký sự kiện thay đổi giá trị min/max slider
        if (healthSlider != null)
        {
            healthSlider.onValueChanged.AddListener((value) => SaveSliderMinMax());
        }

        if (energySlider != null)
        {
            energySlider.onValueChanged.AddListener((value) => SaveSliderMinMax());
        }

        // Tải min/max của các slider từ Firebase
        LoadSliderMinMaxFromFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    // Lưu giá trị min/max của các slider vào Firebase
    private async void SaveSliderMinMax()
    {
        if (string.IsNullOrEmpty(username) || databaseReference == null)
        {
            Debug.LogError("SaveSliderMinMax failed: Username is empty or Firebase is not initialized.");
            return;
        }

        // Lưu min/max cho healthSlider vào Firebase
        if (healthSlider != null)
        {
            await databaseReference.Child("players").Child(username).Child("HealthSliderMin").SetValueAsync(healthSlider.minValue);
            await databaseReference.Child("players").Child(username).Child("HealthSliderMax").SetValueAsync(healthSlider.maxValue);
        }

        // Lưu min/max cho energySlider vào Firebase
        if (energySlider != null)
        {
            await databaseReference.Child("players").Child(username).Child("EnergySliderMin").SetValueAsync(energySlider.minValue);
            await databaseReference.Child("players").Child(username).Child("EnergySliderMax").SetValueAsync(energySlider.maxValue);
        }
    }

    // Tải min/max của các slider từ Firebase
    private async void LoadSliderMinMaxFromFirebase()
    {
        if (string.IsNullOrEmpty(username) || databaseReference == null)
        {
            Debug.LogError("LoadSliderMinMax failed: Username is empty or Firebase is not initialized.");
            return;
        }

        var snapshot = await databaseReference.Child("players").Child(username).GetValueAsync();

        if (snapshot.Exists)
        {
            // Lấy min/max cho healthSlider từ Firebase
            if (snapshot.Child("HealthSliderMin").Exists && snapshot.Child("HealthSliderMax").Exists)
            {
                float healthSliderMin = float.Parse(snapshot.Child("HealthSliderMin").Value.ToString());
                float healthSliderMax = float.Parse(snapshot.Child("HealthSliderMax").Value.ToString());

                if (healthSlider != null)
                {
                    healthSlider.minValue = healthSliderMin;
                    healthSlider.maxValue = healthSliderMax;
                }
            }

            // Lấy min/max cho energySlider từ Firebase
            if (snapshot.Child("EnergySliderMin").Exists && snapshot.Child("EnergySliderMax").Exists)
            {
                float energySliderMin = float.Parse(snapshot.Child("EnergySliderMin").Value.ToString());
                float energySliderMax = float.Parse(snapshot.Child("EnergySliderMax").Value.ToString());

                if (energySlider != null)
                {
                    energySlider.minValue = energySliderMin;
                    energySlider.maxValue = energySliderMax;
                }
            }
        }
        else
        {
            Debug.LogWarning("No slider min/max data found for player: " + username);
        }
    }

    // Gọi phương thức này để lưu lại min/max khi cần
    private void OnApplicationQuit()
    {
        SaveSliderMinMax();
    }
}
