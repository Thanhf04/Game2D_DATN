using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenxepHinh : MonoBehaviour
{
    public GameObject minigame; // GameObject của mini game
    private bool isPlayerInRange = false; // Cờ kiểm tra xem player có trong vùng va chạm không
    private bool isMinigameOpened = false; // Cờ kiểm tra xem mini game đã được mở chưa

    void Start()
    {
        minigame.SetActive(false); // Đảm bảo mini game ban đầu bị tắt
    }

    void Update()
    {
        // Kiểm tra nếu player ở trong vùng va chạm, nhấn phím F và mini game chưa mở
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !isMinigameOpened)
        {
            minigame.SetActive(true); // Bật mini game
            isMinigameOpened = true; // Đánh dấu là mini game đã mở
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

    private void OnTriggerExit2D(Collider2D other)
    {
        // Tắt cờ khi player rời khỏi vùng va chạm
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
