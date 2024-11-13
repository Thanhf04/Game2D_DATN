using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter triggered by {other.name} with tag {other.tag}"); // Debug toàn bộ thông tin về va chạm.

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã rơi vào Dead Zone! Game Over!");
            GameOver(other.gameObject);
        }
    }

    private void GameOver(GameObject player)
    {
        Debug.Log("Player is dead");
        Destroy(player); // Phá hủy Player
    }
}
