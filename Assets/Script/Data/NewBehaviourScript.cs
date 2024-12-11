using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    Gamesapxep gamesapxepa;

    void Start()
    {
        gamesapxepa = FindObjectOfType<Gamesapxep>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && gamesapxepa.completionCount > 1)
        {
            SceneManager.LoadScene("SceneBoss");
        }
    }
}
