using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager instance = null;
    public LayerMask objectsAffectedByExplosions;
    public GameObject ExplosionEffect;
    public GameObject SoundPlayer;
    public string explosionSoundName;
    [Tooltip("What percentage of the push radius should the attack radius be")]
    public float pushToAttackRadiusRatio = 0.5f;
    [Tooltip("Minimum distance for which the explosion is guaranteed to have effect")]
    public float minimumExplosionDistance = 1f;

    void Awake()
    {
        // Singleton: A single explosion manager can exist.
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private bool IsAffectedByExplosion(Vector3 explosionPosition, GameObject target)
    {
        Vector3 targetPosition = target.transform.position;
        float distance = Vector3.Distance(explosionPosition, targetPosition);
        if (distance <= minimumExplosionDistance)
            return true;

        if (Physics.Raycast(explosionPosition, targetPosition - explosionPosition, out RaycastHit hit))
        {
            return hit.collider.gameObject.Equals(target);
        }

        return false;
    }

    // position - center of explosion
    // pushRadius - radius used for the push force
    // damage - explosion damage (at the center of explosion)
    // pushForce - amount of force the target receives at the center of the explosion
    public void Explode(Vector3 position, float pushRadius, float damage, float pushForce)
    {
        float trueDamage;
        ParticleSystem particleExplosion = Instantiate(ExplosionEffect, position, Quaternion.identity).GetComponent<ParticleSystem>();
        var main = particleExplosion.main;
        main.startLifetime = pushRadius / particleExplosion.main.startSpeed.constant + 0.2f; 

        Collider[] hits = Physics.OverlapSphere(position, pushRadius, objectsAffectedByExplosions);

        List<EnemyMaster> enemiesHitByExplosion = new List<EnemyMaster>();

        foreach (Collider hit in hits)
        {
            if (!IsAffectedByExplosion(position, hit.gameObject))
                continue;

            switch (LayerMask.LayerToName(hit.gameObject.layer))
            {
                case "Enemy":
                    // Damage decreases proportional to the distance squared.
                    //trueDamage = 1f / Mathf.Pow(Vector3.Distance(position, hit.transform.position), 2) * damage;

                    Rigidbody enemyRB = hit.gameObject.GetComponentInParent<Rigidbody>();
                    if (enemyRB == null)
                        break;
                    enemyRB.AddExplosionForce(pushForce, position, pushRadius, 0f, ForceMode.Impulse);

                    EnemyMaster enemyStats = hit.gameObject.GetComponentInParent<EnemyMaster>();
                    if (enemyStats == null || enemiesHitByExplosion.Contains(enemyStats))
                        break;
                    enemiesHitByExplosion.Add(enemyStats);
                    enemyStats.TakeDamage(damage);
                    
                    break;
                case "Player":
                    // The amount of damage the player takes is proportional to the distance from the center of explosion.
                    // The player can't take damage if he is farther than the attack radius (push radius multiplied by a ratio).
                    float dist = Vector3.Distance(position, hit.transform.position);
                    if (dist < pushRadius * pushToAttackRadiusRatio)
                        Player.m.TakeDamage(damage * (pushRadius - dist));

                    Player.m.gameObject.GetComponent<Rigidbody>().AddExplosionForce(pushForce, position, pushRadius, 0f, ForceMode.Impulse);
                    break;
                case "Pushable":
                    Rigidbody pushableRB = hit.gameObject.GetComponent<Rigidbody>();
                    pushableRB.AddExplosionForce(pushForce, position, pushRadius, 0f, ForceMode.Impulse);
                    break;
                case "Explosive":
                    hit.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
                    break;
            }
        }

        PlaySoundThenDestroy playSoundScript = Instantiate(SoundPlayer, position, Quaternion.identity).GetComponent<PlaySoundThenDestroy>();
        playSoundScript.SoundToPlay = explosionSoundName;
    }


}
