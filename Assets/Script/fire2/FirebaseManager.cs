﻿using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public class FirebaseManager1 : MonoBehaviour
{
    private DatabaseReference reference;
    private bool isFirebaseInitialized = false;

    void Start()

    {
        // Kiểm tra và khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;
                isFirebaseInitialized = true;  // Đánh dấu Firebase đã khởi tạo
                Debug.Log("Firebase Initialized");
            }
            else
            {
                Debug.LogError("Failed to initialize Firebase: " + task.Exception);
            }
        });
    }


    // Lưu dữ liệu người chơi lên Firebase
    public void SavePlayerData(Dichuyennv1 playerStats)
    {
        // Lấy giá trị username từ PlayerPrefs
        string username = PlayerPrefs.GetString("username", "");

        // Kiểm tra nếu username là null hoặc rỗng
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is null or empty.");
            return;
        }

        // Kiểm tra nếu Firebase chưa được khởi tạo
        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase Database reference is not initialized.");
            return;
        }

        // Tạo đối tượng PlayerData bao gồm các trạng thái nhiệm vụ và kỹ năng
        PlayerData playerData = new PlayerData(playerStats);
        string json = JsonUtility.ToJson(playerData);
        reference.Child("players").Child(username).SetRawJsonValueAsync(json);
        Debug.Log("Player data saved to Firebase");
    }

    // Tải dữ liệu người chơi từ Firebase
    public void LoadPlayerData(Action<PlayerData> onDataLoaded)
    {
        // Lấy giá trị username từ PlayerPrefs
        string username = PlayerPrefs.GetString("username", "");

        // Kiểm tra nếu username là null hoặc rỗng
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is null or empty.");
            onDataLoaded?.Invoke(null); // Gửi giá trị null nếu username không hợp lệ
            return;
        }

        // Kiểm tra nếu Firebase chưa được khởi tạo
        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase Database reference is not initialized.");
            onDataLoaded?.Invoke(null); // Gửi giá trị null nếu Firebase chưa được khởi tạo
            return;
        }

        reference.Child("players").Child(username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading player data: " + task.Exception);
                onDataLoaded?.Invoke(null); // Gửi giá trị null nếu có lỗi
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                    onDataLoaded?.Invoke(playerData);
                }
                else
                {
                    Debug.LogWarning("No data found for player: " + username);
                    onDataLoaded?.Invoke(null); // Gửi giá trị null nếu không có dữ liệu
                }
            }
        });
    }

    // Cập nhật dữ liệu người chơi trong game sau khi tải từ Firebase
    public void UpdatePlayerStats(PlayerData playerData, Dichuyennv1 playerStats)
    {
        if (playerData == null || playerStats == null)
        {
            Debug.LogError("Player data or player stats is null");
            return;
        }

        // Cập nhật các thông tin cơ bản của người chơi
        playerStats.currentHealth = playerData.currentHealth;
        playerStats.currentMana = playerData.currentMana;
        playerStats.maxHealth = playerData.maxHealth;
        playerStats.maxMana = playerData.maxMana;
        playerStats.damageAmount = playerData.damageAmount;
        playerStats.level = playerData.level;
        playerStats.upgradePoints = playerData.upgradePoints;
        playerStats.expCurrent = playerData.expCurrent;
        playerStats.expMax = playerData.expMax;

        // Cập nhật trạng thái nhiệm vụ và kỹ năng
        playerStats.isQuest1Complete = playerData.isQuest1Complete;
        playerStats.isAppleQuestComplete = playerData.isAppleQuestComplete;

        // Cập nhật các thông số kỹ năng
        playerStats.skill1Cooldown = playerData.skill1Cooldown;
        playerStats.skill2Cooldown = playerData.skill2Cooldown;
        playerStats.skill3Cooldown = playerData.skill3Cooldown;

        // Cập nhật các timer kỹ năng
        playerStats.skill1Timer = playerData.skill1Timer;
        playerStats.skill2Timer = playerData.skill2Timer;
        playerStats.skill3Timer = playerData.skill3Timer;

        // Cập nhật UI và các thay đổi liên quan đến dữ liệu người chơi
        playerStats.UpdateStatsText();  // Cập nhật UI sau khi tải dữ liệu
    }

    // Định nghĩa lớp dữ liệu người chơi (PlayerData) với các trường nhiệm vụ và kỹ năng
    [System.Serializable]
    public class PlayerData
    {
        // Thông tin cơ bản về người chơi
        public int currentHealth;
        public int currentMana;
        public int maxHealth;
        public int maxMana;
        public int damageAmount;
        public int level;
        public int upgradePoints;
        public float expCurrent;
        public float expMax;

        // Các trạng thái nhiệm vụ
        public bool isQuest1Complete;
        public bool isAppleQuestComplete;

        // Các thông số cooldown kỹ năng
        public float skill1Cooldown;
        public float skill2Cooldown;
        public float skill3Cooldown;

        // Các timer cho kỹ năng
        public float skill1Timer;
        public float skill2Timer;
        public float skill3Timer;

        // Constructor để chuyển đổi từ dữ liệu người chơi trong game sang Firebase
        public PlayerData(Dichuyennv1 playerStats)
        {
            currentHealth = playerStats.currentHealth;
            currentMana = playerStats.currentMana;
            maxHealth = playerStats.maxHealth;
            maxMana = playerStats.maxMana;
            damageAmount = playerStats.damageAmount;
            level = playerStats.level;
            upgradePoints = playerStats.upgradePoints;
            expCurrent = playerStats.expCurrent;
            expMax = playerStats.expMax;

            // Nhiệm vụ
            isQuest1Complete = playerStats.isQuest1Complete;
            isAppleQuestComplete = playerStats.isAppleQuestComplete;

            // Kỹ năng
            skill1Cooldown = playerStats.skill1Cooldown;
            skill2Cooldown = playerStats.skill2Cooldown;
            skill3Cooldown = playerStats.skill3Cooldown;

            // Timer kỹ năng
            skill1Timer = playerStats.skill1Timer;
            skill2Timer = playerStats.skill2Timer;
            skill3Timer = playerStats.skill3Timer;
        }
    }
}
