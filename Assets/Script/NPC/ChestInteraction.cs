using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public GameObject swordPrefab;   // Thanh kiếm sẽ xuất hiện
    public Transform spawnPoint;     // Điểm xuất hiện kiếm
    private bool isPlayerNearby = false; // Biến kiểm tra người chơi có gần rương không

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F)) // Thay chuột trái bằng phím F
        {
            OpenChest(); // Mở rương
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // Dùng 2D Collider
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player vào vùng tương tác với rương.");
        }
    }

    private void OnTriggerExit2D(Collider2D other) // Dùng 2D Collider
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player rời vùng tương tác với rương.");
        }
    }

    public void OpenChest()
    {
        Debug.Log("Rương được mở!");
        gameObject.SetActive(false); // Ẩn rương

        // Kiểm tra và tạo kiếm
        if (swordPrefab != null && spawnPoint != null)
        {
            Instantiate(swordPrefab, spawnPoint.position, spawnPoint.rotation); // Tạo kiếm tại spawn point
            Debug.Log("Kiếm đã xuất hiện tại " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("Prefab hoặc Spawn Point bị thiếu!");
        }
    }
}
