using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NPCQuestFirebase : MonoBehaviour
{
    public Text swordCountText;
    public Text monsterCountText;
    public UI_Coin uiCoin;

    private string userName;
    private int swordCount = 0;
    private int monsterKillCount = 0;
    private bool hasReceivedReward = false;


    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firebase initialization failed.");
            }
            else
            {
                Debug.Log("Firebase initialized successfully.");
            }
        });

        userName = PlayerPrefs.GetString("username", "");
        if (string.IsNullOrEmpty(userName))
        {
            Debug.LogError("Username is empty. Cannot load player data.");
            return;
        }

        LoadQuestStatus();
    }

    // Cập nhật trạng thái nhiệm vụ vào Firebase
    public void SaveQuestStatus()
    {
        Debug.Log("Saving quest status...");

        // Save the quest status to Firebase
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("quests").Child(userName);

        // Tạo Dictionary để lưu trữ dữ liệu quest
        Dictionary<string, object> questData = new Dictionary<string, object>
        {
            { "swordCount", swordCount },
            { "monsterKillCount", monsterKillCount },
            { "hasReceivedReward", hasReceivedReward }
        };

        // Sử dụng SetValueAsync để cập nhật dữ liệu
        reference.SetValueAsync(questData).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Quest status saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save quest status: " + task.Exception);
            }
        });
    }

    // Tải dữ liệu quest từ Firebase
    public void LoadQuestStatus()
    {
        // Kiểm tra kết nối Firebase trước khi thực hiện truy vấn
        if (FirebaseDatabase.DefaultInstance == null)
        {
            Debug.LogError("Firebase is not initialized.");
            return;
        }

        // Tải dữ liệu quest từ Firebase
        FirebaseDatabase.DefaultInstance.RootReference.Child("quests").Child(userName).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    // Load dữ liệu từ Firebase và cập nhật trạng thái quest
                    swordCount = Convert.ToInt32(snapshot.Child("swordCount").Value);
                    monsterKillCount = Convert.ToInt32(snapshot.Child("monsterKillCount").Value);
                    hasReceivedReward = Convert.ToBoolean(snapshot.Child("hasReceivedReward").Value);

                    Debug.Log($"Quest status loaded: Sword Count: {swordCount}, Monster Kill Count: {monsterKillCount}, Has Received Reward: {hasReceivedReward}");

                    UpdateUI();
                }
                else
                {
                    Debug.Log("No quest data found for this user. Initializing quest...");
                    InitializeQuest();
                }
            }
            else
            {
                Debug.LogError("Failed to get quest data from Firebase.");
            }
        });
    }

    // Khởi tạo quest nếu không có dữ liệu trong Firebase
    private void InitializeQuest()
    {
        swordCount = 0;
        monsterKillCount = 0;
        hasReceivedReward = false;

        UpdateUI();
    }

    // Cập nhật UI
    private void UpdateUI()
    {
        swordCountText.text = $"Số kiếm đã tìm được: {swordCount}/1";
        monsterCountText.text = $"Số quái cần giết: {monsterKillCount}/5";

        // Nếu có thay đổi UI khác, bạn có thể làm thêm ở đây
    }

    // Các phương thức getter để truy xuất dữ liệu
    public int GetSwordCount()
    {
        return swordCount;
    }

    public int GetMonsterKillCount()
    {
        return monsterKillCount;
    }

    public bool GetHasReceivedReward()
    {
        return hasReceivedReward;
    }

    // Các phương thức setter để thay đổi dữ liệu
    public void SetSwordCount(int count)
    {
        swordCount = count;
        CheckQuestCompletion(); // Kiểm tra tình trạng hoàn thành nhiệm vụ
        UpdateUI();
    }

    public void SetMonsterKillCount(int count)
    {
        monsterKillCount = count;
        CheckQuestCompletion(); // Kiểm tra tình trạng hoàn thành nhiệm vụ
        UpdateUI();
    }

    public void SetHasReceivedReward(bool received)
    {
        hasReceivedReward = received;
        UpdateUI();
    }

    // Kiểm tra xem người chơi đã hoàn thành nhiệm vụ hay chưa
    private void CheckQuestCompletion()
    {
        if (swordCount >= 1 && monsterKillCount >= 5 && !hasReceivedReward)
        {
            // Nhiệm vụ hoàn thành, cập nhật trạng thái
            hasReceivedReward = true;
            Debug.Log("Quest completed! Reward received.");

            // Cập nhật trạng thái vào Firebase
            SaveQuestStatus();
        }
    }

    // Các phương thức để thu thập kiếm và giết quái vật
    public void CollectSword()
    {
        swordCount++;
        SaveQuestStatus();
    }

    public void KillMonster()
    {
        monsterKillCount++;
        SaveQuestStatus();
    }
}