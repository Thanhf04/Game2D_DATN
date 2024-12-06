using UnityEngine;

public class Sword : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (player != null)
            {
                NPCQuest npcQuest = FindObjectOfType<NPCQuest>();
                if (npcQuest != null)
                {
                    npcQuest.FindSword();
                }

                Dichuyennv1 playerScript = player.GetComponent<Dichuyennv1>();
                if (playerScript != null)
                {
                    playerScript.UpdateQuest();
                }

                Destroy(gameObject);
            }
        }
    }
}
