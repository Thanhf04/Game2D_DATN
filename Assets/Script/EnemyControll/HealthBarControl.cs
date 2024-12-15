using UnityEngine;

public class HealthBarControl : MonoBehaviour
{
    public Transform enemy; // Đối tượng quái vật

    void LateUpdate()
    {
        // Lấy scale hiện tại của quái vật
        Vector3 enemyScale = enemy.localScale;

        // Cố định scale của thanh máu theo hướng X
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * Mathf.Sign(enemyScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
}
