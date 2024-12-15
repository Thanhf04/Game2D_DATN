using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Saveloadgame : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        // Lưu lại tất cả dữ liệu trước khi chuyển scene
        PlayerStats.Instance.SaveStats();

        // Chuyển scene
        SceneManager.LoadScene(sceneName);
    }
}
