using UnityEngine;
using UnityEngine.SceneManagement;

public class Instruct_Player : MonoBehaviour
{
    [SerializeField] GameObject HealthBar;
    [SerializeField] GameObject Control;
    [SerializeField] GameObject Skill;
    [SerializeField] GameObject Enemy;
    [SerializeField] GameObject End;

    private int step = 0;
    // Start is called before the first frame update
    void Start()
    {
        ShowStep(step);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShowStep(step);
            step++;
        }

    }
    private void ShowStep(int step)
    {
        switch (step)
        {
            case 0:
                Control.SetActive(true);
                break;
            case 1:
                HealthBar.SetActive(true);
                Control.SetActive(false);
                break;
            case 2:
                Enemy.SetActive(true);
                HealthBar.SetActive(false);

                break;
            case 3:
                Skill.SetActive(true);
                Enemy.SetActive(false);

                break;
            case 4:
                End.SetActive(true);
                Skill.SetActive(false);

                break;
            default:
                End.SetActive(false);

                MainSreen();
                break;
        }
    }
    private void MainSreen()
    {
        SceneManager.LoadScene("Player1");
    }

}
