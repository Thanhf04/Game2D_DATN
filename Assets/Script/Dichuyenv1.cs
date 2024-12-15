using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dichuyennv1 : MonoBehaviour
{
    #region Khai báo các biến

    public Dichuyennv1 playerStats;
    private FirebaseManager1 firebaseManager1;
    private string username = ""; // Đây là ID người chơi, bạn có thể lấy từ hệ thống đăng nhập
    //nhiemvu
    public PlayerStatsManager playerStatsManager1;
    private NPCQuest npcQuest;
    public bool isQuest1Complete = false;
    public bool isAppleQuestComplete = false;
    private NPCAppleArmorQuest npcapple;
    public bool isPlayerNearby = false;
    private GameObject currentChest;
    public GameObject quizGamePanel;
    public GameObject tbaoQuizGamePanel; // Panel thông báo (Tbaoquizz game)


    [SerializeField]
    private InventoryManager inventoryManager; // Tham chiếu đến InventoryManager

    [SerializeField]
    private ItemClass appleItem;

    [SerializeField]
    private ItemClass armorItem; // ItemClass đại diện cho Apple

    // Các biến điều khiển nhân vật
    public float speed = 5f;
    private Rigidbody2D rb;
    public float jump = 10f;
    private int jumpCount = 0; // Đếm số lần nhảy
    private bool isGrounded;
    private bool isRunning;
    private bool isRoll;
    private bool isJump;
    public static bool isStatsPanelOpen = false;
    public static bool isStatsDisplayOpen = false;
    private Animator anim;
    public TextMeshProUGUI notificationText;

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
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI damaText;
    public Text levelText;
    public Text pointsText;

    // Các biến cooldown và UI cho kỹ năng
    public float skill1Cooldown = 2f;
    public float skill2Cooldown = 3f;
    public float skill3Cooldown = 5f;

    public float skill1Timer;
    public float skill2Timer;
    public float skill3Timer;

    public Image skill1Image;
    public Image skill2Image;
    public Image skill3Image;

    public Text skill1CooldownText;
    public Text skill2CooldownText;
    public Text skill3CooldownText;


    #endregion
    void Start()
    {
        firebaseManager1 = FindObjectOfType<FirebaseManager1>();
        playerStatsManager1 = FindObjectOfType<PlayerStatsManager>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isRunning = false;
        isRoll = false;
        isJump = false;

        #region đóng tương tác với slider
        healthSlider.interactable = false;
        manaSlider.interactable = false;
        expSlider.interactable = false;
        #endregion
        StartSound();

        // Khởi tạo UI
        statsPanel.SetActive(false);

        // NPC
        npcQuest = FindObjectOfType<NPCQuest>();
        npcapple = FindObjectOfType<NPCAppleArmorQuest>();
        isQuest1Complete = false;

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

        gameOverPanel.SetActive(false);

        // Gán các sự kiện cho các nút
        tryAgainButton.onClick.AddListener(OnTryAgain);
        resetButton.onClick.AddListener(OnReset);
        mainMenuButton.onClick.AddListener(OnMainMenu);

        ChisoPanel.SetActive(false);
        exitButton.onClick.AddListener(ClosePanel);

        // Tải dữ liệu người chơi từ Firebase ngay khi game bắt đầu
        //LoadPlayerDataFromFirebase();
    }

    //private void LoadPlayerDataFromFirebase()
    //{
    //    // Gọi hàm LoadPlayerData từ FirebaseManager chỉ một lần trong Start
    //    Debug.Log("Bắt đầu tải dữ liệu người chơi từ Firebase...");
    //    firebaseManager1.LoadPlayerData(OnPlayerDataLoaded);
    //}
    private bool isDataLoaded = false;
    private void OnPlayerDataLoaded(FirebaseManager1.PlayerData playerData)
    {
        if (playerData != null)
        {
            // Cập nhật thông tin người chơi vào script Dichuyennv1
            firebaseManager1.UpdatePlayerStats(playerData, this);

            // Kiểm tra lại trạng thái nhiệm vụ
            Debug.Log("Apple Armor Quest Completed: " + NPCAppleArmorQuest.isCompletedAppleQuest);
        }
        else
        {
            Debug.Log("Player data not found or loading failed.");
        }
    }



    void Update()
    {

        if (!isDataLoaded)
        {
            // Nếu dữ liệu chưa được tải, gọi hàm LoadPlayerData từ FirebaseManager1
            Debug.Log("Bắt đầu tải dữ liệu người chơi từ Firebase...");
            firebaseManager1.LoadPlayerData(OnPlayerDataLoaded);
            isDataLoaded = true;  // Đánh dấu rằng dữ liệu đã được tải
        }

        if (quizGamePanel.activeSelf || tbaoQuizGamePanel.activeSelf)
        {
            // Dừng animation và âm thanh
            anim.SetBool("isRunning", false);
            playWalk.Stop();
            playJump.Stop();

            // Ngăn player di chuyển hoặc thực hiện các hành động khác
            rb.velocity = Vector2.zero; // Giữ nhân vật đứng yên
            return;
        }


        //if (Quest_3.isWolfQuest)
        //{
        //    isJump = false;
        //    anim.SetBool("isJump", false);
        //    Debug.Log("Khóa");
        //    return;

        //}



        // Dừng di chuyển nếu đang mở cửa hàng hoặc panel stats
        if (ShopOpen.isOpenShop || isStatsPanelOpen || NPC_Controller.isDialogue || GameManager.isMiniGame || OpenSettings.isSettings
            || OpenChiSoCaNhan.ischisoCaNhan || isStatsDisplayOpen || Quest_3.isQuest3 || OpenMiniGame_Input.isMiniGameInput
            || OpenMiniGame_Input.isDialogue_MiniGameInput || Inventory.isInventoryOpen || NPCQuest.isQuest
            || NPCAppleArmorQuest.isQuestAppleArmor || NPCQuestSkill2.isNPCQuestSkill2)
        {
            isRunning = false;
            anim.SetBool("isRunning", false);
            playWalk.Stop();
            playJump.Stop();
            return;
        }
        // Điều khiển di chuyển và trạng thái di chuyển (kể cả khi đang nhảy)
        float moveInput = Input.GetAxis("Horizontal");
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

        if (Input.GetKeyDown(KeyCode.F) && currentChest != null) // Phím F
        {
            OpenChest(currentChest); // Gọi hàm mở rương
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
        if (Input.GetMouseButtonDown(0) && !isRoll && !IsPointerOverUI() && isQuest1Complete) // Phải hoàn thành nhiệm vụ 1 mới tấn công
        {
            StartCoroutine(Attack());
            string userName = PlayerPrefs.GetString("username", "");
            firebaseManager1.SavePlayerData(this);

        }
        if (Input.GetKeyDown(KeyCode.Q) && NPCAppleArmorQuest.isCompletedAppleQuest == true)
        {
            firebaseManager1.SavePlayerData(this);
            if (skill1Timer <= 0 && currentMana >= 20)
            {
                Skill1();
                string userName = PlayerPrefs.GetString("username", "");
              

            }
          
        }
        if (Input.GetKeyDown(KeyCode.E) && NPCQuestSkill2.hasCompletedQuest == true)
        {
            if (skill2Timer <= 0 && currentMana >= 30)
            {

                Skill2();
                firebaseManager1.SavePlayerData(this);

            }
        }
        if (Input.GetKeyDown(KeyCode.R) && Quest_3.hasCompletedQuestInput == true)
        {
            if (skill3Timer <= 0 && currentMana >= 30)
            {

                Skill3();
                firebaseManager1.SavePlayerData(this);
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
            //firebaseManager1.SavePlayerData(this);
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
            //firebaseManager1.SavePlayerData(this);
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
            firebaseManager1.SavePlayerData(this);

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
            currentMana -= 20; // Giảm mana khi sử dụng kỹ năng
            manaSlider.value = currentMana;
            UpdateStatsText(); // Cập nhật giao diện người dùng
            firebaseManager1.SavePlayerData(this);
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
                currentMana -= 30; // Giảm mana khi sử dụng kỹ năng
                manaSlider.value = currentMana;
                UpdateStatsText(); // Cập nhật giao diện người dùng
                firebaseManager1.SavePlayerData(this);
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
            firebaseManager1.SavePlayerData(this);
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
        //firebaseManager1.SavePlayerData(username, this);
        if (currentHealth <= 0)
        {
            Die();
            // Save data after decreasing mana
        }
        firebaseManager1.SavePlayerData(this);
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
    public void ToggleStatsPanel()
    {
        //statsPanel.SetActive(!statsPanel.activeSelf);
        //isStatsPanelOpen = statsPanel.activeSelf;
        PanelManager.Instance.OpenPanel(statsPanel);
        isStatsPanelOpen = true;
    }

    public void ToggleCloseStatsPanel()
    {
        PanelManager.Instance.ClosePanel(statsPanel);
        isStatsPanelOpen = false;
    }

    void IncreaseHealth()
    {
        if (upgradePoints > 0)
        {
            maxHealth += 100;
            currentHealth = Mathf.Min(maxHealth, currentHealth + 100);
            healthSlider.maxValue = maxHealth;
            //healthSlider.value = currentHealth;
            upgradePoints--;
            UpdateStatsText();
            firebaseManager1.SavePlayerData(this);
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
            firebaseManager1.SavePlayerData(this);
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
            firebaseManager1.SavePlayerData(this);
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
            firebaseManager1.SavePlayerData(this);
        }
    }

    void IncreaseDame()
    {
        if (upgradePoints > 0)
        {
            damageAmount += 10;
            upgradePoints--;
            UpdateStatsText();
            firebaseManager1.SavePlayerData(this);
        }
        else
        {
            ShowNotification("Bạn đã hết điểm nâng cấp!");
        }
    }

    void DecreaseDamage()
    {
        if (damageAmount > 1 && upgradePoints < level + 5)
        {
            damageAmount = Mathf.Max(10, damageAmount - 10); // Ensure damage doesn't go below 10
            upgradePoints++;
            UpdateStatsText();
            firebaseManager1.SavePlayerData(this);
        }
    }


    public void UpdateStatsText()
    {
        healthText.text = ((maxHealth - 100) / 100).ToString();
        manaText.text = ((maxMana - 100) / 100).ToString();
        damaText.text = ((damageAmount - 10) / 10).ToString();
        levelText.text = "Level: " + level;
        pointsText.text = "Points: " + upgradePoints;
        firebaseManager1.SavePlayerData(this);
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
        //firebaseManager1.SavePlayerData(playerId, this);
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

    public void ToggleStatsDisplay()
    {
        // Hiển thị hoặc ẩn bảng Chỉ Số
        bool isActive = ChisoPanel.activeSelf;
        isStatsDisplayOpen = true;
        PanelManager.Instance.OpenPanel(ChisoPanel);
        {
            // Cập nhật thông tin nếu bảng hiển thị
            if (!isActive)
            {
                UpdateStatsDisplay();
            }
        }
    }

    public void ToggleCloseStatsDisplay()
    {
        PanelManager.Instance.ClosePanel(ChisoPanel);
        isStatsDisplayOpen = false;
    }

    void UpdateStatsDisplay()
    {
        // Cập nhật các dòng chữ trong bảng "Chỉ Số"
        healthInfoText.text = $"Máu:  {currentHealth}/{maxHealth}";
        manaInfoText.text = $"Năng lượng:  {currentMana}/{maxMana}";
        damageInfoText.text = $"Sát thương:  {damageAmount}";
    }

    public void ClosePanel()
    {
        PanelManager.Instance.ClosePanel(ChisoPanel);
    }

    public void UpdateQuest()
    {
        Debug.Log("Cập nhật nhiệm vụ cho NPCQuest");
        // Chỉ cập nhật nếu nhiệm vụ chưa hoàn thành
        if (npcQuest != null && !isQuest1Complete)
        {
            npcQuest.FindSword();
            isQuest1Complete = true;
            firebaseManager1.SavePlayerData(this);
            //Debug.Log(this);
            ////SaveQuestStatus();

        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra khi chạm vào thanh kiếm
        if (other.CompareTag("kiem") && !isQuest1Complete)
        {
            Debug.Log("anh nhat dep trai qua");
            UpdateQuest();
            Destroy(other.gameObject);
        }

        // Kiểm tra khi chạm vào rương
        if (other.CompareTag("Chest"))
        {
            currentChest = other.gameObject;
            Debug.Log("Đã vào vùng tương tác với rương!");
            isPlayerNearby = true;
        }

        // Kiểm tra khi chạm vào táo
        if (other.CompareTag("Apple"))
        {
            CollectApple(other.gameObject);
        }

        // Kiểm tra khi chạm vào giáp
        if (other.CompareTag("Armor"))
        {
            CollectArmor(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player rời vùng tương tác với rương.");
        }

        if (other.CompareTag("Chest"))
        {
            currentChest = null; // Xóa tham chiếu rương
            Debug.Log("Rời vùng tương tác với rương!");
        }
    }

    private void OpenChest(GameObject chest)
    {
        Debug.Log("Rương được mở!");
        chest.SetActive(false); // Ẩn rương khi mở

        if (chest.GetComponent<ChestInteraction>() != null)
        {
            chest.GetComponent<ChestInteraction>().OpenChest();
        }
    }

    private void CollectApple(GameObject apple)
    {
        AddAppleToInventory(); // Thêm táo vào Inventory
        UpdateApple(); // Cập nhật nhiệm vụ (nếu cần)
        Destroy(apple); // Hủy object táo trong game
    }

    private void CollectArmor(GameObject armor)
    {
        AddArmorToInventory(); // Thêm giáp vào Inventory
        UpdateArmor(); // Cập nhật nhiệm vụ giáp (nếu cần)
        Destroy(armor); // Hủy object giáp trong game
    }

    public void UpdateApple()
    {
        Debug.Log("Cập nhật nhiệm vụ cho NPCApple");
        // Chỉ cập nhật nếu nhiệm vụ táo chưa hoàn thành
        if (npcapple != null)
        {
            isAppleQuestComplete = true;
            npcapple.CollectApple();
            firebaseManager1.SavePlayerData(this);
        }
    }
    public void UpdateArmor()
    {
        Debug.Log("Cập nhật nhiệm vụ cho NPCApple");
        // Chỉ cập nhật nếu nhiệm vụ giáp chưa hoàn thành
        if (npcapple != null)
        {
            npcapple.CollectArmor(); // Gọi phương thức để cập nhật nhiệm vụ giáp
        }
    }

    private void AddAppleToInventory()
    {
        if (inventoryManager != null) // Kiểm tra InventoryManager đã được tham chiếu
        {
            inventoryManager.AddItem(appleItem, 1); // Thêm 1 quả táo vào Inventory
            Debug.Log("Táo đã được thêm vào Inventory!");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy InventoryManager!");
        }
    }

    private void AddArmorToInventory()
    {
        if (inventoryManager != null) // Kiểm tra InventoryManager đã được tham chiếu
        {
            inventoryManager.AddItem(armorItem, 1); // Thêm 1 bộ giáp vào Inventory
            Debug.Log("Giáp đã được thêm vào Inventory!");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy InventoryManager!");
        }
    }
    // Mở panel quiz game
    // Mở panel quiz game
    public void OpenQuizGamePanel()
    {
        quizGamePanel.SetActive(true); // Hiển thị panel
    }

    // Đóng panel quiz game
    public void CloseQuizGamePanel()
    {
        quizGamePanel.SetActive(false); // Ẩn panel
    }

    // Mở panel thông báo
    public void OpenTbaoQuizGamePanel()
    {
        tbaoQuizGamePanel.SetActive(true); // Hiển thị panel
    }

    // Đóng panel thông báo và mở panel quiz game
    public void CloseTbaoAndOpenQuizGamePanel()
    {
        tbaoQuizGamePanel.SetActive(false); // Ẩn panel thông báo
        OpenQuizGamePanel(); // Mở panel quiz game
    }

    // Phương thức cập nhật UI sau khi dữ liệu người chơi được tải
    //private void UpdateUI(FirebaseManager1.PlayerData playerData)
    //{
    //    if (playerData != null)
    //    {
    //        // Cập nhật các trường UI với dữ liệu người chơi
    //        healthText.text = "Health: " + playerData.currentHealth;
    //        manaText.text = "Mana: " + playerData.currentMana;
    //        levelText.text = "Level: " + playerData.level;
    //        expText.text = "EXP: " + playerData.expCurrent + "/" + playerData.expMax;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Player data is null, UI will not be updated.");
    //    }
    //}

}


//    currentHealth expCurrent isQuest1Complete isAppleQuestComplete
