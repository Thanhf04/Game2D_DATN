using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")//va cham voi player
        {
            StartCoroutine(LoadNextLevel());//goi ham voi do tre
        }
    }
    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(1f);//tre 1 giay
        //lay index cua scene hien tai
        int currentindex = SceneManager.GetActiveScene().buildIndex;
        int nextindex = currentindex + 1; //scene tiep theo

        //vuot khoi so scene dang co
        if(nextindex==SceneManager.sceneCountInBuildSettings)
            nextindex = 3;//hoac xu ly ket thuc game...
        SceneManager.LoadScene(nextindex);
    }
}