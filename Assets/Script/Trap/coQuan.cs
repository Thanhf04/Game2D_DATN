using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coQuan : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject door;
    
    private void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.CompareTag("Player")){
            door.SetActive(false);
        }else
        {
            door.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D other){
        if (other.gameObject.CompareTag("Player")){
            door.SetActive(true);
        }
    }
}
