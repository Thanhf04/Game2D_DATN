using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainToken : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] faces; // Các mặt của thẻ bài
    public Sprite back;    // Mặt sau của thẻ bài
    public int faceIndex;  // Chỉ số của mặt thẻ
    public bool matched = false;

    private Image imageRenderer;
    private GameManager gameManager;

    private void Awake()
    {
        imageRenderer = GetComponent<Image>(); // Lấy thành phần Image từ GameObject
        gameManager = FindObjectOfType<GameManager>();
        imageRenderer.sprite = back; // Đặt hình ảnh khởi tạo là mặt sau
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (matched || gameManager.TwoCardsUp()) return;

        //if (imageRenderer.sprite == back)
        //{
        //    // Lật mặt thẻ
        //    imageRenderer.sprite = faces[faceIndex];
        //    gameManager.AddVisibleFace(faceIndex);

        //    // Kiểm tra ghép đúng
        //    //if (gameManager.CheckMatch())
        //    //{
        //    //    matched = true;
        //    //    Debug.Log("Matched!");
        //    //}
        //}
        //else
        //{
        //    // Lật về mặt sau
        //    imageRenderer.sprite = back;
        //    gameManager.RemoveVisibleFace(faceIndex);
        //}
        if (matched == false)
        {
            if (imageRenderer.sprite == back)
            {
                if (gameManager.GetComponent<GameManager>().TwoCardsUp() == false)
                {
                    imageRenderer.sprite = faces[faceIndex];
                    gameManager.GetComponent<GameManager>().AddVisibleFace(faceIndex);
                    matched = gameManager.GetComponent<GameManager>().CheckMatch();
                    if (matched)
                    {
                        StartCoroutine(WaitBeforeReset());
                    }
                }
            }
            else
            {
                imageRenderer.sprite = back;
                gameManager.GetComponent<GameManager>().RemoveVisibleFace(faceIndex);
                //gameManager.GetComponent<GameManager>().ResetGame();
            }
        }
        IEnumerator WaitBeforeReset()
        {
            yield return new WaitForSeconds(1f);  // Wait for 1 second
            gameManager.GetComponent<GameManager>().ResetGame();  // Reset the game after the delay
        }
    }
}
