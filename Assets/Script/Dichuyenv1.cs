using System.Collections;
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

    public GameObject fireBulletPrefab;
    public GameObject fireBreathPrefab;
    public GameObject fireHandPrefab;
    public Transform firePoint;
    public Transform firePoint2;
    public Transform firePoint3;
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 2f;

    public AudioSource music;
    public AudioSource playWalk;
    public AudioSource playAttack;
    public AudioSource playAttack2;
    public AudioSource playAttack_Fire1;
    public AudioSource playAttack_Fire2;
    public AudioSource playAttack_Fire3;

    // public AudioSource Die;
    public AudioSource playJump;

    public int maxHealth = 100;
    public int currentHealth;

    private GameObject currentFireBreath;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isRunning = false;
        isRoll = false;
        isJump = false;
        music.Play();
        playAttack.Stop();
        playJump.Stop();
        playAttack_Fire1.Stop();
        playAttack_Fire2.Stop();
        playAttack_Fire3.Stop();
        playAttack2.Stop();
        currentHealth = maxHealth;
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
            isJump = true;
            anim.SetBool("isJump", true);
            playJump.Play();
            playWalk.Stop();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isJump)
            {
                anim.SetBool("isAttack2", true);
                rb.velocity = new Vector2(rb.velocity.x, -10f);
                playAttack2.Play();
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShootFireBullet();

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            BreathFire();

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            FireHand();

        }

        // Cập nhật vị trí của lửa nếu đang phun lửa
        if (currentFireBreath != null)
        {
            currentFireBreath.transform.position = firePoint2.position;
            currentFireBreath.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        }
    }

    private IEnumerator Attack()
    {
        anim.SetBool("isAttack", true);
        playAttack.Play();
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
            float newPosX = Mathf.Lerp(
                originalPosition,
                targetPosition,
                (elapsedTime / rollDuration)
            );
            rb.MovePosition(new Vector2(newPosX, rb.position.y));
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
            anim.SetBool("isAttack2", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = false;
        }
    }

    void ShootFireBullet()
    {
        GameObject bullet = Instantiate(fireBulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.velocity = transform.localScale.x * Vector2.right * bulletSpeed;
        playAttack_Fire1.Play();
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(bullet);
    }

    void BreathFire()
    {
        if (currentFireBreath == null)
        {
            currentFireBreath = Instantiate(
                fireBreathPrefab,
                firePoint2.position,
                firePoint2.rotation
            );
            currentFireBreath.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
            StartCoroutine(DestroyFireBreathAfterTime(currentFireBreath, 1.5f));

            playAttack_Fire2.Play();

        }
    }

    private IEnumerator DestroyFireBreathAfterTime(GameObject fireBreath, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(fireBreath);
        currentFireBreath = null; // Reset biến lửa
    }

    void FireHand()
    {
        GameObject fireHand = Instantiate(fireHandPrefab, firePoint3.position, firePoint3.rotation);
        fireHand.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        Rigidbody2D rbFireHand = fireHand.GetComponent<Rigidbody2D>();
        if (rbFireHand == null)
        {
            rbFireHand = fireHand.AddComponent<Rigidbody2D>();
        }
        playAttack_Fire3.Play();
        rbFireHand.gravityScale = 1f;
        Vector2 fireDirection = new Vector2(transform.localScale.x, -1);
        rbFireHand.velocity = fireDirection * (bulletSpeed * 0.5f);
        StartCoroutine(DestroyFireHandAfterTime(fireHand, 3f));
    }

    private IEnumerator DestroyFireHandAfterTime(GameObject fireHand, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(fireHand);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount; // Giảm máu của người chơi

        if (currentHealth <= 0)
        {
            Die(); // Xử lý chết nếu máu bằng 0
        }
    }

    void Die()
    {
        // Xử lý khi người chơi chết, có thể phát animation hoặc restart game
        Debug.Log("Player is dead");
        Destroy(gameObject); // Xóa game object của người chơi
    }
}
