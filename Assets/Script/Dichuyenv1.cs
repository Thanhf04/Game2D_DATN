using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dichuyennv1 : MonoBehaviour
{
    // Các biến điều khiển nhân vật
    public float speed = 5f;
    private Rigidbody2D rb;
    public float jump = 10f;
    private bool isGrounded;
    private bool isRunning;
    private bool isRoll;
    private bool isJump;
    private Animator anim;

    // Các biến liên quan đến lăn (roll)
    public float rollDistance = 3f;
    public float rollDuration = 0.5f;

    // Các biến liên quan đến tấn công
    public GameObject fireBulletPrefab;
    public GameObject fireBreathPrefab;
    public GameObject fireHandPrefab;
    public Transform firePoint;
    public Transform firePoint2;
    public Transform firePoint3;
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 2f;

    // Các biến âm thanh
    public AudioSource music;
    public AudioSource playWalk;
    public AudioSource playAttack;
    public AudioSource playAttack2;
    public AudioSource playAttack_Fire1;
    public AudioSource playAttack_Fire2;
    public AudioSource playAttack_Fire3;
    public AudioSource playJump;

    // Các biến máu và mana
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMana = 100;
    public int currentMana;
    public int damageAmount = 10;
    private GameObject currentFireBreath;

    // Các biến level và điểm nâng
    public int level = 1;
    public int upgradePoints = 5;

    // Các biến UI
    public GameObject statsPanel;
    public Button openPanelButton;
    public Button increaseHealthButton;
    public Button decreaseHealthButton;
    public Button increaseManaButton;
    public Button decreaseManaButton;
    public Text healthText;
    public Text manaText;
    public Text levelText;
    public Text pointsText;

    // Các biến cooldown và UI cho kỹ năng
    public float skill1Cooldown = 2f;
    public float skill2Cooldown = 3f;
    public float skill3Cooldown = 5f;

    private float skill1Timer;
    private float skill2Timer;
    private float skill3Timer;

    public Image skill1Image;
    public Image skill2Image;
    public Image skill3Image;

    public Text skill1CooldownText;
    public Text skill2CooldownText;
    public Text skill3CooldownText;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isRunning = false;
        isRoll = false;
        isJump = false;
        music.Play();
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Khởi tạo UI
        statsPanel.SetActive(false);
        openPanelButton.onClick.AddListener(ToggleStatsPanel);
        increaseHealthButton.onClick.AddListener(IncreaseHealth);
        decreaseHealthButton.onClick.AddListener(DecreaseHealth);
        increaseManaButton.onClick.AddListener(IncreaseMana);
        decreaseManaButton.onClick.AddListener(DecreaseMana);

        UpdateStatsText();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Điều khiển di chuyển
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

    // Đổi hướng nhân vật và bật âm thanh khi di chuyển
    if (moveInput != 0 && isGrounded)
    {
        transform.localScale = moveInput > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        if (!playWalk.isPlaying)
        {
            playWalk.Play();
        }
    }
        else if (playWalk.isPlaying)
    {
        playWalk.Stop();
    }

    // Nhảy
    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
    {
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        isGrounded = false;
        isJump = true;
        anim.SetBool("isJump", true);
        playJump.Play();
        playWalk.Stop();
    }

        // Tấn công
    if (Input.GetKeyDown(KeyCode.F) && !isRoll)
    {
        StartCoroutine(Roll());
    }

    // Kỹ năng tấn công
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (skill1Timer <= 0 && currentMana >= 20)
            {
                Skill1();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (skill2Timer <= 0 && currentMana >= 30)
            {
                Skill2();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (skill3Timer <= 0 && currentMana >= 30)
            {
                Skill3();
            }
        }

        // Cập nhật các timer hồi chiêu
        UpdateSkillCooldowns();

        // Cập nhật vị trí của lửa nếu đang phun lửa
        if (currentFireBreath != null)
        {
            currentFireBreath.transform.position = firePoint2.position;
            currentFireBreath.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        }

        CheckLevelUp();
    }

    void UpdateSkillCooldowns()
    {
        if (skill1Timer > 0)
        {
            skill1Timer -= Time.deltaTime;
            skill1CooldownText.text = Mathf.Ceil(skill1Timer).ToString(); // Hiển thị thời gian còn lại
            skill1Image.fillAmount = skill1Timer / skill1Cooldown; // Cập nhật hình ảnh skill1
        }
        else
        {
            skill1CooldownText.text = ""; // Ẩn văn bản khi không trong thời gian hồi
        }

        if (skill2Timer > 0)
        {
            skill2Timer -= Time.deltaTime;
            skill2CooldownText.text = Mathf.Ceil(skill2Timer).ToString();
            skill2Image.fillAmount = skill2Timer / skill2Cooldown;
        }
        else
        {
            skill2CooldownText.text = "";
        }

        if (skill3Timer > 0)
        {
            skill3Timer -= Time.deltaTime;
            skill3CooldownText.text = Mathf.Ceil(skill3Timer).ToString();
            skill3Image.fillAmount = skill3Timer / skill3Cooldown;
        }
        else
        {
            skill3CooldownText.text = "";
        }
    }

    void Skill1()
    {
        if (currentMana >= 20) // Kiểm tra nếu mana đủ
        {
            ShootFireBullet();
            currentMana -= 20; // Giảm mana khi sử dụng kỹ năng
            skill1Timer = skill1Cooldown; // Bắt đầu thời gian hồi chiêu
            UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void Skill2()
    {
        if (currentMana >= 30) // Kiểm tra nếu mana đủ
        {
            BreathFire();
            currentMana -= 30; // Giảm mana khi sử dụng kỹ năng
            skill2Timer = skill2Cooldown; // Bắt đầu thời gian hồi chiêu
            UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void Skill3()
    {
        if (currentMana >= 30) // Kiểm tra nếu mana đủ
        {
            FireHand();
            currentMana -= 30; // Giảm mana khi sử dụng kỹ năng
            skill3Timer = skill3Cooldown; // Bắt đầu thời gian hồi chiêu
            UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void CheckLevelUp()
    {
        if (upgradePoints >= level + 5)
        {
            level++;
            upgradePoints++;
            UpdateStatsText();
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
            float newPosX = Mathf.Lerp(originalPosition, targetPosition, (elapsedTime / rollDuration));
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

    // Các phương thức liên quan đến tấn công
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
            currentFireBreath = Instantiate(fireBreathPrefab, firePoint2.position, firePoint2.rotation);
            currentFireBreath.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
            StartCoroutine(DestroyFireBreathAfterTime(currentFireBreath, 3f));
            playAttack_Fire2.Play();
     
        }
}

    private IEnumerator DestroyFireBreathAfterTime(GameObject fireBreath, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(fireBreath);
        currentFireBreath = null;
    }

void FireHand()
{
    // Tạo đối tượng fireHand tại vị trí của firePoint3
    GameObject fireHand = Instantiate(fireHandPrefab, firePoint3.position, firePoint3.rotation);
    fireHand.transform.localScale = new Vector3(transform.localScale.x, 1, 1);

    // Thêm Rigidbody2D và Collider2D nếu chưa có
    Rigidbody2D rbFireHand = fireHand.GetComponent<Rigidbody2D>();
    if (rbFireHand == null)
    {
        rbFireHand = fireHand.AddComponent<Rigidbody2D>();
    }

    BoxCollider2D collider = fireHand.GetComponent<BoxCollider2D>();
    if (collider == null)
    {
        collider = fireHand.AddComponent<BoxCollider2D>();
    }

    playAttack_Fire3.Play();

    // Bắt đầu quá trình tự hủy sau thời gian
    StartCoroutine(DestroyFireHandAfterTime(fireHand, 1.5f));
}

private IEnumerator DestroyFireHandAfterTime(GameObject fireHand, float time)
{
    yield return new WaitForSeconds(time);
    Destroy(fireHand);
}

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead");
        Destroy(gameObject);
    }

    // Các phương thức UI tăng/giảm máu và mana
    void ToggleStatsPanel()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }
    

    void IncreaseHealth()
    {
        if (upgradePoints > 0)
        {
            maxHealth += 100;
            currentHealth += 100;
            upgradePoints--;
            UpdateStatsText();
        }
    }

    void DecreaseHealth()
    {
        if (currentHealth > 0 && upgradePoints < level + 5)
        {
            maxHealth -= 100;
            currentHealth -= 100;
            upgradePoints++;
            UpdateStatsText();
        }
    }

    void IncreaseMana()
    {
        if (upgradePoints > 0)
        {
            maxMana += 50;
            currentMana += 50;
            upgradePoints--;
            UpdateStatsText();
        }
    }

    void DecreaseMana()
    {
        if (currentMana > 0 && upgradePoints < level + 5)
        {
            maxMana -= 10;
            currentMana -= 10;
            upgradePoints++;
            UpdateStatsText();
        }
    }

    void UpdateStatsText()
    {
        healthText.text = ((maxHealth - 100) / 100).ToString();
        manaText.text = ((maxMana - 100) / 10).ToString();
        levelText.text = "Level: " + level;
        pointsText.text = "Points: " + upgradePoints;
    }
}
