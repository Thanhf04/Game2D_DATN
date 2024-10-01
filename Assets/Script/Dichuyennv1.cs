using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dichuyennv1 : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    public float jump = 10f;
    private bool isGrounded;
    private bool isRunning;
    private bool isRoll;
    private bool isJump;
    private Animator anim;

    public float rollDistance = 3f;
    public float rollDuration = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isRunning = false;
        isRoll = false;
        isJump = false;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (isGrounded) 
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
            isRunning = moveInput != 0;
            anim.SetBool("isRunning", isRunning);
        } 
        else 
        {
            isRunning = false;
            anim.SetBool("isRunning", false);
        }

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            isGrounded = false;
            isJump = true;
            anim.SetBool("isJump", true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isJump)
            {
                anim.SetBool("isAttack2", true); // Kích hoạt animation tấn công khi nhảy
                rb.velocity = new Vector2(rb.velocity.x, -10f); // Tăng tốc độ rơi xuống
            }
            else
            {
                StartCoroutine(Attack());
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !isRoll)
        {
            StartCoroutine(Roll());
        }
    }

    private IEnumerator Attack()
    {
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.25f);
        anim.SetBool("isAttack", false);
    }

    private IEnumerator Roll()
    {
        isRoll = true;
        anim.SetBool("isRoll", true);

        float originalPosition = transform.position.x;
        float targetPosition = originalPosition + rollDistance * transform.localScale.x;

        float elapsedTime = 0f;
        while (elapsedTime < rollDuration)
        {
            float newPosX = Mathf.Lerp(originalPosition, targetPosition, (elapsedTime / rollDuration));
            rb.MovePosition(new Vector2(newPosX, rb.position.y)); // Chỉ thay đổi vận tốc x
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        anim.SetBool("isRoll", false);
        isRoll = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = true;
            isJump = false;
            anim.SetBool("isJump", false);
            anim.SetBool("isAttack2", false); // Dừng animation tấn công khi nhảy
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
