using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoQuanNang : MonoBehaviour
{
    public GameObject diemNang; // Đối tượng sẽ được nâng lên
    public float liftAmount; // Khoảng cách nâng lên theo trục Y
    public float liftSpeed; // Tốc độ nâng lên

    private Vector3 originalPosition; // Vị trí ban đầu của diemNang
    private Vector3 liftedPosition; // Vị trí nâng lên
    private int playerCollisionCount = 0; // Số lượng va chạm với "Player"

    void Start()
    {
        if (diemNang != null)
        {
            // Lưu vị trí ban đầu và tính toán vị trí nâng lên
            originalPosition = diemNang.transform.position;
            liftedPosition = originalPosition + new Vector3(0, liftAmount, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) // Kiểm tra nếu người chơi va chạm với vùng này
        {
            playerCollisionCount++; // Tăng số lượng va chạm
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) // Kiểm tra nếu người chơi rời khỏi vùng va chạm
        {
            playerCollisionCount--; // Giảm số lượng va chạm
        }
    }

    void Update()
    {
        if (diemNang != null)
        {
            // Di chuyển đối tượng đến vị trí nâng lên hoặc vị trí ban đầu
            if (playerCollisionCount > 0)
            {
                diemNang.transform.position = Vector3.MoveTowards(diemNang.transform.position, liftedPosition, liftSpeed * Time.deltaTime);
            }
            else
            {
                diemNang.transform.position = Vector3.MoveTowards(diemNang.transform.position, originalPosition, liftSpeed * Time.deltaTime);
            }
        }
    }
}
