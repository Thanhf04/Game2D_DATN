using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject tokenPrefab; // Prefab của token
    public GameObject Panel; // Panel chứa các thẻ bài

    private List<int> faceIndexes = new List<int> { 0, 1, 2, 3, 0, 1, 2, 3 };
    public static System.Random rnd = new System.Random(); // Để trộn danh sách
    private int[] visibleFaces = { -1, -2 }; // Các thẻ đang được lật lên
    private UI_Coin ui; // UI để thêm điểm

    void Start()
    {
        ui = FindObjectOfType<UI_Coin>();
        InitializeGame(); // Gọi phương thức khởi tạo game
    }

    void InitializeGame()
    {

        float yPosition = 4f; // Tọa độ Y ban đầu
        float xPosition = -14.7f; // Tọa độ X ban đầu

        ClearTokens();

        // Kiểm tra số lượng phần tử trong faceIndexes trước khi tạo token
        int tokensToCreate = Mathf.Min(faceIndexes.Count, 8); // Đảm bảo không tạo nhiều hơn số lượng phần tử có trong faceIndexes

        for (int i = 0; i < tokensToCreate; i++)
        {
            if (faceIndexes.Count == 0) break; // Kiểm tra nếu danh sách faceIndexes đã hết phần tử

            int shuffleNum = rnd.Next(0, faceIndexes.Count); // Khai báo và lấy chỉ số ngẫu nhiên từ faceIndexes

            if (shuffleNum < 0 || shuffleNum >= faceIndexes.Count) continue; // Đảm bảo shuffleNum hợp lệ

            var temp = Instantiate(tokenPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
            temp.tag = "Token";
            temp.GetComponent<MainToken>().faceIndex = faceIndexes[shuffleNum];

            // Sau khi sử dụng chỉ số, xóa phần tử khỏi faceIndexes để không tạo lại token giống nhau
            faceIndexes.RemoveAt(shuffleNum);

            xPosition += 2; // Tăng tọa độ X để tạo thẻ kế tiếp

            // Nếu đã tạo đủ số thẻ trên 1 dòng, chuyển sang dòng mới
            if (i == 3)
            {
                yPosition = 1f;
                xPosition = -14.7f;
            }
        }
    }
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
            ui.AddCoins(10); // Thưởng người chơi khi có một cặp bài
        }
        return success;
    }

    public void ResetGame()
    {
        ClearTokens(); // Xóa tất cả các token
        faceIndexes = new List<int> { 0, 1, 2, 3, 0, 1, 2, 3 }; // Tạo lại danh sách faceIndexes
        InitializeGame(); // Gọi lại phương thức khởi tạo game
    }

    // Xóa tất cả các token trong game
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
        ClearTokens(); // Xóa tất cả các token khi đóng game
    }

    public void ClosePanel1()
    {
        PanelManager.Instance.ClosePanel(Panel); // Đóng panel
        ClearTokens();
    }

    public void OpenPanel1()
    {
        PanelManager.Instance.OpenPanel(Panel); // Mở panel
        ResetGame(); // Khởi động lại game
    }
}
