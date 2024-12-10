using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public gamesapxep gamesapxep;

    // Start is called before the first frame update
    void Start()
    {
        // Lấy component gamesapxep từ cùng GameObject
        gamesapxep = GetComponent<gamesapxep>();

        // Nếu gamesapxep không được gán, báo lỗi
        if (gamesapxep == null)
        {
            Debug.LogError("gamesapxep script is not attached to the GameObject!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu completionCount bằng 1 thì tải scene mới
        if (gamesapxep != null && gamesapxep.completionCount == 1)
        {
            SceneManager.LoadScene("Player2");
        }
    }
}
