using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{

    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("State:")]
    public bool isStunned;
    public bool armored = false;

    [Header("References and others")]
    public Image healthBar;
    public float healthBarWidth;

    //Other necessary variables to make other scripts work
    [HideInInspector]
    public int lastMeleeIndex;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }

    public void ReceiveHit(float damage)
    {

        if (currentHealth <= 0) return;

        if (armored)
            damage /= 2;

        StartCoroutine(HealthBarAnimation(damage));
    }


    private void UpdateHealthBar()
    {
        float ratio = currentHealth / maxHealth;
        healthBar.rectTransform.sizeDelta = new Vector2(ratio * healthBarWidth, healthBar.rectTransform.sizeDelta.y);
    }

    private IEnumerator HealthBarAnimation(float damage)
    {
        float step = 0.01f;
        for (int i = 0; i < 100; i++)
        {
            currentHealth -= step * damage;

            UpdateHealthBar();

            yield return new WaitForSeconds(0.01f);
        }

        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        print(this.gameObject.name + " has died");
    }
}
