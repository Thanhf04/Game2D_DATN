//using UnityEngine;
//using UnityEngine.SceneManagement; // Thư viện để chuyển cảnh

//public class SceneController : MonoBehaviour
//{
//    // Tham chiếu đến GameObject mà bạn muốn di chuyển
//    public GameObject objectToMove;

//    // Vị trí mà GameObject sẽ di chuyển đến
//    private Vector3 targetPosition = new Vector3(175, 2, 0);

//    // Hàm này sẽ chuyển cảnh và đặt vị trí của đối tượng
//    public void GoToPlayer1Scene()
//    {
//        // Chuyển sang Scene có tên "Player1"
//        SceneManager.LoadScene("Player1");

//        // Đặt vị trí của đối tượng này (hoặc đối tượng cụ thể bạn muốn) ở vị trí (175, 2, 0)
//        if (objectToMove != null)
//        {
//            objectToMove.transform.position = targetPosition;
//        }
//    }
//}
