using System.Collections;
using Cinemachine;
using Fusion;
using Fusion.Addons;
using Fusion.Addons.Physics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    private NetworkRigidbody2D _rigidbody2D;
    private NetworkMecanimAnimator _networkAnimator;
    private PlayerRef controllingPlayer;
    private int jumpCount = 0;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isGrounded = false;

    [Networked]
    public int MaxHealth { get; set; } = 100;

    [Networked]
    public int Health { get; set; } = 100;

    [Networked]
    public int MaxMana { get; set; } = 100;

    [Networked]
    public int Mana { get; set; } = 100;

    [Networked]
    public int Damage { get; set; } = 10;

    public int UpgradePoints = 5;
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

    public float attackRange = 1f;
    private CinemachineVirtualCamera vCam;

    [SerializeField]
    public AudioSource music;
    public AudioSource playWalk;
    public AudioSource playAttack;
    public AudioSource playAttack2;
    public AudioSource playAttack_Fire1;
    public AudioSource playAttack_Fire2;
    public AudioSource playAttack_Fire3;
    public AudioSource playJump;
    private bool wasJumpPressed = false;
    private bool wasSkill1Pressed = false;
    private bool wasSkill2Pressed = false;
    private bool wasSkill3Pressed = false;

    public int level = 1;
    public float currentLevel;
    public int upgradePoints = 5;

    public GameObject fireBulletPrefab;
    public GameObject fireBreathPrefab;
    public GameObject fireHandPrefab;
    public Transform firePoint;
    public Transform firePoint2;
    public Transform firePoint3;
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 2f;

    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;
    public GameObject gameOverPanel;
    public Button tryAgainButton;
    public Button resetButton;
    public Button mainMenuButton;
    private GameObject currentFireBreath;
    public float expMax = 100;
    public float expCurrent = 0;
    public TextMeshProUGUI textExp;

    [SerializeField]
    public TextMeshProUGUI textLevel;

    private tangchiso tangchiso1;

    public GameObject ChisoPanel;
    public Button ChisoButton;
    public TMP_Text healthInfoText;
    public TMP_Text manaInfoText;
    public TMP_Text damageInfoText;
    public Button exitButton;

    void Start()
    {
        if (Object.HasInputAuthority)
        {
            vCam = FindObjectOfType<CinemachineVirtualCamera>();

            if (vCam != null)
            {
                vCam.Follow = transform;
                vCam.LookAt = transform;
            }
        }

        Playmusic();

        if (tangchiso1 == null)
        {
            tangchiso1 = GameObject.Find("HeThongTangDiem").GetComponent<tangchiso>(); // Gán đối tượng cần tham chiếu
        }

        tryAgainButton.onClick.AddListener(OnTryAgain);
        resetButton.onClick.AddListener(OnReset);
        mainMenuButton.onClick.AddListener(OnMainMenu);
        gameOverPanel.SetActive(false);

        SetSlider();
        Health = MaxHealth;
        Mana = MaxMana;
        expSlider.value = expCurrent;
        textExp.SetText(expCurrent + "%");
        currentLevel = level;
        tangchiso1.UpdateStatsText();
        tangchiso1.notificationText.text = "";


        ChisoPanel.SetActive(false);
        ChisoButton.onClick.AddListener(ToggleStatsDisplay);
        exitButton.onClick.AddListener(ClosePanel);
    }

    void Playmusic()
    {
        music.Play();
        playWalk.Stop();
        playAttack.Stop();
        playAttack2.Stop();
        playAttack_Fire1.Stop();
        playAttack_Fire2.Stop();
        playAttack_Fire3.Stop();
        playJump.Stop();
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<NetworkRigidbody2D>();
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    public void SetPlayerControl(PlayerRef player)
    {
        controllingPlayer = player;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector2 velocity = data.direction * moveSpeed;
            _rigidbody2D.Rigidbody.velocity = new Vector2(
                velocity.x,
                _rigidbody2D.Rigidbody.velocity.y
            );

            if (data.isJumping && !wasJumpPressed)
            {
                if (CanJump())
                {
                    Jump();
                }
            }
            wasJumpPressed = data.isJumping;
            HandleMovement(velocity.x, data.isAttacking);

            if (data.isSkill1 && !wasSkill1Pressed)
            {
                Skill1();
            }
            if (data.isSkill2 && !wasSkill2Pressed)
            {
                Skill2();
            }
            if (data.isSkill3 && !wasSkill3Pressed)
            {
                Skill3();
            }

            wasSkill1Pressed = data.isSkill1;
            wasSkill2Pressed = data.isSkill2;
            wasSkill3Pressed = data.isSkill3;
        }
    }

    void Update(){
        // Cập nhật vị trí của lửa nếu đang phun lửa
        if (currentFireBreath != null)
        {
            currentFireBreath.transform.position = firePoint2.position;
            currentFireBreath.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        }
    }

    private bool CanJump()
    {
        return isGrounded || jumpCount < 2;
    }

    private void Jump()
    {
        _rigidbody2D.Rigidbody.velocity = new Vector2(_rigidbody2D.Rigidbody.velocity.x, 0);
        _rigidbody2D.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        playJump.Play();

        jumpCount++;
        isGrounded = false;

        // Đồng bộ trạng thái "đang nhảy"
        _networkAnimator.Animator.SetBool("isJump", true);
    }

    private void HandleMovement(float velocityX, bool attacking)
    {
        if (velocityX != 0 && !playWalk.isPlaying)
        {
            // playWalk.Play();
        }
        else if (velocityX == 0 && playWalk.isPlaying)
        {
            playWalk.Stop();
        }
        _networkAnimator.Animator.SetBool("isRunning", velocityX != 0);

        if (attacking && !_networkAnimator.Animator.GetBool("isAttack"))
        {
            StartCoroutine(Attack());
        }

        FlipSprite(velocityX);
    }

    private IEnumerator Attack()
    {
        _networkAnimator.Animator.SetBool("isAttack", true);
        playAttack.Play();
        PerformAttack();
        yield return new WaitForSeconds(0.5f);
        _networkAnimator.Animator.SetBool("isAttack", false);
    }

    private void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            LayerMask.GetMask("Enemy")
        );
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(Damage);
        }
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;

        if (Health <= 0)
        {
            Die();
        }
        healthSlider.value = Health;
    }

    public void UseMana(int manaCost)
    {
        if (Mana >= manaCost)
        {
            Mana -= manaCost;
            // Perform ability if mana is sufficient
        }
        else
        {
            Debug.Log("Not enough mana");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            isGrounded = true;
            _networkAnimator.Animator.SetBool("isJump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NenDat"))
        {
            jumpCount = 0;
            isGrounded = false;
            _networkAnimator.Animator.SetBool("isJump", true);
        }
    }

    private void FlipSprite(float velocityX)
    {
        if (velocityX > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (velocityX < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
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
        if (Mana >= 20) // Kiểm tra nếu mana đủ
        {
            ShootFireBullet();
            skill1Timer = skill1Cooldown; // Bắt đầu thời gian hồi chiêu
            tangchiso1.UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void Skill2()
    {
        if (Mana >= 30) // Kiểm tra nếu mana đủ
        {
            BreathFire();
            skill2Timer = skill2Cooldown; // Bắt đầu thời gian hồi chiêu
            tangchiso1.UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void Skill3()
    {
        if (Mana >= 30) // Kiểm tra nếu mana đủ
        {
            FireHand();
            skill3Timer = skill3Cooldown; // Bắt đầu thời gian hồi chiêu
            tangchiso1.UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void ShootFireBullet()
    {
        if (Mana >= 20) // Kiểm tra nếu mana đủ
        {
            GameObject bullet = Instantiate(
                fireBulletPrefab,
                firePoint.position,
                firePoint.rotation
            );
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.velocity = transform.localScale.x * Vector2.right * bulletSpeed;
            playAttack_Fire1.Play();
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));
            //Mana -= 20; // Giảm mana khi sử dụng kỹ năng
            manaSlider.value = Mana -= 20;
            tangchiso1.UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(bullet);
    }

void BreathFire()
    {
        if (Mana >= 30) // Kiểm tra nếu mana đủ
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
                // currentMana -= 30; // Giảm mana khi sử dụng kỹ năng
                manaSlider.value = Mana -= 30;
                tangchiso1.UpdateStatsText(); // Cập nhật giao diện người dùng
            }
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
        if (Mana >= 10) // Kiểm tra nếu mana đủ
        {
            GameObject fireHand = Instantiate(
                fireHandPrefab,
                firePoint3.position,
                firePoint3.rotation
            );
            fireHand.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
            Rigidbody2D rbFireHand = fireHand.GetComponent<Rigidbody2D>();
            if (rbFireHand == null)
            {
                rbFireHand = fireHand.AddComponent<Rigidbody2D>();
            }
            playAttack_Fire3.Play();
            rbFireHand.gravityScale = 1f;
            // Vector2 fireDirection = new Vector2(transform.localScale.x, -1);
            // rbFireHand.velocity = fireDirection * (bulletSpeed * 0.5f);
            StartCoroutine(DestroyFireHandAfterTime(fireHand, 2.5f));
            Mana -= 10; // Giảm mana khi sử dụng kỹ năng
            manaSlider.value = Mana;
            tangchiso1.UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    private IEnumerator DestroyFireHandAfterTime(GameObject fireHand, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(fireHand);
    }

    void Die()
    {
        Debug.Log("Player is dead");
        ShowGameOverPanel();
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Tạm dừng game
    }

    void OnTryAgain()
    {
        // Tải lại cảnh hiện tại để chơi lại
        Time.timeScale = 1f; // Tiếp tục game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnReset()
    {
        Time.timeScale = 1f; // Tiếp tục game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnMainMenu()
    {
        // Quay lại menu chính
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Thay "MainMenu" bằng tên cảnh menu chính của bạn
    }

    public void SetSlider()
    {
        healthSlider.maxValue = MaxHealth;
        manaSlider.maxValue = MaxMana;

        expSlider.maxValue = expMax;
    }

    public void LevelSlider(float amount)
    {
        expCurrent += amount;
        textExp.SetText(expCurrent + "%");
        if (expCurrent >= expMax)
        {
            expCurrent = 0;
            textExp.SetText(expCurrent + "%");
            level++;
            textLevel.SetText("Lv" + level);
            upgradePoints++;
            tangchiso1.UpdateStatsText();
        }
        expSlider.value = expCurrent;
    }

    void ToggleStatsDisplay()
    {
        // Hiển thị hoặc ẩn bảng Chỉ Số
        bool isActive = ChisoPanel.activeSelf;
        ChisoPanel.SetActive(!isActive);

        // Cập nhật thông tin nếu bảng hiển thị
        if (!isActive)
        {
            UpdateStatsDisplay();
        }
    }

    void UpdateStatsDisplay()
    {
        // Cập nhật các dòng chữ trong bảng "Chỉ Số"
        healthInfoText.text = $"Máu:  {Health}/{MaxHealth}";
        manaInfoText.text = $"Năng lượng:  {Mana}/{MaxMana}";
        damageInfoText.text = $"Sát thương:  {Damage}";
    }

    void ClosePanel()
    {
        ChisoPanel.SetActive(false);
    }
}
