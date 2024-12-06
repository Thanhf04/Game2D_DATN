using UnityEngine;

public class AppleItem : MonoBehaviour
{
    public NPCAppleArmorQuest npcApple;  

    void Start()
    {
        if (npcApple == null)
        {
            npcApple = FindObjectOfType<NPCAppleArmorQuest>(); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng va chạm có tag "Player" không
        if (other.CompareTag("Player"))
        {
            // Tăng số táo trong NPCQuest
            if (npcApple != null)
            {
                npcApple    .CollectApple();  // Gọi phương thức để tăng số táo
            }

            // Xóa quả táo sau khi thu thập
            Destroy(gameObject);  // Xóa đối tượng quả táo khỏi scene
        }
    }
}
