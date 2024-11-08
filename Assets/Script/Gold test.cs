using UnityEngine;
using TMPro; // Đừng quên thêm namespace cho TextMesh Pro

public class GoldManager : MonoBehaviour
{
    public TMP_Text goldText; // Tham chiếu đến TextMeshPro Text
    private int goldAmount; // Số lượng vàng hiện tại

    void Start()
    {
        goldAmount = 0; // Khởi tạo vàng bằng 0
        UpdateGoldText();
    }

    void Update()
    {
        // Kiểm tra phím A để tăng vàng
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddGold(1); // Tăng 1 vàng
        }
        // Kiểm tra phím B để giảm vàng
        if (Input.GetKeyDown(KeyCode.B))
        {
            RemoveGold(1); // Giảm 1 vàng
        }
    }

    public void AddGold(int amount)
    {
        goldAmount += amount;
        UpdateGoldText();
    }

    public void RemoveGold(int amount)
    {
        goldAmount -= amount;
        if (goldAmount < 0) // Đảm bảo vàng không âm
        {
            goldAmount = 0;
        }
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        goldText.text = " " + goldAmount; // Cập nhật văn bản
    }
}
