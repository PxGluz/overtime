using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{

    [Header("Important variables: ")]
    private EnemyMaster enemy;
    public Transform DamageSphereOrigin;
    public LayerMask PlayerLayer;
    public float DamageSphereRange;
    public float DistanceToStartPunch;
    public Transform KnifePosition;

    [Header("Statistics: ")]
    public float meleeDamage;
    public float meleeAttackSpeed;

    [Header("Animations durations")]
    private float enemyPunchDuration;

    //States:
    [HideInInspector]
    public bool isMeleeAttacking = false;
    private bool canAttack = true;
    private bool thisAttackHasHitThePlayer;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyMaster>();
        enemy.animator.SetLayerWeight(1, 1);

        if (enemy.enemyType.ToString() != "Melee")
            this.enabled = false;

        PutWeaponInHand ();
      
        UpdateAnimClipTimes();
    }

    void PutWeaponInHand()
    {
        GameObject weaponInHand = Instantiate (Player.m.weaponManager.GetWeaponByName(enemy.myWeapon).WeaponPrefab,KnifePosition.transform.position, KnifePosition.transform.rotation);
        weaponInHand.transform.parent = KnifePosition.transform;

        foreach (var comp in weaponInHand.GetComponents<Component>())
        {
            if (!(comp is Transform))
            {
                Destroy(comp);
            }
        }

        Destroy(weaponInHand.GetComponent<Rigidbody>());
        Destroy(weaponInHand.GetComponent<DamageOnCollision>());
    }

    void Update()
    {

        // start attacking
        if (enemy.enemyMovement.canSeePlayer && Vector3.Distance(transform.position,Player.m.transform.position) <= DistanceToStartPunch && canAttack)
        {
            enemy.animator.SetTrigger("StartPunch");
            
            canAttack = false;

            thisAttackHasHitThePlayer = false;

            Invoke(nameof(stopAttacking), enemyPunchDuration);
        }

        // check for targets in melee range
        if (isMeleeAttacking)
            DealDamageFromDamagePoint(DamageSphereOrigin, DamageSphereRange);
    }

    public void DealDamageFromDamagePoint(Transform origin, float radius)
    {
        //detect player
        Collider[] hitObjects = Physics.OverlapSphere(origin.position, radius, PlayerLayer);

        foreach (Collider obj in hitObjects)
        {
            switch (LayerMask.LayerToName(obj.gameObject.layer))
            {
                case "Player":
                    if (!thisAttackHasHitThePlayer)
                    {
                        thisAttackHasHitThePlayer = true;
                        Player.m.TakeDamage(meleeDamage);
                    }
                break;
            }
        }
    }

    private void stopAttacking() { isMeleeAttacking = false; Invoke(nameof(resetCanAttack), meleeAttackSpeed + 0.1f); }// +0.1f because of the transition duration
    private void resetCanAttack() { canAttack = true; }

    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = enemy.animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Enemy Punch":
                    enemyPunchDuration = clip.length;
                    break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(DamageSphereOrigin.position, DamageSphereRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y +1, transform.position.z), DistanceToStartPunch);
    }
}
