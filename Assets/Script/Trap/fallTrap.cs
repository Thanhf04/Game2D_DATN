using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallTrap : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool daRoi = false;
    public Transform diemKhoiPhuc;
    private SpriteRenderer sr;
    public float khoangCachHienThi = 5f; // Khoảng cách để bẫy hiện lên
    private Transform playerTransform;
    private Collider2D damageCollider; // Collider để gây sát thương

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false; // Ẩn bẫy lúc đầu
        rb.isKinematic = true; // Đảm bảo bẫy không rơi ngay từ đầu

        // Lấy damage collider (BoxCollider2D) để gây sát thương
        damageCollider = GetComponent<BoxCollider2D>();
        damageCollider.enabled = false; // Vô hiệu hóa lúc đầu

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null && !daRoi)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance <= khoangCachHienThi)
            {
                sr.enabled = true; // Hiện bẫy khi người chơi đến gần
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !daRoi)
        {
            rb.isKinematic = false;
            daRoi = true;
            damageCollider.enabled = true; // Kích hoạt vùng sát thương
            Invoke("KhoiPhuc", 3f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && daRoi)
        {
            // Gây sát thương cho người chơi nếu vùng sát thương chạm
            Dichuyennv1 playerHealth = collision.collider.GetComponent<Dichuyennv1>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20); // Gây 20 sát thương
            }
        }
    }

    private void KhoiPhuc()
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularDrag = 0;
        transform.position = diemKhoiPhuc.position;
        daRoi = false;
        sr.enabled = false; // Ẩn bẫy sau khi phục hồi
        damageCollider.enabled = false; // Vô hiệu hóa vùng sát thương
    }
}
