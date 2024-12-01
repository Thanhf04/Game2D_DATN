using UnityEngine;

public class HealthBarControl : MonoBehaviour
{
    private Transform parentTransform;

    void Start()
    {
        // Lấy transform của đối tượng cha (thường là quái vật)
        parentTransform = transform.parent;
    }

    void Update()
    {
        if (parentTransform != null)
        {
            // Đảm bảo thanh máu không bị xoay khi quái vật lật
            transform.rotation = Quaternion.identity;  // Đặt rotation của thanh máu về zero
        }
    }
}
