using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamesesion : MonoBehaviour
{
    public static Gamesesion Instance;

    // Các chỉ số cần lưu


    // Túi đồ
    public List<string> inventory = new List<string>();

    private void Awake()
    {
        // Đảm bảo GameManager không bị trùng khi chuyển scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Không bị hủy khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
