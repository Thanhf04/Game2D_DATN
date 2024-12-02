using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayer : MonoBehaviour
{
    private Vector3 position;
    
    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3(-15, 5, 0);
        PhotonNetwork.Instantiate("Player1", position,Quaternion.identity);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
