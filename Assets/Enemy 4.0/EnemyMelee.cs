using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{

    [Header("Important variables: ")]
    private EnemyMaster enemy;
    //public Transform DamageSphereOrigin;
    public LayerMask PlayerLayer;
    //public float DamageSphereRange;
    public float DistanceToStartPunch;
    public Transform KnifePosition;
    public List<MeleeDamagePoint> meleeDamagePoints = new List<MeleeDamagePoint>();


    [Header("Statistics: ")]
    public float meleeDamage;
    public float meleeAttackSpeed;

    [Header("Animations durations")]
    public float enemyPunchDuration;

    //States:
    public bool isMeleeAttacking = false;
    private bool canAttack = true;
    private bool thisAttackHasHitThePlayer;

    [Serializable]
    public class MeleeDamagePoint
    {
        public Transform DamageSphereOrigin;
        public float DamageSphereRange;
    }

    private void Start()
    {
        enemy = GetComponentInParent<EnemyMaster>();
        enemy.animator.SetLayerWeight(1, 1);

        if (enemy.enemyType.ToString() != "Melee")
        {
            this.enabled = false;
            return;
        }

        UpdateAnimClipTimes();
    }

    void Update()
    {

        // start attacking
        if (enemy.enemyMovement.canSeePlayer && Vector3.Distance(transform.position, Player.m.transform.position) <= DistanceToStartPunch && canAttack)
        {
            enemy.animator.SetTrigger("StartPunch");

            canAttack = false;

            thisAttackHasHitThePlayer = false;

            Invoke(nameof(stopAttacking), enemyPunchDuration);
        }

        // check for targets in melee range
        if (isMeleeAttacking)
            DealDamageFromDamagePoint();
    }

    public void DealDamageFromDamagePoint()
    {
        //detect player
        List<Collider> hitObjects = new List<Collider>();
        foreach (MeleeDamagePoint damagePoint in meleeDamagePoints)
        {
            hitObjects.AddRange(Physics.OverlapSphere(damagePoint.DamageSphereOrigin.position, damagePoint.DamageSphereRange, PlayerLayer));
        }

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
                case "Enemy Stab":
                    enemyPunchDuration = clip.length;
                    break;
            }
        }
    }

    
    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < meleeDamagePoints.Count;i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeDamagePoints[i].DamageSphereOrigin.position, meleeDamagePoints[i].DamageSphereRange);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y +1, transform.position.z), DistanceToStartPunch);
    }
    
}
