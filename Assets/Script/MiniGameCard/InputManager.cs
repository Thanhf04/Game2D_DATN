using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static event System.Action OnFPressed; // Sự kiện khi nhấn F

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnFPressed?.Invoke(); // Kích hoạt sự kiện khi nhấn phím F
        }
    }
}
