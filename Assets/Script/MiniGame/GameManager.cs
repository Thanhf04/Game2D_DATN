using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject tokenPrefab; // Prefab của Token
    [SerializeField] private GameObject panel;       // Panel chứa các Token
    [SerializeField] private TextMeshProUGUI currentTurn; // Hiển thị lượt chơi
    [SerializeField] private GameObject outOfTurn;
    [SerializeField] private GameObject button;
    private List<int> faceIndexes = new List<int> { 0, 1, 2, 3, 0, 1, 2, 3 };
    public static System.Random rnd = new System.Random(); // Để trộn danh sách
    private int[] visibleFaces = { -1, -2 }; // Các thẻ đang được lật lên
    private int remainingTurns = 4;         // Số lượt chơi còn lại
    public static bool isMiniGame = false;
    UI_Coin ui;
    public NPCAppleArmorQuest npcQuest;
    void Start()
    {

        InitializeGame(); // Khởi tạo game
        ui = FindObjectOfType<UI_Coin>();
        npcQuest = FindObjectOfType<NPCAppleArmorQuest>();
    }

    private void InitializeGame()
    {
        ClearTokens(); // Xóa Token cũ nếu có

        if (remainingTurns <= 0)
        {
            outOfTurn.SetActive(true);
            return;
        }

        for (int i = 0; i < 8; i++) // Tạo 8 token
        {
            if (faceIndexes.Count == 0) break;

            int shuffleNum = rnd.Next(0, faceIndexes.Count);

            GameObject token = Instantiate(tokenPrefab);
            token.transform.SetParent(panel.transform, false); // Đặt Token vào Panel


            // Gán faceIndex cho token
            token.GetComponent<MainToken>().faceIndex = faceIndexes[shuffleNum];
            faceIndexes.RemoveAt(shuffleNum);
        }
        UpdateTurnText();
    }
    private void UpdateTurnText()
    {
        remainingTurns--;
        // currentTurn.text = $"Lượt chơi: {remainingTurns}/4";
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
            ui.AddCoins(10);
            // npcQuest.CurrentCard();
        }
        return success;
    }

    private void ClearTokens()
    {
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ResetGame()
    {
        ClearTokens();
        faceIndexes = new List<int> { 0, 1, 2, 3, 0, 1, 2, 3 };
        InitializeGame();
    }
    public void OpenMiniGame()
    {
        PanelManager.Instance.OpenPanel(panel);
        isMiniGame = true;
        button.SetActive(true);
        ResetGame();
    }
    public void CloseMiniGame()
    {
        PanelManager.Instance.ClosePanel(panel);
        ClearTokens();
        button.SetActive(false);
        isMiniGame = false;
        NPC_Controller.isDialogue = false;
        outOfTurn.SetActive(false);
    }
}
