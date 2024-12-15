using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainToken : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] faces; // Các mặt của thẻ bài
    public Sprite back; // Mặt sau của thẻ bài
    public int faceIndex; // Chỉ số của mặt thẻ
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
        // Nếu thẻ đã khớp hoặc đang lật lên, không làm gì cả
        if (matched || imageRenderer.sprite != back)
            return;

        // Lật thẻ
        imageRenderer.sprite = faces[faceIndex];
        gameManager.AddVisibleFace(faceIndex);

        // Kiểm tra nếu cả 2 thẻ đã được lật
        if (gameManager.TwoCardsUp())
        {
            bool match = gameManager.CheckMatch();
            if (!match)
            {
                // Nếu không khớp, úp lại sau một khoảng thời gian
                StartCoroutine(FlipBackAfterDelay());
            }
            else
            {
                // Nếu khớp, đánh dấu thẻ là đã khớp
                matched = true;
                StartCoroutine(WaitBeforeReset());
            }
        }
        IEnumerator WaitBeforeReset()
        {
            yield return new WaitForSeconds(1.0f); // Wait for 1 second
            gameManager.GetComponent<GameManager>().ResetGame(); // Reset the game after the delay
        }
    }

    public void FlipDown()
    {
        imageRenderer.sprite = back; // Đặt lại hình ảnh là mặt sau
    }

    private IEnumerator FlipBackAfterDelay()
    {
        yield return new WaitForSeconds(1.0f); // Chờ 1 giây

        // Úp lại thẻ không khớp
        gameManager.ResetVisibleFaces();
    }
}
