// using UnityEngine;
// using UnityEngine.UI;

// public class NPCInteractionWithDialogue : MonoBehaviour
// {
//     public GameObject dialoguePanel; // Panel hiển thị hội thoại
//     public Text dialogueText; // Text hiển thị câu thoại
//     public Button nextButton; // Nút "Tiếp theo"
//     public GameObject miniGamePanel; // Panel mini game
//     public Button agreeButton; // Nút đồng ý thực hiện mini game

//     private string[] dialogues; // Danh sách hội thoại
//     private int currentDialogueIndex = 0; // Chỉ số hội thoại hiện tại
//     private bool isPlayerNearby = false; // Kiểm tra người chơi ở gần

//     void Start()
//     {
//         dialoguePanel.SetActive(false); // Ẩn panel hội thoại ban đầu
//         miniGamePanel.SetActive(false); // Ẩn panel mini game ban đầu
//         nextButton.onClick.AddListener(ShowNextDialogue); // Gắn sự kiện cho nút "Tiếp theo"
//         agreeButton.onClick.AddListener(StartMiniGame); // Gắn sự kiện cho nút "Đồng ý thực hiện"

//         // Ví dụ danh sách hội thoại
//         dialogues = new string[]
//         {
//             "Chào bạn! Tôi có một thử thách muốn mời bạn tham gia.",
//             "Thử thách của tôi là một trò chơi điền chữ đơn giản.",
//             "Bạn có muốn tham gia thử thách không? Nếu đồng ý, hãy nhấn nút bên dưới!"
//         };
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
//         currentDialogueIndex = 0; // Bắt đầu từ câu đầu tiên
//         ShowDialogue(); // Hiển thị câu thoại
//     }

//     private void ShowDialogue()
//     {
//         if (currentDialogueIndex < dialogues.Length)
//         {
//             dialogueText.text = dialogues[currentDialogueIndex]; // Hiển thị câu thoại hiện tại
//         }
//         else
//         {
//             // Kết thúc hội thoại, hiển thị nút "Đồng ý"
//             nextButton.gameObject.SetActive(false); // Ẩn nút "Tiếp theo"
//             agreeButton.gameObject.SetActive(true); // Hiển thị nút "Đồng ý thực hiện"
//         }
//     }

//     private void ShowNextDialogue()
//     {
//         currentDialogueIndex++; // Chuyển sang câu tiếp theo
//         ShowDialogue(); // Hiển thị câu thoại mới
//     }

//     private void StartMiniGame()
//     {
//         dialoguePanel.SetActive(false); // Đóng panel hội thoại
//         miniGamePanel.SetActive(true); // Mở panel mini game
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
//             dialoguePanel.SetActive(false); // Đóng panel nếu rời khỏi NPC
//         }
//     }
// }
