using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tokenPrefab; // Prefab của Token

    [SerializeField]
    private GameObject panel; // Panel chứa các Token

    [SerializeField]
    private GameObject button;
    private List<int> faceIndexes = new List<int> { 0, 1, 2, 3, 0, 1, 2, 3 };
    public static System.Random rnd = new System.Random(); // Để trộn danh sách
    private int[] visibleFaces = { -1, -2 }; // Các thẻ đang được lật lên
    public static bool isMiniGame = false;
    public GameObject parentTextNotification;
    public TextMeshProUGUI textNotification;
    UI_Coin ui;
    Quest_3 q3;

    void Start()
    {
        InitializeGame(); // Khởi tạo game
        ui = FindObjectOfType<UI_Coin>();
        q3 = FindObjectOfType<Quest_3>();
        textNotification.text = "";
    }

    private void InitializeGame()
    {
        ClearTokens(); // Xóa Token cũ nếu có

        for (int i = 0; i < 8; i++) // Tạo 8 token
        {
            if (faceIndexes.Count == 0)
                break;

            int shuffleNum = rnd.Next(0, faceIndexes.Count);

            GameObject token = Instantiate(tokenPrefab);
            token.transform.SetParent(panel.transform, false); // Đặt Token vào Panel

            // Gán faceIndex cho token
            token.GetComponent<MainToken>().faceIndex = faceIndexes[shuffleNum];
            faceIndexes.RemoveAt(shuffleNum);
        }
    }

    public bool TwoCardsUp()
    {
        return visibleFaces[0] >= 0 && visibleFaces[1] >= 0;
    }

    public void AddVisibleFace(int index)
    {
        if (visibleFaces[0] == -1)
            visibleFaces[0] = index;
        else if (visibleFaces[1] == -2)
            visibleFaces[1] = index;
    }

    public void RemoveVisibleFace(int index)
    {
        if (visibleFaces[0] == index)
            visibleFaces[0] = -1;
        else if (visibleFaces[1] == index)
            visibleFaces[1] = -2;
    }

    public bool CheckMatch()
    {
        bool success = false;
        if (visibleFaces[0] == visibleFaces[1])
        {
            visibleFaces[0] = -1;
            visibleFaces[1] = -2;
            success = true;
            q3.CompleteQuestCard();
            parentTextNotification.SetActive(true);
            textNotification.text = "Thẻ trùng nhau";
            textNotification.color = Color.yellow;
            StartCoroutine(StartNotification());
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
        textNotification.text = "";
    }

    public void OpenMiniGame()
    {
        PanelManager.Instance.OpenPanel(panel);
        isMiniGame = true;
        button.SetActive(true);
        parentTextNotification.SetActive(true);
        ResetGame();
    }

    public void CloseMiniGame()
    {

        PanelManager.Instance.ClosePanel(panel);
        ResetGame();
        button.SetActive(false);
        isMiniGame = false;
        parentTextNotification.SetActive(false);
        //NPC_Controller.isDialogue = false;
    }

    public void ResetVisibleFaces()
    {
        foreach (Transform child in panel.transform)
        {
            MainToken token = child.GetComponent<MainToken>();
            if (
                token != null
                && !token.matched
                && (token.faceIndex == visibleFaces[0] || token.faceIndex == visibleFaces[1])
            )
            {
                token.FlipDown(); // Úp lại thẻ
                parentTextNotification.SetActive(true);
                textNotification.text = "Thẻ không trùng nhau";
                textNotification.color = Color.red;
                StartCoroutine(StartNotification());
            }
        }

        visibleFaces[0] = -1;
        visibleFaces[1] = -2;
    }

    IEnumerator StartNotification()
    {
        yield return new WaitForSeconds(0.7f);
        parentTextNotification.SetActive(false);
    }
}
