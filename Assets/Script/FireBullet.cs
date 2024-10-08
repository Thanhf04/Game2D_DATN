using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float initialPositionX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Lưu vị trí ban đầu để so sánh sau này
        initialPositionX = transform.position.x;
        anim.SetBool("isSkill1", true); // Kích hoạt animation khi đạn được bắn ra
    }

    void Update()
    {
        // Kiểm tra xem vị trí X đã thay đổi hay chưa
        if (transform.position.x != initialPositionX)
        {
            anim.SetBool("isSkill1", true); // Chạy animation khi vị trí X thay đổi
        }
    }

    private void OnDestroy()
    {
        anim.SetBool("isSkill1", false); // Tắt animation khi đạn bị hủy
    }

    public void SetDirection(Vector2 direction)
    {
        rb.velocity = direction; // Thiết lập vận tốc cho đạn dựa trên hướng
    }
}
