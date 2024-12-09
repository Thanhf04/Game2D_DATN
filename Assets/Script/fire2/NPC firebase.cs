using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NPCAppleArmorQuestFirebase : MonoBehaviour
{
    public GameObject questPanel;
    public Text questText;
    public Button continueButton;
    public Button confirmButton;
    public Text appleCountText;
    public Text armorCountText;
    public UI_Coin uiCoin;
    public Text currentCardText;

    private bool hasCompletedAppleQuest = false;
    private bool hasCompletedArmorQuest = false;
    private int appleCount = 0;
    private int armorCount = 0;
    private int currentCard = 0;
    private string userName;

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

        LoadQuestStatus();  // Tải trạng thái nhiệm vụ khi bắt đầu
        SetupUI();
    }

    private void SetupUI()
    {
        if (questPanel != null)
            questPanel.SetActive(false);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);

        if (appleCountText != null)
        {
            appleCountText.text = "";
            appleCountText.gameObject.SetActive(false);
        }

        if (armorCountText != null)
        {
            armorCountText.text = "";
            armorCountText.gameObject.SetActive(false);
        }

        if (currentCardText != null)
        {
            currentCardText.text = "";
            currentCardText.gameObject.SetActive(false);
        }

        uiCoin = FindObjectOfType<UI_Coin>();
    }

    void OnMouseDown()
    {
        if (questPanel != null && !hasCompletedAppleQuest && !hasCompletedArmorQuest)
        {
            questPanel.SetActive(true);
            questText.text = "Chào bạn, hãy bắt đầu nhiệm vụ mới!";
        }
    }

    // Khi người chơi nhấn nút "Continue"
    private void OnContinue()
    {
        if (appleCount >= 3 && !hasCompletedAppleQuest)
        {
            // Cập nhật trạng thái nhiệm vụ táo
            hasCompletedAppleQuest = true;
            appleCount = 0; // Reset số táo sau khi hoàn thành nhiệm vụ
            Debug.Log("Apple quest completed!");

            // Lưu trạng thái nhiệm vụ vào Firebase
            SaveQuestStatus(new Dictionary<string, object>
            {
                { "hasCompletedAppleQuest", hasCompletedAppleQuest },
                { "appleCount", appleCount },
                { "hasCompletedArmorQuest", hasCompletedArmorQuest },
                { "armorCount", armorCount },
                { "currentCard", currentCard }
            });

            // Cập nhật UI
            UpdateUI();
        }
        else if (hasCompletedAppleQuest && armorCount >= 1 && !hasCompletedArmorQuest)
        {
            // Hoàn thành nhiệm vụ giáp
            hasCompletedArmorQuest = true;
            armorCount = 0; // Reset số giáp sau khi hoàn thành nhiệm vụ
            Debug.Log("Armor quest completed!");

            // Lưu trạng thái nhiệm vụ vào Firebase
            SaveQuestStatus(new Dictionary<string, object>
            {
                { "hasCompletedAppleQuest", hasCompletedAppleQuest },
                { "appleCount", appleCount },
                { "hasCompletedArmorQuest", hasCompletedArmorQuest },
                { "armorCount", armorCount },
                { "currentCard", currentCard }
            });

            // Cập nhật UI
            UpdateUI();
        }
        else
        {
            questText.text = "Bạn chưa hoàn thành các nhiệm vụ yêu cầu.";
        }
    }

    // Khi người chơi nhấn nút "Confirm"
    private void OnConfirm()
    {
        // Đóng bảng nhiệm vụ
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }
        Debug.Log("Quest panel closed.");
    }

    public void SaveQuestStatus(Dictionary<string, object> questData)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("players1").Child(userName);
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

    public void LoadQuestStatus()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("players1").Child(userName).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    hasCompletedAppleQuest = Convert.ToBoolean(snapshot.Child("hasCompletedAppleQuest").Value);
                    appleCount = Convert.ToInt32(snapshot.Child("appleCount").Value);
                    hasCompletedArmorQuest = Convert.ToBoolean(snapshot.Child("hasCompletedArmorQuest").Value);
                    armorCount = Convert.ToInt32(snapshot.Child("armorCount").Value);
                    currentCard = Convert.ToInt32(snapshot.Child("currentCard").Value);

                    Debug.Log($"Quest status loaded: Apple Quest Completed: {hasCompletedAppleQuest}, Apple Count: {appleCount}, Armor Quest Completed: {hasCompletedArmorQuest}, Armor Count: {armorCount}, Current Card: {currentCard}");

                    UpdateUI();
                }
                else
                {
                    Debug.Log("No quest data found for this user.");
                }
            }
            else
            {
                Debug.LogError("Failed to get quest data from Firebase.");
            }
        });
    }

    private void UpdateUI()
    {
        appleCountText.text = $"Số táo đã thu thập: {appleCount}/3";
        armorCountText.text = $"Số giáp đã thu thập: {armorCount}/1";
        currentCardText.text = $"Số thẻ đã lật thành công: {currentCard}/2";
    }

    public void CollectApple()
    {
        appleCount++;
        SaveQuestStatus(new Dictionary<string, object>
        {
            { "hasCompletedAppleQuest", hasCompletedAppleQuest },
            { "appleCount", appleCount },
            { "hasCompletedArmorQuest", hasCompletedArmorQuest },
            { "armorCount", armorCount },
            { "currentCard", currentCard }
        });

        UpdateUI();
    }

    public void CollectArmor()
    {
        armorCount++;
        SaveQuestStatus(new Dictionary<string, object>
        {
            { "hasCompletedAppleQuest", hasCompletedAppleQuest },
            { "appleCount", appleCount },
            { "hasCompletedArmorQuest", hasCompletedArmorQuest },
            { "armorCount", armorCount },
            { "currentCard", currentCard }
        });

        UpdateUI();
    }

    public void FlipCard()
    {
        currentCard++;
        SaveQuestStatus(new Dictionary<string, object>
        {
            { "hasCompletedAppleQuest", hasCompletedAppleQuest },
            { "appleCount", appleCount },
            { "hasCompletedArmorQuest", hasCompletedArmorQuest },
            { "armorCount", armorCount },
            { "currentCard", currentCard }
        });

        UpdateUI();
    }
}
