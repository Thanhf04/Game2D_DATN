using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openpenal : MonoBehaviour
{
    // Reference đến panel
    public GameObject panel;

    // Biến kiểm tra nhân vật có trong vùng va chạm không
    private bool isPlayerInRange = false;

    private void Start() {
        // Ẩn panel khi bắt đầu game
        panel.SetActive(false);
    }

    private void Update() {
        // Kiểm tra nếu nhân vật vào vùng va chạm và nhấn phím F
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F)) {
            // Mở panel khi nhấn phím F
            panel.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Kiểm tra nếu đối tượng vào vùng va chạm là nhân vật
        if (other.CompareTag("Player")) {
            isPlayerInRange = true; // Đánh dấu là nhân vật đang trong vùng va chạm
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        // Kiểm tra nếu đối tượng ra khỏi vùng va chạm là nhân vật
        if (other.CompareTag("Player")) {
            isPlayerInRange = false; // Đánh dấu là nhân vật đã ra khỏi vùng va chạm
        }
    }
}
