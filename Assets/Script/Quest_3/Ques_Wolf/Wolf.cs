using UnityEngine;

public class Wolf : MonoBehaviour
{
    public Transform player; // Nhân vật chính
    public float followDistance = 1f; // Khoảng cách tối thiểu để bắt đầu đi theo
    public float speed = 2f; // Tốc độ di chuyển
    private SpriteRenderer spriteRenderer; // Để lật hướng
    private Animator animator; // Để điều khiển Animation

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > followDistance)
        {
            // Di chuyển về phía nhân vật chính
            Vector3 direction = player.position - transform.position;
            direction.y = -3.65f;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            // Quay mặt theo hướng di chuyển
            if (direction.x > 0)
                spriteRenderer.flipX = false; // Hướng về bên phải
            else if (direction.x < 0)
                spriteRenderer.flipX = true; // Hướng về bên trái
            // Bật animation đi bộ
            animator.SetBool("Walk", true);
        }
        else
        {
            // Tắt animation đi bộ khi đứng yên
            animator.SetBool("Walk", false);
        }
    }

}
