//using UnityEngine;

//public class GameManager : MonoBehaviour
//{
//    // Biến Singleton duy nhất cho GameManager
//    public static GameManager Instance { get; private set; }

//    // Dữ liệu của người chơi
//    private PlayerData currentPlayerData;

//    // Khởi tạo GameManager
//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);  // Đảm bảo GameManager không bị hủy khi chuyển cảnh
//        }
//        else
//        {
//            Destroy(gameObject);  // Hủy GameManager trùng lặp nếu đã có
//        }
//    }

//    // Hàm để lưu PlayerData
//    public void SetPlayerData(PlayerData playerData)
//    {
//        currentPlayerData = playerData;
//        Debug.Log("Player data set: " + playerData.diamond);
//    }

//    // Hàm để lấy PlayerData
//    public PlayerData GetPlayerData()
//    {
//        return currentPlayerData;
//    }
//}
