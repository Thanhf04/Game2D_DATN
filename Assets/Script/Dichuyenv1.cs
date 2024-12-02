using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Dichuyennv1 : MonoBehaviour
{
    // Các biến điều khiển nhân vật
    public float speed = 5f;
    private Rigidbody2D rb;
    public float jump = 10f;
    private int jumpCount = 0; // Đếm số lần nhảy
    private bool isGrounded;
    private bool isRunning;
    private bool isRoll;
    private bool isJump;
    private bool isStatsPanelOpen = false;
    private Animator anim;
    public Text notificationText;

    //panel die
    public GameObject gameOverPanel;
    public Button tryAgainButton;
    public Button resetButton;
    public Button mainMenuButton;

    //Panel chỉ số player
    public GameObject ChisoPanel;
    public Button ChisoButton;
    public TMP_Text healthInfoText;
    public TMP_Text manaInfoText;
    public TMP_Text damageInfoText;
    public Button exitButton;

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
    [SerializeField]
    public AudioSource music;
    public AudioSource playWalk;
    public AudioSource playAttack;
    public AudioSource playAttack2;
    public AudioSource playAttack_Fire1;
    public AudioSource playAttack_Fire2;
    public AudioSource playAttack_Fire3;
    public AudioSource playJump;

    // Các biến máu và mana
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMana = 100;
    public int currentMana;
    public float expMax = 100;
    public float expCurrent = 0;

    [SerializeField]
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textExp;
    public int damageAmount = 10;
    public int damageTrap = 20;
    private GameObject currentFireBreath;

    // Các biến level và điểm nâng
    public int level = 1;
    public float currentLevel;
    public int upgradePoints = 5;

    // Các biến UI
    [SerializeField]
    public GameObject statsPanel;
    public Button openPanelButton;
    public Button increaseHealthButton;
    public Button decreaseHealthButton;
    public Button increaseManaButton;
    public Button decreaseManaButton;
    public Button increaseDamethButton;
    public Button decreaseDamethButton;
    public Text healthText;
    public Text manaText;
    public Text damaText;
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

        StartSound();
        // Khởi tạo UI
        statsPanel.SetActive(false);
        openPanelButton.onClick.AddListener(ToggleStatsPanel);
        increaseHealthButton.onClick.AddListener(IncreaseHealth);
        decreaseHealthButton.onClick.AddListener(DecreaseHealth);
        increaseManaButton.onClick.AddListener(IncreaseMana);
        decreaseManaButton.onClick.AddListener(DecreaseMana);
        increaseDamethButton.onClick.AddListener(IncreaseDame);
        decreaseDamethButton.onClick.AddListener(DecreaseDamage);

        SetSlider();
        currentHealth = maxHealth;
        currentMana = maxMana;
        expSlider.value = expCurrent;
        textExp.SetText(expCurrent + "%");
        currentLevel = level;
        UpdateStatsText();

        // notificationText.text = "";

        gameOverPanel.SetActive(false);

        // Gán các sự kiện cho các nút
        tryAgainButton.onClick.AddListener(OnTryAgain);
        resetButton.onClick.AddListener(OnReset);
        mainMenuButton.onClick.AddListener(OnMainMenu);

        ChisoPanel.SetActive(false);
        ChisoButton.onClick.AddListener(ToggleStatsDisplay);
        exitButton.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Dừng di chuyển nếu đang mở cửa hàng hoặc panel stats
        if (NPC.isOpenShop || isStatsPanelOpen)
        {
            isRunning = false;
            anim.SetBool("isRunning", false);
            playWalk.Stop();
            return;
        }
        // Điều khiển di chuyển và trạng thái di chuyển (kể cả khi đang nhảy)
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        isRunning = moveInput != 0;
        anim.SetBool("isRunning", isRunning);

        if (moveInput != 0)
        {
            transform.localScale = moveInput > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            if (!playWalk.isPlaying && isGrounded)
            {
                playWalk.Play();
            }
            if (playAttack.isPlaying)
            {
                playAttack.Stop();
            }
        }
        else if (playWalk.isPlaying)
        {
            playWalk.Stop();
        }
        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < 2)) // Kiểm tra nếu nhân vật đang trên mặt đất hoặc đã nhảy ít hơn 2 lần
        {
            rb.velocity = new Vector2(rb.velocity.x, jump);
            isGrounded = false;
            isJump = true;
            anim.SetBool("isJump", true);
            playJump.Play();
            playWalk.Stop();
            jumpCount++; // Tăng số lần nhảy mỗi khi nhấn phím nhảy
        }

        // Tấn công

        if (Input.GetMouseButtonDown(0) && !isRoll && !IsPointerOverUI()) // Kiểm tra nếu nhấn chuột trái và không trong quá trình lăn
        {
            StartCoroutine(Attack());
        }
        //lan
        // if (Input.GetKeyDown(KeyCode.F) && !isRoll)
        // {
        //     StartCoroutine(Roll());
        // }
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
            UpdateStatsText();
        }
        expSlider.value = expCurrent;
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
            skill1Timer = skill1Cooldown; // Bắt đầu thời gian hồi chiêu
            UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void Skill2()
    {
        if (currentMana >= 30) // Kiểm tra nếu mana đủ
        {
            BreathFire();
            skill2Timer = skill2Cooldown; // Bắt đầu thời gian hồi chiêu
            UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    void Skill3()
    {
        if (currentMana >= 30) // Kiểm tra nếu mana đủ
        {
            FireHand();
            skill3Timer = skill3Cooldown; // Bắt đầu thời gian hồi chiêu
            UpdateStatsText(); // Cập nhật giao diện người dùng
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
            jumpCount = 0; // Đặt lại số lần nhảy khi nhân vật chạm đất
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
        if (currentMana >= 20) // Kiểm tra nếu mana đủ
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
            //currentMana -= 20; // Giảm mana khi sử dụng kỹ năng
            manaSlider.value = currentMana -= 20;
            UpdateStatsText(); // Cập nhật giao diện người dùng
        }
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(bullet);
    }

    void BreathFire()
    {
        if (currentMana >= 30) // Kiểm tra nếu mana đủ
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
                manaSlider.value = currentMana -= 30;
                UpdateStatsText(); // Cập nhật giao diện người dùng
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
        if (currentMana >= 10) // Kiểm tra nếu mana đủ
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
            currentMana -= 10; // Giảm mana khi sử dụng kỹ năng
            manaSlider.value = currentMana;
            UpdateStatsText(); // Cập nhật giao diện người dùng
        }
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
        healthSlider.value = currentHealth;
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
        SceneManager.LoadScene("SampleScene"); // Thay "MainMenu" bằng tên cảnh menu chính của bạn
    }

    // kiểm tra âm thanh
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Các phương thức UI tăng/giảm máu và mana
    void ToggleStatsPanel()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        isStatsPanelOpen = statsPanel.activeSelf;
    }

    void IncreaseHealth()
    {
        if (upgradePoints > 0)
        {
            maxHealth += 100;
            currentHealth = Mathf.Min(maxHealth, currentHealth + 100);
            healthSlider.maxValue = maxHealth;
            upgradePoints--;
            UpdateStatsText();
        }
        else
        {
            ShowNotification("Bạn đã hết điểm nâng cấp!");
        }
    }

    void DecreaseHealth()
    {
        if (currentHealth > 1 && upgradePoints < level + 5)
        {
            maxHealth = Mathf.Max(100, maxHealth - 100);
            healthSlider.maxValue = maxHealth;
            currentHealth = Mathf.Clamp(currentHealth - 100, 1, maxHealth);
            upgradePoints++;
            UpdateStatsText();
        }
    }

    void IncreaseMana()
    {
        if (upgradePoints > 0)
        {
            maxMana += 100;
            currentMana = Mathf.Min(maxMana, currentMana + 50);
            manaSlider.maxValue = maxMana;
            upgradePoints--;
            UpdateStatsText();
        }
        else
        {
            ShowNotification("Bạn đã hết điểm nâng cấp!");
        }
    }

    void DecreaseMana()
    {
        if (currentMana > 1 && upgradePoints < level + 5)
        {
            maxMana = Mathf.Max(100, maxMana - 100);
            manaSlider.maxValue = maxMana;
            currentMana = Mathf.Clamp(currentMana - 100, 1, maxMana);
            upgradePoints++;
            UpdateStatsText();
        }
    }

    void IncreaseDame()
    {
        if (upgradePoints > 0)
        {
            damageAmount += 10;
            upgradePoints--;
            UpdateStatsText();
        }
        else
        {
            ShowNotification("Bạn đã hết điểm nâng cấp!");
        }
    }

    void DecreaseDamage()
    {
        if (damageAmount > 0 && upgradePoints < level + 5)
        {
            damageAmount -= 10;
            upgradePoints++;
            UpdateStatsText();
        }
    }

    void UpdateStatsText()
    {
        healthText.text = ((maxHealth - 100) / 100).ToString();
        manaText.text = ((maxMana - 100) / 100).ToString();
        damaText.text = ((damageAmount - 10) / 10).ToString();
        levelText.text = "Level: " + level;
        pointsText.text = "Points: " + upgradePoints;
    }

    void ShowNotification(string message)
    {
        notificationText.text = message; // Hiển thị thông báo
        Invoke("ClearNotification", 2f); // Xóa thông báo sau 2 giây
    }

    void ClearNotification()
    {
        notificationText.text = ""; // Xóa thông báo
    }

    public void SetSlider()
    {
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;
        expSlider.maxValue = expMax;
    }

    public void TakeDamageTrap(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player mất máu! Máu còn lại: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void StartSound()
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
        healthInfoText.text = $"Máu:  {currentHealth}/{maxHealth}";
        manaInfoText.text = $"Năng lượng:  {currentMana}/{maxMana}";
        damageInfoText.text = $"Sát thương:  {damageAmount}";
    }
    void ClosePanel()
    {
        ChisoPanel.SetActive(false);

    }
}
