using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab; // Prefab của mũi tên
    public Transform shootPoint; // Điểm xuất phát của mũi tên
    public float arrowSpeed = 10f; // Tốc độ của mũi tên

    // Hàm này sẽ được gọi từ animation
    public void SpawnArrow()
    {
        // Tạo mũi tên mới
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        
        // Lấy Rigidbody2D từ mũi tên và thiết lập tốc độ
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.velocity = shootPoint.right * arrowSpeed; // Thiết lập hướng mũi tên
        Debug.Log("ban ne");
    }
}
