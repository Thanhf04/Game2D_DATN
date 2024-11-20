using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Hàm chuyển màn, truyền vào tên màn hoặc ID của màn
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Dùng để thoát khỏi trò chơi (khi build ra file exe)
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
