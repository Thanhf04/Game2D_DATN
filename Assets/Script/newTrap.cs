using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class newTrap : MonoBehaviour
{
    public float tocDoXoay =5f;
    public float tocDoDiChuyen =5f;
    public Transform diemA;
    public Transform diemB;
    private Vector3 diemMucTieu;
    void Start()
    {
        diemMucTieu = diemA.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, diemMucTieu, tocDoDiChuyen * Time.deltaTime);
        if(Vector3.Distance(transform.position, diemMucTieu)< 0.1f){
            if(transform.position == diemA.position){
                diemMucTieu = diemB.position;
            }else{
                diemMucTieu = diemA.position;
            }
        }
    }

    private void FixedUpdate(){
        transform.Rotate(0,0,tocDoXoay);
    }
    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Player")){
            SceneManager.LoadScene("Player1");
        }
    }
}
