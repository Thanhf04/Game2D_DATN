using UnityEngine;

public class ArrowGuide : MonoBehaviour
{
    public Transform player; // Vị trí của người chơi
    public Transform target; // Vị trí của vật phẩm nhiệm vụ
    public RectTransform arrow; // Mũi tên trong Canvas
    public float offsetDistance; // Khoảng cách mũi tên cách rìa màn hình

    void Update()
    {
        if (target != null)
        {
            // Tính hướng từ người chơi đến mục tiêu
            Vector3 direction = target.position - player.position;

            // Tính góc quay của mũi tên
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0, 0, -angle);

            // Giới hạn vị trí mũi tên ở rìa màn hình
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
            screenPosition.x = Mathf.Clamp(screenPosition.x, offsetDistance, Screen.width - offsetDistance);
            screenPosition.y = Mathf.Clamp(screenPosition.y, offsetDistance, Screen.height - offsetDistance);

            arrow.position = screenPosition;
        }
        else
        {
            // Ẩn mũi tên nếu không có mục tiêu
            arrow.gameObject.SetActive(false);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        arrow.gameObject.SetActive(newTarget != null);
    }
}
