using UnityEngine;

public class newTrap : MonoBehaviour
{
    public float tocDoXoay = 5f;
    public float tocDoDiChuyen = 5f;
    public Transform diemA;
    public Transform diemB;
    private Vector3 diemMucTieu;
    // Tham chiếu tới Prefab hiệu ứng
    public GameObject vaChamEffectPrefab; // Prefab hiệu ứng khi va chạm
    public Transform effectSpawnPoint; // Điểm sinh hiệu ứng (nếu cần)

    void Start()
    {
        diemMucTieu = diemA.position;
    }

    void Update()
    {

        // Di chuyển trap giữa điểm A và điểm B
        transform.position = Vector3.MoveTowards(transform.position, diemMucTieu, tocDoDiChuyen * Time.deltaTime);
        if (Vector3.Distance(transform.position, diemMucTieu) < 0.1f)
        {
            diemMucTieu = (transform.position == diemA.position) ? diemB.position : diemA.position;
        }
    }

    private void FixedUpdate()
    {
        // Xoay trap
        transform.Rotate(0, 0, tocDoXoay);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Gây sát thương cho người chơi
            Dichuyennv1 playerHealth = collision.gameObject.GetComponent<Dichuyennv1>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(5); // Gây 10 sát thương
            }

            // Hiển thị hiệu ứng va chạm
            ShowEffect();
        }
    }
    private void ShowEffect()
    {
        if (vaChamEffectPrefab != null)
        {
            // Determine spawn position
            Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;

            // Create the effect instance
            GameObject effectInstance = Instantiate(vaChamEffectPrefab, spawnPosition, Quaternion.identity);

            // Schedule destruction of the effect instance after 2 seconds
            Destroy(effectInstance, 2f);
        }
    }
}
