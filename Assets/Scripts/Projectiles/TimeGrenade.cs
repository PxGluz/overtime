using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeGrenade : MonoBehaviour
{
    private ThrownProjectile thrownProjectile;
    public GameObject SphereEffect;
    public LayerMask objectsAffectedByExplosions;

    public float FinalRadius;
    public float currentRadius;

    public float explosionDuration;

    private void Start()
    {
        thrownProjectile = GetComponent<ThrownProjectile>();
    }

    private IEnumerator ExplodeRef;
    private void OnCollisionEnter(Collision collision)
    {
        if (thrownProjectile.isInPickUpState || ExplodeRef != null)
            return;

        ExplodeRef = Explode();
        StartCoroutine(ExplodeRef);

    }

    public IEnumerator Explode() {

        GameObject sphereEffect = Instantiate(SphereEffect, transform.position, Quaternion.identity);
        sphereEffect.transform.localScale = new Vector3(0, 0, 0);
        Vector3 previousPosition = sphereEffect.transform.localScale;
        
        float timer = 0.0f;

        do {
            timer += Time.deltaTime;
            
            sphereEffect.transform.localScale = Vector3.Lerp(previousPosition, new Vector3(FinalRadius * 2, FinalRadius * 2, FinalRadius * 2), timer / explosionDuration);
            currentRadius = sphereEffect.transform.localScale.x / 2;

            Collider[] hits = Physics.OverlapSphere(sphereEffect.transform.position, currentRadius, objectsAffectedByExplosions);
            List<EnemyMaster> enemiesHitByExplosion = new List<EnemyMaster>();

            foreach (Collider hit in hits)
            {
                switch (LayerMask.LayerToName(hit.gameObject.layer))
                {
                    case "Enemy":
                        EnemyMaster enemyStats = hit.gameObject.GetComponentInParent<EnemyMaster>();
                        if (enemyStats == null || enemiesHitByExplosion.Contains(enemyStats) || enemyStats.isDead)
                            break;
                        enemiesHitByExplosion.Add(enemyStats);

                        enemyStats.IncapacitateEnemy();
                        if (enemyStats.animator != null)
                            enemyStats.animator.speed = 0;
                        break;
                }
            }

            yield return 0;

        }while (timer < explosionDuration);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gameObject.transform.position, FinalRadius);
    }

}
