//using Firebase;
//using Firebase.Database;
//using Firebase.Extensions;
//using UnityEngine;
//using UnityEngine.UI;
//using System;

//public class NPCQuestFirebase : MonoBehaviour
//{
//    public GameObject questPanel;
//    public Text questText;
//    public Button continueButton;
//    public Button confirmButton;
//    public Text swordCountText;
//    public Text monsterCountText;
//    public UI_Coin uiCoin;

//    private string initialQuestText = "Xin chào chàng hiệp sĩ, bạn là người được chọn để giải cứu vùng đất này.";
//    private string secondQuestText = "Nhiệm vụ đầu tiên của bạn là tìm lại thanh kiếm đã mất.";
//    private string congratulationText = "Giỏi lắm chàng trai, bây giờ bạn có thể dùng chuột trái để tấn công!";
//    private string thirdQuestText = "Nhiệm vụ tiếp theo của bạn là giết 5 con quái.";
//    private string rewardCompletionText = "Chúc mừng bạn đã hoàn thành nhiệm vụ, phần thưởng của bạn là 50 vàng!";
//    private string finalEncouragementText = "Chúc mừng chàng trai, bây giờ bạn có thể tiếp tục cuộc hành trình rồi.";
//    private string continuareText = "Còn hãy đi tìm người thợ rèn để học tập thêm.";

//    private bool isPanelVisible = false;
//    private bool isQuestStarted = false;
//    private bool hasShownCongratulation = false;
//    private bool hasReceivedReward = false;

//    private int swordCount = 0;
//    private int monsterKillCount = 0;

//    private string userName;

//    void Start()
//    {
//        // Ensure Firebase is initialized before anything else
//        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
//            FirebaseApp app = FirebaseApp.DefaultInstance;
//            if (task.IsFaulted || task.IsCanceled)
//            {
//                Debug.LogError("Firebase initialization failed.");
//            }
//            else
//            {
//                Debug.Log("Firebase initialized successfully.");
//            }
//        });

//        // Get user name from PlayerPrefs
//        userName = PlayerPrefs.GetString("username", "");

//        if (string.IsNullOrEmpty(userName))
//        {
//            Debug.LogError("Username is empty. Cannot load player data.");
//            return;
//        }

//        // Load quest status on game start
//        LoadQuestStatus();

//        // Setup UI
//        if (questPanel != null)
//        {
//            questPanel.SetActive(false);
//        }

//        if (continueButton != null)
//        {
//            continueButton.onClick.AddListener(OnContinue);
//        }

//        if (confirmButton != null)
//        {
//            confirmButton.onClick.AddListener(OnConfirm);
//        }

//        if (swordCountText != null)
//        {
//            swordCountText.text = "";
//            swordCountText.gameObject.SetActive(false);
//        }

//        if (monsterCountText != null)
//        {
//            monsterCountText.text = "";
//            monsterCountText.gameObject.SetActive(false);
//        }

//        uiCoin = FindObjectOfType<UI_Coin>();
//    }

//    void OnMouseDown()
//    {
//        if (questPanel != null && !isPanelVisible)
//        {
//            questPanel.SetActive(true);

//            if (swordCount == 1 && !hasShownCongratulation)
//            {
//                questText.text = congratulationText;
//                hasShownCongratulation = true;
//                isQuestStarted = false;
//            }
//            else if (swordCount == 0)
//            {
//                questText.text = initialQuestText;
//                isPanelVisible = true;
//            }
//        }
//    }

//    private void OnContinue()
//    {
//        if (questPanel != null)
//        {
//            if (!isQuestStarted && swordCount == 0)
//            {
//                // Nhiệm vụ kiếm
//                questText.text = secondQuestText;
//                isQuestStarted = true;

//                swordCountText.gameObject.SetActive(true);
//                swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1";
//                swordCountText.color = Color.white;
//            }
//            else if (swordCount == 1 && monsterKillCount < 5)
//            {
//                // Nhiệm vụ giết quái
//                questText.text = thirdQuestText;

//                swordCountText.gameObject.SetActive(false);
//                monsterCountText.gameObject.SetActive(true);
//                monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/5";
//                monsterCountText.color = Color.white;
//            }
//            else if (swordCount == 1 && monsterKillCount >= 5 && !hasReceivedReward)
//            {
//                // Hoàn thành nhiệm vụ và nhận thưởng
//                questText.text = rewardCompletionText;

//                if (uiCoin != null)
//                {
//                    uiCoin.AddCoins(50);
//                }
//                hasReceivedReward = true;
//                monsterCountText.gameObject.SetActive(false);
//            }
//            else if (hasReceivedReward)
//            {
//                // Câu chúc mừng cuối cùng
//                questText.text = finalEncouragementText;
//                questText.text = continuareText;

//                // Save quest status to Firebase after completing
//                SaveQuestStatus();
//            }
//        }
//    }

//    private void OnConfirm()
//    {
//        if (questPanel != null)
//        {
//            questPanel.SetActive(false);
//            isPanelVisible = false;
//        }
//    }

//    public void FindSword()
//    {
//        swordCount = 1;
//        swordCountText.text = "Số kiếm đã tìm được: " + swordCount + "/1";

//        if (swordCount == 1)
//        {
//            swordCountText.color = Color.yellow;
//        }

//        // Save quest status to Firebase after collecting sword
//        SaveQuestStatus();
//    }

//    public void KillMonster()
//    {
//        monsterKillCount++;
//        monsterCountText.text = "Số quái cần giết: " + monsterKillCount + "/5";

//        if (monsterKillCount == 5)
//        {
//            monsterCountText.color = Color.yellow;
//        }

//        // Save quest status to Firebase after killing monster
//        SaveQuestStatus();
//    }

//    public void SaveQuestStatus()
//    {
//        // Save the quest status to Firebase
//        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("quests").Child(userName);
//        reference.SetRawJsonValueAsync(JsonUtility.ToJson(new QuestData(swordCount, monsterKillCount, hasReceivedReward)))
//            .ContinueWithOnMainThread(task => {
//                if (task.IsCompleted)
//                {
//                    Debug.Log("Quest status saved successfully.");
//                }
//                else
//                {
//                    Debug.LogError("Failed to save quest status: " + task.Exception);
//                }
//            });
//    }

//    public void LoadQuestStatus()
//    {
//        // Check Firebase connection before making a query
//        if (FirebaseDatabase.DefaultInstance == null)
//        {
//            Debug.LogError("Firebase is not initialized.");
//            return;
//        }

//        // Load quest data from Firebase
//        FirebaseDatabase.DefaultInstance.RootReference
//            .Child("quests")
//            .Child(userName)
//            .GetValueAsync().ContinueWithOnMainThread(task => {
//                if (task.IsCompleted)
//                {
//                    DataSnapshot snapshot = task.Result;

//                    if (snapshot.Exists)
//                    {
//                        // Load data from Firebase and update quest status
//                        swordCount = Convert.ToInt32(snapshot.Child("swordCount").Value);
//                        monsterKillCount = Convert.ToInt32(snapshot.Child("monsterKillCount").Value);
//                        hasReceivedReward = Convert.ToBoolean(snapshot.Child("hasReceivedReward").Value);

//                        Debug.Log($"Quest status loaded: Sword Count: {swordCount}, Monster Kill Count: {monsterKillCount}, Has Received Reward: {hasReceivedReward}");

//                        UpdateUI();
//                    }
//                    else
//                    {
//                        Debug.Log("No quest data found for this user.");
//                    }
//                }
//                else
//                {
//                    Debug.LogError("Failed to get quest data from Firebase.");
//                }
//            });
//    }

//    private void UpdateUI()
//    {
//        swordCountText.text = $"Số kiếm đã tìm được: {swordCount}/1";
//        monsterCountText.text = $"Số quái cần giết: {monsterKillCount}/5";
//    }

//    [System.Serializable]
//    public class QuestData
//    {
//        public int swordCount;
//        public int monsterKillCount;
//        public bool hasReceivedReward;

//        public QuestData(int swordCount, int monsterKillCount, bool hasReceivedReward)
//        {
//            this.swordCount = swordCount;
//            this.monsterKillCount = monsterKillCount;
//            this.hasReceivedReward = hasReceivedReward;
//        }
//    }
//}
