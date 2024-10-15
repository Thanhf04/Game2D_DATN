using System.Collections;
using UnityEngine;

public class Dichuyennv2 : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 10f;
    public float rollDistance = 3f; // Đoạn lăn
    public float rollDuration = 0.5f; // Thời gian lăn
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator anim;
    private bool isRolling; // Trạng thái lăn

    public AudioSource music;
    public AudioSource playWalk;
    public AudioSource playAttack;
    // public AudioSource Die;
    public AudioSource playJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        music.Play();
        playAttack.Stop();
        playJump.Stop();
    }

    void Update()
    {

        float moveInput = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
            bool isRunning = moveInput != 0;
            anim.SetBool("isRunning", isRunning);
        }

        if (moveInput != 0 && isGrounded)
        {
            if (moveInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                if (!playWalk.isPlaying)
                {
                    playWalk.Play();

                }
            }
            else if (moveInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                if (!playWalk.isPlaying)
                {
                    playWalk.Play();
                }
            }
        }
        else
        {
            if (playWalk.isPlaying)
            {
                playWalk.Stop();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("isJump", true);
            anim.SetBool("isRunning", false);
            playAttack.Stop();
            if (playWalk.isPlaying)
            {
                playWalk.Stop();
            }
            playJump.Play();
        }

        if (Input.GetMouseButtonDown(0) && moveInput == 0 && isGrounded) // Nhấn chuột trái để tấn công
        {
            anim.SetBool("isAttack", true);
            playAttack.Play();

        }
        else if (Input.GetMouseButtonUp(0)) // Dừng animation tấn công khi nhả chuột trái
        {
            anim.SetBool("isAttack", false);
            playAttack.Stop();
        }

        if (Input.GetMouseButtonDown(1) && moveInput == 0 && isGrounded) // Nhấn chuột phải để tấn công thứ hai
        {
            anim.SetBool("isAttack2", true);
            playAttack.Play();
        }
        else if (Input.GetMouseButtonUp(1)) // Dừng animation tấn công thứ hai khi nhả chuột phải
        {
            anim.SetBool("isAttack2", false);
            playAttack.Stop();
        }

        if (Input.GetKeyDown(KeyCode.F) && isGrounded && !isRolling) // Nhấn phím F để lăn
        {
            StartCoroutine(Roll());
        }
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        anim.SetBool("isRoll", true); // Thiết lập trạng thái lăn

        float originalPosition = transform.position.x;
        float targetPosition = originalPosition + rollDistance * transform.localScale.x;

        float elapsedTime = 0f;
        while (elapsedTime < rollDuration)
        {
            float newPosX = Mathf.Lerp(originalPosition, targetPosition, (elapsedTime / rollDuration));
            rb.MovePosition(new Vector2(newPosX, rb.position.y)); // Di chuyển theo trục x
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        anim.SetBool("isRoll", false);
        isRolling = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = true;
            anim.SetBool("isJump", false);
            anim.SetBool("isRunning", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = false;
        }
    }
}
