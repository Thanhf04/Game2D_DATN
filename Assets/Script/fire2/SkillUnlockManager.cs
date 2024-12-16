using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class SkillUnlockManager : MonoBehaviour
{
    // Tham chiếu đến GameObject ổ khóa của skill bạn muốn quản lý
    public GameObject lockImageSkill1; // Chỉ quản lý 1 kỹ năng này

    // Biến lưu trạng thái mở khóa của skill 1
    private bool isSkill1Unlocked = false;

    // Tham chiếu tới Firebase Database
    private DatabaseReference databaseReference;

    // ID của khóa kỹ năng trong Firebase
    private string skill1Id = "skill_1";

    // Vị trí mở khóa của kỹ năng 1
    private Vector3 skill1UnlockPosition = new Vector3(151.1985f, 17.11524f, 0f);

    // Khoảng cách dung sai để xác định xem người chơi đã đến vị trí mục tiêu chưa
    private float positionTolerance = 0.5f;

    // Tên người chơi
    private string username;

    void Start()
    {
        // Khởi tạo Firebase
        InitializeFirebase();

        // Lấy tên người chơi từ PlayerPrefs (hoặc một giá trị mặc định nếu không có)
        username = PlayerPrefs.GetString("username", "defaultUsername");

        // Kiểm tra trạng thái mở khóa kỹ năng 1 khi game bắt đầu
        CheckSkillUnlockStatus(skill1Id);
    }

    void InitializeFirebase()
    {
        // Lấy tham chiếu đến Firebase Realtime Database
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void CheckSkillUnlockStatus(string skillId)
    {
        // Lấy trạng thái mở khóa skill từ Firebase
        FirebaseDatabase.DefaultInstance
            .GetReference("skills/" + skillId)  // Lấy trạng thái của khóa từ Firebase
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // Nếu trạng thái là "unlocked", thì hình ảnh ổ khóa bị ẩn
                        isSkill1Unlocked = snapshot.Value.ToString() == "unlocked";
                        UpdateLockImageVisibility(lockImageSkill1, isSkill1Unlocked);
                    }
                }
            });
    }

    void UpdateLockImageVisibility(GameObject lockImage, bool isUnlocked)
    {
        // Cập nhật hiển thị hình ảnh ổ khóa dựa trên trạng thái
        if (lockImage != null)
        {
            lockImage.SetActive(!isUnlocked); // Nếu skill đã unlocked, ổ khóa sẽ ẩn
        }
    }

    void Update()
    {
        // Kiểm tra nếu nhân vật đã đến gần vị trí unlockPosition của skill 1
        if (!isSkill1Unlocked && Vector3.Distance(transform.position, skill1UnlockPosition) < positionTolerance)
        {
            UnlockSkill(skill1Id);
        }
    }

    void UnlockSkill(string skillId)
    {
        // Mở khóa skill và ẩn hình ảnh ổ khóa
        isSkill1Unlocked = true;
        UpdateFirebaseUnlockStatus(skillId, "unlocked");
        UpdateLockImageVisibility(lockImageSkill1, isSkill1Unlocked);

        // Lưu trạng thái mở khóa vào PlayerPrefs
        SaveSkillUnlockStatusToPlayerPrefs();
    }

    void UpdateFirebaseUnlockStatus(string skillId, string status)
    {
        // Lưu trạng thái mở khóa skill vào Firebase
        databaseReference.Child("skills").Child(skillId).SetValueAsync(status);
    }

    void SaveSkillUnlockStatusToPlayerPrefs()
    {
        // Lưu trạng thái mở khóa của kỹ năng vào PlayerPrefs
        PlayerPrefs.SetString("username", username);  // Lưu tên người chơi vào PlayerPrefs
        PlayerPrefs.SetInt("skill1Status", isSkill1Unlocked ? 1 : 0);  // Lưu trạng thái mở khóa (1 = unlocked, 0 = locked)
        PlayerPrefs.Save();  // Lưu lại tất cả dữ liệu vào PlayerPrefs
    }

    void LoadSkillUnlockStatusFromPlayerPrefs()
    {
        // Tải trạng thái kỹ năng từ PlayerPrefs
        username = PlayerPrefs.GetString("username", "defaultUsername");
        int skill1Status = PlayerPrefs.GetInt("skill1Status", 0);  // 0 = locked, 1 = unlocked
        isSkill1Unlocked = (skill1Status == 1);

        // Cập nhật hiển thị hình ảnh ổ khóa
        UpdateLockImageVisibility(lockImageSkill1, isSkill1Unlocked);
    }
}
