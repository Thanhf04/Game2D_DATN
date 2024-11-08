using UnityEngine;

public class ColliderSkill_DestroySkill : MonoBehaviour
{
    public GameObject destroySkillPrefab; // Prefab của DestroySkill
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu va chạm với player
        if (other.CompareTag("Player"))
        {
            // Tạo destroySkill tại vị trí hiện tại của skill
            if (destroySkillPrefab != null)
            {
                GameObject destroySkillEffect = Instantiate(destroySkillPrefab, transform.position, Quaternion.identity);
                Destroy(destroySkillEffect, 0.3f);
            }

            // Hủy skill hiện tại sau khi tạo destroySkill
            Destroy(gameObject);

        }
    }
}
