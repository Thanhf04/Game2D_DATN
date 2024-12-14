using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    public void ResetScene1()
    {
        SceneManager.LoadScene("Player1");
    }
}
