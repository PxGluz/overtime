using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageWhileFalling : MonoBehaviour
{

    public float damage = 100;

    public List<GameObject> alreadyHitObjects = new List<GameObject>();


    private void OnCollisionEnter(Collision collision)
    {

        if (alreadyHitObjects.Contains(collision.gameObject))
            return;

        alreadyHitObjects.Add(collision.gameObject);

        EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.ReceiveHit(damage);
        }

        print(collision.gameObject + "  " + Player.m.gameObject);
        if (collision.gameObject == Player.m.gameObject)
        {
            Player.m.TakeDamage(damage);
        }
    }

}
