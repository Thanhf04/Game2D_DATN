// using System.Collections;
// using Fusion;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// public class tangchiso : NetworkBehaviour
// {
//     public GameObject statsPanel;
//     public TMP_Text notificationText;
//     public TMP_Text healthText;
//     public TMP_Text manaText;
//     public TMP_Text damaText;
//     public Text levelText;
//     public Text pointsText;

//     private Player player1;

//     void Start()
//     {
//         statsPanel.SetActive(false);
//         StartCoroutine(WaitForPlayerSpawn());
//     }

//     // Coroutine để đợi Player1 spawn
//     IEnumerator WaitForPlayerSpawn()
//     {
//         while (player1 == null)
//         {
//             GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//             if (playerObj != null)
//             {
//                 var playerNetworkObject = playerObj.GetComponent<NetworkObject>();

//                 // Kiểm tra quyền sở hữu (Ownership)
//                 if (playerNetworkObject != null && playerNetworkObject.HasInputAuthority)
//                 {
//                     player1 = playerObj.GetComponent<Player>();
//                 }
//             }
//             yield return null;
//         }
//     }

//     // Phương thức gọi khi nhấn nút mở/đóng panel
//     public void ToggleStatsPanel()
//     {
//         statsPanel.SetActive(!statsPanel.activeSelf);
//     }

//     // Phương thức gọi khi nhấn nút tăng máu
//     public void IncreaseHealth()
//     {
//         if (player1 == null)
//         {
//             return;
//         }
//         if (player1.upgradePoints > 0)
//         {
//             player1.MaxHealth += 100;
//             player1.Health = Mathf.Min(player1.MaxHealth, player1.Health + 100);
//             Debug.Log(player1.MaxHealth);
//             player1.upgradePoints--;
//             UpdateStatsText();
//         }
//         else
//         {
//             ShowNotification("Bạn đã hết điểm nâng cấp!");
//         }
//     }

//     // Phương thức gọi khi nhấn nút giảm máu
//     public void DecreaseHealth()
//     {
//         if (player1 == null)
//         {
//             return;
//         }
//         if (player1.Health > 1 && player1.upgradePoints < player1.level + 5)
//         {
//             player1.MaxHealth -= 100;
//             player1.Health = Mathf.Clamp(player1.Health - 100, 1, player1.MaxHealth);
//             player1.upgradePoints++;
//             UpdateStatsText();
//         }
//     }

//     // Phương thức gọi khi nhấn nút tăng mana
//     public void IncreaseMana()
//     {
//         if (player1 == null)
//         {
//             return;
//         }
//         if (player1.upgradePoints > 0)
//         {
//             player1.MaxMana += 100;
//             player1.Mana = Mathf.Min(player1.MaxMana, player1.Mana + 100);
//             player1.upgradePoints--;
//             UpdateStatsText();
//         }
//         else
//         {
//             ShowNotification("Bạn đã hết điểm nâng cấp!");
//         }
//     }

//     // Phương thức gọi khi nhấn nút giảm mana
//     public void DecreaseMana()
//     {
//         if (player1 == null)
//         {
//             return;
//         }
//         if (player1.Mana > 1 && player1.upgradePoints < player1.level + 5)
//         {
//             player1.MaxMana -= 100;
//             player1.Mana = Mathf.Clamp(player1.Mana - 100, 1, player1.MaxMana);
//             player1.upgradePoints++;
//             UpdateStatsText();
//         }
//     }

//     // Phương thức gọi khi nhấn nút tăng damage
//     public void IncreaseDame()
//     {
//         if (player1 == null)
//         {
//             return;
//         }
//         if (player1.upgradePoints > 0)
//         {
//             player1.Damage += 10;
//             player1.upgradePoints--;
//             UpdateStatsText();
//         }
//         else
//         {
//             ShowNotification("Bạn đã hết điểm nâng cấp!");
//         }
//     }

//     // Phương thức gọi khi nhấn nút giảm damage
//     public void DecreaseDamage()
//     {
//         if (player1 == null)
//         {
//             return;
//         }
//         if (player1.Damage > 0 && player1.upgradePoints < player1.level + 5)
//         {
//             player1.Damage -= 10;
//             player1.upgradePoints++;
//             UpdateStatsText();
//         }
//     }

//     public void UpdateStatsText()
//     {
//         if (player1 == null)
//         {
//             return;
//         }
//         healthText.text = ((player1.MaxHealth - 100) / 100).ToString();
//         manaText.text = ((player1.MaxMana - 100) / 100).ToString();
//         damaText.text = ((player1.Damage - 10) / 10).ToString();
//         levelText.text = "Level: " + player1.level;
//         pointsText.text = "Points: " + player1.upgradePoints;
//     }

//     // Hiển thị thông báo
//     void ShowNotification(string message)
//     {
//         notificationText.text = message;
//         Invoke("ClearNotification", 2f);
//     }

//     // Xóa thông báo sau một khoảng thời gian
//     void ClearNotification()
//     {
//         notificationText.text = "";
//     }
// }
