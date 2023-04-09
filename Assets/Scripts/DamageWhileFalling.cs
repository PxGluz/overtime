using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageWhileFalling : MonoBehaviour
{
    [HideInInspector]
    public bool isActivated = false;

    public float damage = 100;

    public List<GameObject> alreadyHitObjects = new List<GameObject>();


    private void OnCollisionEnter(Collision collision)
    {
        if (!isActivated)
            return;

        if (alreadyHitObjects.Contains(collision.gameObject))
            return;

        alreadyHitObjects.Add(collision.gameObject);

        EnemyMaster enemy = collision.gameObject.GetComponentInParent<EnemyMaster>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        print(collision.gameObject + "  " + Player.m.gameObject);
        if (collision.gameObject == Player.m.gameObject)
        {
            Player.m.TakeDamage(damage);
        }

        Invoke(nameof(DeactivateCollisionDetection), 0.3f);
    }

    private void DeactivateCollisionDetection()
    {
        isActivated = false;
    }

}

