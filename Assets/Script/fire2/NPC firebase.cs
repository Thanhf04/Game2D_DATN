using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseQuestManager : MonoBehaviour
{
    private string userName;
    private int appleCount;
    private int armorCount;
    private bool hasCompletedAppleQuest;
    private bool hasCompletedArmorQuest;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firebase initialization failed.");
            }
            else
            {
                Debug.Log("Firebase initialized successfully.");
                userName = PlayerPrefs.GetString("username", "");

                if (string.IsNullOrEmpty(userName))
                {
                    Debug.LogError("Username is empty. Cannot load player data.");
                    return;
                }

                LoadQuestStatus(); // Load quest status when the game starts
            }
        });
    }

    public void LoadQuestStatus()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("quests").Child(userName);

        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    appleCount = Convert.ToInt32(snapshot.Child("appleCount").Value);
                    armorCount = Convert.ToInt32(snapshot.Child("armorCount").Value);
                    hasCompletedAppleQuest = Convert.ToBoolean(snapshot.Child("hasCompletedAppleQuest").Value);
                    hasCompletedArmorQuest = Convert.ToBoolean(snapshot.Child("hasCompletedArmorQuest").Value);

                    Debug.Log("Quest status loaded from Firebase.");

                    // After loading data, call UpdateUI to update the UI
                    NPCAppleArmorQuest npcAppleArmorQuest = FindObjectOfType<NPCAppleArmorQuest>();
                    if (npcAppleArmorQuest != null)
                    {
                        npcAppleArmorQuest.UpdateUI();
                    }
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

    public void SaveQuestStatus()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("quests").Child(userName);

        Dictionary<string, object> questData = new Dictionary<string, object>
        {
            { "appleCount", appleCount },
            { "armorCount", armorCount },
            { "hasCompletedAppleQuest", hasCompletedAppleQuest },
            { "hasCompletedArmorQuest", hasCompletedArmorQuest }
        };

        reference.SetValueAsync(questData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Quest status saved to Firebase successfully.");
            }
            else
            {
                Debug.LogError("Failed to save quest status to Firebase: " + task.Exception);
            }
        });
    }

    public void SetAppleCount(int count) { appleCount = count; }
    public void SetArmorCount(int count) { armorCount = count; }
    public void SetHasCompletedAppleQuest(bool completed) { hasCompletedAppleQuest = completed; }
    public void SetHasCompletedArmorQuest(bool completed) { hasCompletedArmorQuest = completed; }

    public int GetAppleCount() { return appleCount; }
    public int GetArmorCount() { return armorCount; }
    public bool GetHasCompletedAppleQuest() { return hasCompletedAppleQuest; }
    public bool GetHasCompletedArmorQuest() { return hasCompletedArmorQuest; }

    private void InitializeQuest()
    {
        appleCount = 0;
        armorCount = 0;
        hasCompletedAppleQuest = false;
        hasCompletedArmorQuest = false;

        SaveQuestStatus(); // Save default values to Firebase
    }
}
