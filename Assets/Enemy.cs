using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Statistics:")]
    public float Health = 100;


    [Header("State:")]
    public bool isStuned;


    //Other necessary variables to make other scripts work
    [HideInInspector]
    public int lastMeleeIndex;

    void Start()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (Health <= 0) return;

        print("muie");

        Health -= damage;

        if (Health <= 0) Die();
    }
  
    public void Die()
    {
        print(this.gameObject.name +" has died");
    }
}
