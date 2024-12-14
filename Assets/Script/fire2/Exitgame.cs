using UnityEngine;

public class ExitGameScript : MonoBehaviour
{
    
    public void ExitGame()
    {
     
#if UNITY_EDITOR
            // Nếu đang chạy trong Unity Editor, chỉ dừng Play Mode
            UnityEditor.EditorApplication.isPlaying = false;
#else
       
        Application.Quit();
#endif
    }
}
