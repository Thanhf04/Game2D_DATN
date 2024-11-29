using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class damageEnemy : MonoBehaviour
{
    Enemy enemy;
    NewPlayer playerHealth;

   void Start()
{
    if (transform.childCount > 0)
    {
        enemy = gameObject.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("The child object does not have an Enemy component!");
        }
    }
    else
    {
        Debug.LogError("This object has no children!");
    }
}


    void Update()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<NewPlayer>();
        }
        StartCoroutine(WaitForPlayerSpawn());
    }

    IEnumerator WaitForPlayerSpawn()
    {
        while (playerHealth == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var playerObj in players)
            {
                var playerPhotonView = playerObj.GetComponent<PhotonView>();
                if (playerPhotonView != null && playerPhotonView.IsMine)
                {
                    playerHealth = playerObj.GetComponent<NewPlayer>();
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("day ne");

            if (playerHealth == null)
            {
                playerHealth = other.GetComponent<NewPlayer>();
                if (playerHealth == null)
                {
                    Debug.LogError("playerHealth is null!");
                    return;
                }
            }

            if (enemy != null)
            {
                playerHealth.TakeDamage(enemy.damageAmount); // Gây sát thương cho người chơi
            }
            else
            {
                Debug.LogError("Enemy is not assigned or is null!");
            }
        }
    }
}
