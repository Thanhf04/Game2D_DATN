using Photon.Pun;
using UnityEngine;
public class SpawnPlayer : MonoBehaviour
{
    private Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3(-15, 5, 0);
        PhotonNetwork.Instantiate("Player1", position, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
    }
}