using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public float respawnTime = 5f; // Thời gian chờ để respawn
    public Transform[] spawnPoints; // Vị trí quái vật trên bản đồ
    private Queue<GameObject> enemyPool = new Queue<GameObject>(); // Pool quái vật

    // Hàm gọi khi quái vật chết
    public void OnDeath(Enemy enemy)
    {
        enemy.gameObject.SetActive(false); // Ẩn quái vật khi chết
        StartCoroutine(Respawn(enemy)); // Chờ một thời gian rồi respawn lại
    }

    // Thực hiện respawn quái vật
    private IEnumerator Respawn(Enemy enemy)
    {
        // Chờ một khoảng thời gian trước khi respawn
        yield return new WaitForSeconds(respawnTime);

        // Kiểm tra xem spawnPoints có rỗng không
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            yield break; // Dừng coroutine nếu không có điểm spawn
        }

        // Kiểm tra xem enemy có phải là null không
        if (enemy != null)
        {
            // Đặt lại vị trí của quái vật ở vị trí spawn ngẫu nhiên từ danh sách spawnPoints
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Đặt lại vị trí và kích hoạt quái vật trở lại
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
            enemy.gameObject.SetActive(true); // Hiển thị quái vật trở lại

            // Gọi phương thức Respawn trong Enemy
            enemy.Respawn();
        }
        else
        {
            Debug.LogError("Enemy is null! Can't respawn.");
        }
    }
}
