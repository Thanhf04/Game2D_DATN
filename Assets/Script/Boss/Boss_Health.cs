﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Health : MonoBehaviour
{
    [SerializeField] private Slider HealthBoss;
    [SerializeField] private GameObject Boss;
    public int maxHealth = 100;
    public int currentHealth;
    Animator animator;
    public GameObject prefabsItem;
    public GameObject Panel_KillBoss;
    Player Player;

    // Start is called before the first frame update
    void Start()
    {
        HealthBoss.maxValue = maxHealth;
        HealthBoss.value = maxHealth;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        Player = FindObjectOfType<Player>();
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        animator.SetBool("Hit", true);
        StartCoroutine(ResetHitAnimation());
        HealthBoss.value = currentHealth;

    }
    public void Die()
    {
        //Player.LevelSlider(100);
        animator.SetBool("Death", true);
        Destroy(gameObject, 2f);
        Panel_KillBoss.SetActive(true);
        DropItem();
    }
    public void DropItem()
    {
        if (prefabsItem != null)
        {
            Instantiate(prefabsItem, transform.position, Quaternion.identity);
        }
    }
    public void ClosePanel()
    {
        Panel_KillBoss.SetActive(false);
    }
    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Hit", false);
    }
}
