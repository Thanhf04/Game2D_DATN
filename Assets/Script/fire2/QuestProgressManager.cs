//using Firebase;
//using Firebase.Database;
//using Firebase.Extensions;
//using UnityEngine;
//using System.Collections.Generic;
//using System;

//public class QuestProgressManager : MonoBehaviour
//{
//    private string userName;
//    private int appleCount;
//    private int armorCount;
//    private bool hasCompletedAppleQuest;
//    private bool hasCompletedArmorQuest;

//    // Thêm các biến liên quan đến Quest_3
//    public int currentCard;
//    public bool hasCompletedCardQuest;
//    public bool hasCompletedWolfQuest;
//    public bool hasCompletedQuestInput;

//    void Start()
//    {
//        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
//            if (task.IsFaulted || task.IsCanceled)
//            {
//                Debug.LogError("Firebase initialization failed.");
//            }
//            else
//            {
//                Debug.Log("Firebase initialized successfully.");
//                userName = PlayerPrefs.GetString("username", "");

//                if (string.IsNullOrEmpty(userName))
//                {
//                    Debug.LogError("Username is empty. Cannot load player data.");
//                    return;
//                }

//                LoadQuestStatus(); // Load quest status when the game starts
//            }
//        });
//    }

//    // Load quest data from Firebase (including Quest_3)
//    public void LoadQuestStatus()
//    {
//        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("quests").Child(userName);

//        reference.GetValueAsync().ContinueWithOnMainThread(task => {
//            if (task.IsCompleted)
//            {
//                DataSnapshot snapshot = task.Result;

//                if (snapshot.Exists)
//                {
//                    // Load dữ liệu của cả các quest trước đó và Quest_3
//                    appleCount = Convert.ToInt32(snapshot.Child("appleCount").Value);
//                    armorCount = Convert.ToInt32(snapshot.Child("armorCount").Value);
//                    hasCompletedAppleQuest = Convert.ToBoolean(snapshot.Child("hasCompletedAppleQuest").Value);
//                    hasCompletedArmorQuest = Convert.ToBoolean(snapshot.Child("hasCompletedArmorQuest").Value);

//                    // Load dữ liệu Quest_3
//                    currentCard = Convert.ToInt32(snapshot.Child("currentCard").Value);
//                    hasCompletedCardQuest = Convert.ToBoolean(snapshot.Child("hasCompletedCardQuest").Value);
//                    hasCompletedWolfQuest = Convert.ToBoolean(snapshot.Child("hasCompletedWolfQuest").Value);
//                    hasCompletedQuestInput = Convert.ToBoolean(snapshot.Child("hasCompletedQuestInput").Value);

//                    Debug.Log("Quest status loaded from Firebase.");

//                    // Sau khi tải dữ liệu xong, gọi UpdateUI của Quest_3
//                    Quest_3 quest3 = FindObjectOfType<Quest_3>();  // Tìm đối tượng Quest_3 trong scene
//                    if (quest3 != null)
//                    {
//                        quest3.UpdateUI(); // Cập nhật UI cho Quest_3
//                    }
//                }
//                else
//                {
//                    Debug.Log("No quest data found for this user. Initializing quest...");
//                    InitializeQuest();
//                }
//            }
//            else
//            {
//                Debug.LogError("Failed to get quest data from Firebase.");
//            }
//        });
//    }



//    // Save quest data to Firebase (including Quest_3)
//    public void SaveQuestStatus()
//    {
//        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("quests").Child(userName);

//        Dictionary<string, object> questData = new Dictionary<string, object>
//        {
//            { "appleCount", appleCount },
//            { "armorCount", armorCount },
//            { "hasCompletedAppleQuest", hasCompletedAppleQuest },
//            { "hasCompletedArmorQuest", hasCompletedArmorQuest },

//            // Add Quest_3 data to Firebase
//            { "currentCard", currentCard },
//            { "hasCompletedCardQuest", hasCompletedCardQuest },
//            { "hasCompletedWolfQuest", hasCompletedWolfQuest },
//            { "hasCompletedQuestInput", hasCompletedQuestInput }
//        };

//        reference.SetValueAsync(questData).ContinueWithOnMainThread(task => {
//            if (task.IsCompleted)
//            {
//                Debug.Log("Quest status saved to Firebase successfully.");
//            }
//            else
//            {
//                Debug.LogError("Failed to save quest status to Firebase: " + task.Exception);
//            }
//        });
//    }

//    // Other methods for Quest_3 status
//    public void SetCurrentCard(int card) { currentCard = card; }
//    public void SetHasCompletedCardQuest(bool completed) { hasCompletedCardQuest = completed; }
//    public void SetHasCompletedWolfQuest(bool completed) { hasCompletedWolfQuest = completed; }
//    public void SetHasCompletedQuestInput(bool completed) { hasCompletedQuestInput = completed; }

//    public int GetCurrentCard() { return currentCard; }
//    public bool GetHasCompletedCardQuest() { return hasCompletedCardQuest; }
//    public bool GetHasCompletedWolfQuest() { return hasCompletedWolfQuest; }
//    public bool GetHasCompletedQuestInput() { return hasCompletedQuestInput; }

//    private void InitializeQuest()
//    {
//        appleCount = 0;
//        armorCount = 0;
//        hasCompletedAppleQuest = false;
//        hasCompletedArmorQuest = false;

//        // Initialize Quest_3 data
//        currentCard = 0;
//        hasCompletedCardQuest = false;
//        hasCompletedWolfQuest = false;
//        hasCompletedQuestInput = false;

//        SaveQuestStatus(); // Save default values to Firebase
//    }
//}
