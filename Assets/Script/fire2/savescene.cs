//using Firebase.Database;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Firebase;
//using Firebase.Database;

//public class SceneDataSaver : MonoBehaviour
//{
//    private DatabaseReference databaseReference;
//    private string username;

//    private void Start()
//    {
//        // Kiểm tra và khởi tạo Firebase
//        InitializeFirebase();
//    }

//    private async void InitializeFirebase()
//    {
//        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
//        if (dependencyStatus == DependencyStatus.Available)
//        {
//            // Khởi tạo reference của Firebase Database sau khi Firebase đã sẵn sàng
//            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
//        }
//        else
//        {
//            Debug.LogError("Firebase không thể khởi tạo: " + dependencyStatus);
//            return;
//        }

//        // Lấy username từ PlayerPrefs
//        username = PlayerPrefs.GetString("username", "");

//        if (string.IsNullOrEmpty(username))
//        {
//            Debug.LogError("Username is empty. Cannot save scene data.");
//            return;
//        }
//    }

//    // Lưu tên scene hiện tại vào Firebase
//    public async void SaveSceneData()
//    {
//        if (string.IsNullOrEmpty(username) || databaseReference == null)
//        {
//            Debug.LogError("Username is empty or Firebase is not initialized.");
//            return;
//        }

//        string sceneName = SceneManager.GetActiveScene().name;
//        await databaseReference.Child("players").Child(username).Child("Scene").SetValueAsync(sceneName);
//    }

//    // Cập nhật và lưu tên scene khi scene thay đổi
//    private void OnEnable()
//    {
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    private void OnDisable()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }

//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        SaveSceneData();
//    }
//}
