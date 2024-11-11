using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
   public float changTime;
   public string sceneName;

    // Update is called once per frame
    void Update()
    {
        changTime -= Time.deltaTime;
        if(changTime <= 0){
            SceneManager.LoadScene(sceneName);
        }
    }
}
