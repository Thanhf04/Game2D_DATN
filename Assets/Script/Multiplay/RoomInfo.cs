using TMPro;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public TMP_Text roomCodeText; // Kéo TMP_Text này vào từ Inspector

    void Start()
    {
        // Lấy mã phòng từ PlayerPrefs
        if (PlayerPrefs.HasKey("RoomCode"))
        {
            roomCodeText.text = PlayerPrefs.GetString("RoomCode");
        }
        else
        {
            roomCodeText.text = "Room Code not found.";
        }
    }
}
