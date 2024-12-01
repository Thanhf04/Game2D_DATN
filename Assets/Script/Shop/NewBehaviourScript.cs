using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform target; // Tham chiếu tới đối tượng cần theo dõi
    public float fixedY; // Trục Y cố định

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.y = fixedY; // Giữ nguyên giá trị trục Y
            transform.position = newPosition;
        }
    }
}
