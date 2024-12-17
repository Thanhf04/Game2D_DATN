using UnityEngine;
using UnityEngine.UI;

public class OpenxepHinh : MonoBehaviour
{
    public GameObject minigame; // GameObject của mini game
    private bool isPlayerInRange = false; // Cờ kiểm tra xem player có trong vùng va chạm không
    private bool isMinigameOpened = false; // Cờ kiểm tra xem mini game đã được mở chưa
    public GameObject player;
    private Dichuyennv1 dichuyen1Script;
    public GameObject panel; // Panel để trò chuyện
    public Button btn_xatnhan; // Nút xác nhận trên panel
    private bool isPanelOpen = false; // Cờ kiểm tra xem panel trò chuyện có đang mở không

    void Start()
    {
        // Lấy script Dichuyennv1 từ player
        dichuyen1Script = player.GetComponent<Dichuyennv1>();
        minigame.SetActive(false); // Đảm bảo mini game ban đầu bị tắt
        panel.SetActive(false); // Đảm bảo panel trò chuyện ban đầu bị tắt

        // Gắn sự kiện cho nút xác nhận
        btn_xatnhan.onClick.AddListener(OpenMinigame);
    }

    void Update()
    {
        // Kiểm tra nếu player trong vùng va chạm và nhấn phím F để mở panel trò chuyện
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !isPanelOpen)
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        if (dichuyen1Script != null)
        {
            dichuyen1Script.enabled = false; // Vô hiệu hóa điều khiển player
        }
        panel.SetActive(true); // Mở panel trò chuyện
        isPanelOpen = true; // Đánh dấu panel đã được mở
    }

    private void OpenMinigame()
    {
        if (!isMinigameOpened)
        {
            panel.SetActive(false); // Đóng panel trò chuyện
            minigame.SetActive(true); // Mở mini game
            isMinigameOpened = true; // Đánh dấu mini game đã được mở

            if (dichuyen1Script != null)
            {
                dichuyen1Script.enabled = false; // Giữ player không di chuyển
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu đối tượng va chạm có tag là "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true; // Bật cờ khi player vào vùng va chạm
        }

    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    // Tắt cờ khi player rời khỏi vùng va chạm
    //    if (other.CompareTag("Player"))
    //    {
    //        isPlayerInRange = false;
    //        if (isPanelOpen)
    //        {
    //            panel.SetActive(false); // Đóng panel nếu player rời vùng
    //            isPanelOpen = false;
    //        }
    //    }
    //}
}
