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
        yield return new WaitForSeconds(respawnTime);

        // Kiểm tra nếu mảng spawnPoints không rỗng
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            yield break;
        }

        if (enemy != null)
        {
            // Chọn vị trí spawn ngẫu nhiên
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Debug.Log($"Random spawn index chosen: {spawnIndex}");

            Transform spawnPoint = spawnPoints[spawnIndex];

            // Cập nhật vị trí và góc quay của quái vật
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;

            // Kích hoạt lại quái vật
            enemy.gameObject.SetActive(true);

            // Gọi phương thức Respawn trong script Enemy
            enemy.Respawn();

            Debug.Log($"Enemy respawned at position: {spawnPoint.position}");
        }
        else
        {
            Debug.LogError("Enemy is null! Can't respawn.");
        }
    }
}
