using UnityEngine;

public class Collider_Wolf : MonoBehaviour
{
    public GameObject Wolf;
    Quest_3 q3;
    private void Start()
    {
        q3 = FindObjectOfType<Quest_3>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wolf"))
        {
            q3.CompleteWolf();
            Wolf.SetActive(false);
        }
    }
}
