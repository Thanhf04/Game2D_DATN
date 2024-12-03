using System.Collections;
using UnityEngine;

public class EnemyRespawnParent1 : MonoBehaviour
{
    public GameObject enemy; // Tham chiếu đến quái vật con
    public float respawnTime = 20f; // Thời gian hồi sinh

    private Vector3 initialPosition;
    public bool isRespawning = false; // Cờ kiểm tra xem có đang hồi sinh hay không
    EnemyRespawn enemyRespawn;

    private void Start()
    {
        if (enemy != null)
        {
            initialPosition = enemy.transform.position;
        }
        enemyRespawn = enemy.GetComponent<EnemyRespawn>();
    }

    public void RespawnEnemy()
{
    Debug.Log("RespawnEnemy called at " + Time.time);
    StartCoroutine(RespawnAfterDelay());
}

private IEnumerator RespawnAfterDelay()
{
    // Đợi một thời gian trước khi hồi sinh
    yield return new WaitForSeconds(2f); // Thời gian hồi sinh có thể thay đổi
    Debug.Log("Hồi sinh quái vật");
}
}
