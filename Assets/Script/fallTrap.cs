using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallTrap : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool daRoi = false;
    public Transform diemKhoiPhuc;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collider){
        if(collider.CompareTag("Player")&&!daRoi){
            rb.isKinematic = false;
            daRoi=true;
            Invoke("KhoiPhuc", 2f);
        }
    }
    private  void KhoiPhuc(){
        rb.isKinematic = true;
        rb.velocity =Vector2.zero;
        rb.angularDrag = 0;
        transform.position = diemKhoiPhuc.position;
        daRoi = false;
    }
}
