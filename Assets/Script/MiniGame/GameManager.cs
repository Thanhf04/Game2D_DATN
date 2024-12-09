using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject tokenPrefab; // Prefab của token
    [SerializeField] private GameObject Panel; // Panel chứa các thẻ bài
    [SerializeField] private GameObject noMoreTurnsPanel; // Panel thông báo hết lượt
    [SerializeField] private GameObject notificationPrefab; // Panel thông báo hết lượt
    private List<int> faceIndexes = new List<int> { 0, 1, 2, 3, 0, 1, 2, 3 };
    public static System.Random rnd = new System.Random(); // Để trộn danh sách
    private int[] visibleFaces = { -1, -2 }; // Các thẻ đang được lật lên
    private UI_Coin ui; // UI để thêm điểm
    public static bool isMiniGame = false;
    private int remainingTurns = 4; // Số lượt chơi còn lại
    public TextMeshProUGUI turn;
    int turnInt = 0;
    void Start()
    {
        ui = FindObjectOfType<UI_Coin>();
        InitializeGame(); // Gọi phương thức khởi tạo game
    }

    void InitializeGame()
    {
        // Kiểm tra xem còn lượt chơi hay không
        if (remainingTurns <= 0)
        {
            ShowNoMoreTurnsPanel();
            return;
        }

        float yPosition = 4f; // Tọa độ Y ban đầu
        float xPosition = -17.5f; // Tọa độ X ban đầu

        ClearTokens();

        int tokensToCreate = Mathf.Min(faceIndexes.Count, 8); // Đảm bảo không tạo nhiều hơn số lượng phần tử trong faceIndexes

        for (int i = 0; i < tokensToCreate; i++)
        {
            if (faceIndexes.Count == 0) break;

            int shuffleNum = rnd.Next(0, faceIndexes.Count);

            var temp = Instantiate(tokenPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
            temp.tag = "Token";
            temp.GetComponent<MainToken>().faceIndex = faceIndexes[shuffleNum];
            faceIndexes.RemoveAt(shuffleNum);

            xPosition += 2; // Tăng tọa độ X để tạo thẻ kế tiếp

            // Chuyển sang dòng mới sau khi tạo 4 thẻ trên cùng một hàng
            if (i == 3)
            {
                yPosition = 1f;
                xPosition = -17.5f;
            }
        }

        remainingTurns--; // Giảm số lượt chơi sau mỗi lần khởi tạo
    }

    // Kiểm tra xem có 2 thẻ đang được lật lên hay không
    public bool TwoCardsUp()
    {
        return visibleFaces[0] >= 0 && visibleFaces[1] >= 0;
    }

    public void AddVisibleFace(int index)
    {
        if (visibleFaces[0] == -1) visibleFaces[0] = index;
        else if (visibleFaces[1] == -2) visibleFaces[1] = index;
    }

    public void RemoveVisibleFace(int index)
    {
        if (visibleFaces[0] == index) visibleFaces[0] = -1;
        else if (visibleFaces[1] == index) visibleFaces[1] = -2;
    }

    public bool CheckMatch()
    {
        bool success = false;
        if (visibleFaces[0] == visibleFaces[1])
        {
            visibleFaces[0] = -1;
            visibleFaces[1] = -2;
            success = true;
            ui.AddCoins(10); // Thưởng người chơi khi ghép đúng
            turn.SetText("Lượt chơi: " + $"{turnInt + 1}" + "/2");
            turnInt = 1;
            CreateNotification("Bạn nhận được 10 vàng!");

        }
        return success;
    }

    public void ResetGame()
    {
        ClearTokens(); // Xóa tất cả các token
        faceIndexes = new List<int> { 0, 1, 2, 3, 0, 1, 2, 3 }; // Reset lại danh sách faceIndexes

        InitializeGame(); // Gọi lại phương thức khởi tạo game
    }

    private void ClearTokens()
    {
        var tokens = GameObject.FindGameObjectsWithTag("Token");
        foreach (var token in tokens)
        {
            Destroy(token);
        }
    }

    public void CloseGame()
    {
        ClearTokens();
    }

    public void ClosePanel1()
    {
        PanelManager.Instance.ClosePanel(Panel);
        ClearTokens();
        isMiniGame = false;
        NPC_Controller.isDialogue = false;
    }

    public void OpenPanel1()
    {
        isMiniGame = true;
        PanelManager.Instance.OpenPanel(Panel);
        ResetGame();
    }

    // Hiển thị thông báo hết lượt chơi
    private void ShowNoMoreTurnsPanel()
    {
        if (noMoreTurnsPanel != null)
        {
            noMoreTurnsPanel.SetActive(true);
        }
        Debug.Log("Bạn đã hết lượt chơi!");
    }
    private void CreateNotification(string message)
    {
        // Tạo một instance của prefab
        GameObject notificationInstance = Instantiate(notificationPrefab, new Vector3(238, 491, 0), Quaternion.identity);

        // Đảm bảo thông báo xuất hiện ở đúng vị trí trên UI (nếu cần)
        notificationInstance.transform.SetParent(this.transform, false);
        TextMeshProUGUI text = notificationInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = message; // Đặt nội dung thông báo
        }

        // Hủy thông báo sau 2 giây
        Destroy(notificationInstance, 1.5f);
    }
}
