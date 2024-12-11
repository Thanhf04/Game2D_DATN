using UnityEngine;

public class newTrap : MonoBehaviour
{
    public float tocDoXoay = 5f;
    public float tocDoDiChuyen = 5f;
    public Transform diemA;
    public Transform diemB;
    private Vector3 diemMucTieu;
    void Start()
    {
        diemMucTieu = diemA.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, diemMucTieu, tocDoDiChuyen * Time.deltaTime);
        if (Vector3.Distance(transform.position, diemMucTieu) < 0.1f)
        {
            if (transform.position == diemA.position)
            {
                diemMucTieu = diemB.position;
            }
            else
            {
                diemMucTieu = diemA.position;
            }
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, tocDoXoay);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            // Gây sát thương cho người chơi nếu vùng sát thương chạm
            Dichuyennv1 playerHealth = other.GetComponent<Dichuyennv1>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Gây 20 sát thương
            }
        }
    }
}