// using UnityEngine;
// using UnityEngine.UI;

// public class NPCInteractionFlow : MonoBehaviour
// {
//     public GameObject dialoguePanel; // Panel hội thoại
//     public Text dialogueText; // Text hiển thị hội thoại
//     public Button nextButton; // Nút "Tiếp theo"
//     public Button agreeButton; // Nút "Đồng ý chơi mini game"
//     public GameObject miniGamePanel; // Panel mini game

//     private string[] initialDialogues; // Danh sách hội thoại ban đầu
//     private string hintDialogue; // Gợi ý sau mini game
//     private int currentDialogueIndex = 0; // Chỉ số câu thoại hiện tại
//     private bool isPlayerNearby = false; // Kiểm tra người chơi có gần NPC không
//     private bool isMiniGameCompleted = false; // Trạng thái hoàn thành mini game

//     void Start()
//     {
//         dialoguePanel.SetActive(false); // Ẩn panel hội thoại ban đầu
//         miniGamePanel.SetActive(false); // Ẩn panel mini game ban đầu

//         // Gắn sự kiện cho các nút
//         nextButton.onClick.AddListener(ShowNextDialogue);
//         agreeButton.onClick.AddListener(StartMiniGame);

//         // Thiết lập hội thoại ban đầu và gợi ý
//         initialDialogues = new string[]
//         {
//             "Xin chào, nhà thám hiểm trẻ tuổi!",
//             "Tôi có một thử thách nhỏ cho bạn. Nếu bạn vượt qua, tôi sẽ cho bạn một gợi ý quan trọng.",
//             "Bạn có muốn thử không?"
//         };

//         hintDialogue = "Tuyệt vời! Để đi qua vùng đất mới, hãy tìm cây cầu đá ở phía Tây Nam.";
//     }

//     void Update()
//     {
//         if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
//         {
//             StartDialogue();
//         }
//     }

//     private void StartDialogue()
//     {
//         dialoguePanel.SetActive(true); // Hiển thị panel hội thoại
//         currentDialogueIndex = 0; // Bắt đầu từ câu thoại đầu tiên
//         ShowDialogue();
//     }

//     private void ShowDialogue()
//     {
//         if (currentDialogueIndex < initialDialogues.Length)
//         {
//             dialogueText.text = initialDialogues[currentDialogueIndex]; // Hiển thị câu thoại hiện tại
//             nextButton.gameObject.SetActive(true); // Hiển thị nút "Tiếp theo"
//             agreeButton.gameObject.SetActive(false); // Ẩn nút "Đồng ý"
//         }
//         else
//         {
//             // Khi kết thúc hội thoại ban đầu, hiển thị nút "Đồng ý chơi"
//             nextButton.gameObject.SetActive(false);
//             agreeButton.gameObject.SetActive(true);
//         }
//     }

//     private void ShowNextDialogue()
//     {
//         currentDialogueIndex++; // Chuyển sang câu thoại tiếp theo
//         ShowDialogue();
//     }

//     private void StartMiniGame()
//     {
//         dialoguePanel.SetActive(false); // Đóng panel hội thoại
//         miniGamePanel.SetActive(true); // Hiển thị mini game
//     }

//     // Gọi hàm này khi mini game kết thúc
//     public void CompleteMiniGame()
//     {
//         miniGamePanel.SetActive(false); // Đóng panel mini game
//         isMiniGameCompleted = true; // Đánh dấu trạng thái hoàn thành
//         ShowHintDialogue(); // Hiển thị gợi ý
//     }

//     private void ShowHintDialogue()
//     {
//         dialoguePanel.SetActive(true); // Hiển thị lại panel hội thoại
//         dialogueText.text = hintDialogue; // Hiển thị gợi ý
//         nextButton.gameObject.SetActive(false); // Ẩn nút "Tiếp theo"
//         agreeButton.gameObject.SetActive(false); // Ẩn nút "Đồng ý"
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             isPlayerNearby = true;
//         }
//     }

//     private void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             isPlayerNearby = false;
//             dialoguePanel.SetActive(false); // Đóng panel hội thoại nếu rời xa NPC
//         }
//     }
// }
