using System.Collections;
using UnityEngine;

public class MainToken : MonoBehaviour
{
    GameObject gameManager;
    SpriteRenderer spriteRenderer;
    public Sprite[] faces;
    public Sprite back;
    public int faceIndex;
    public bool matched = false;

    public void OnMouseDown()
    {
        if (matched == false)
        {
            if (spriteRenderer.sprite == back)
            {
                if (gameManager.GetComponent<GameManager>().TwoCardsUp() == false)
                {
                    spriteRenderer.sprite = faces[faceIndex];
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
                spriteRenderer.sprite = back;
                gameManager.GetComponent<GameManager>().RemoveVisibleFace(faceIndex);
            }
        }
    }

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    IEnumerator WaitBeforeReset()
    {
        yield return new WaitForSeconds(1f);  // Wait for 1 second
        gameManager.GetComponent<GameManager>().ResetGame();  // Reset the game after the delay
    }
}