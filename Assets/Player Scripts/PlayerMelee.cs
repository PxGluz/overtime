using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{

    // The script is made with melee animations in mind
    // the variables that should be controlled during animations are: DamagePoint and isMeleeAttacking

    [Header("Important variables: ")]
    public KeyCode meleeKey = KeyCode.Mouse0;
    public bool isMeleeAttacking = false;
    public bool canAttack = true;

    public bool ThisAttackHasDealtDamge;

    void Update()
    {
        // stop the melee script if the player isn't in the melee attack type
        if (Player.m.AttackType != "melee")
            return;

        if (!Player.m.weaponManager.weaponIsInPlace)
            return;

        // start attacking
        if (Input.GetKey(meleeKey) && canAttack)
        {
            Player.m.weaponManager.weaponAnimator.SetTrigger("StartAttack");

            AudioManager.AM.Play("slash");

            canAttack = false;

            ThisAttackHasDealtDamge = false;

            Invoke("stopAttacking", 0.7f);
        }

        // check for enemies in melee range
        if (isMeleeAttacking)
        {
            DealDamageFromDamagePoint(Player.m.weaponManager.currentWeapon.DamageSphereOrigin, Player.m.weaponManager.currentWeapon.DamageSphereRadius);
        }

    }

    public void DealDamageFromDamagePoint(Transform origin, float radius)
    {
        if (ThisAttackHasDealtDamge)
            return;

        //detect enemy
        Collider[] hitObjects = Physics.OverlapSphere(origin.position, radius, Player.m.enemyLayer);

        EnemyMaster closestEnemy = null;
        float distanceToClosestEnemy = 100;
        Collider bodyPart = null;

        foreach (Collider obj in hitObjects)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) == "Enemy")
            {
                EnemyMaster enemy = obj.gameObject.GetComponentInParent<EnemyMaster>();
                if (enemy != null)
                {
                    ThisAttackHasDealtDamge = true;
                        
                    float dist = Vector3.Distance(obj.gameObject.transform.position, origin.position);
                    if (dist < distanceToClosestEnemy || closestEnemy == null)
                    {
                        closestEnemy = enemy;
                        distanceToClosestEnemy = dist;
                        bodyPart = obj;
                    }
                }
            }
        }

        if (closestEnemy != null)
        {
            RaycastHit contactHit;
            Physics.Raycast(Player.m.MainCamera.transform.position, Player.m.MainCamera.transform.forward, out contactHit);
            closestEnemy.TakeDamage(Player.m.weaponManager.currentWeapon.meleeDamage, bodyPart.gameObject, transform.forward * 30f);
            ThisAttackHasDealtDamge = true;
        }

    }


    private void stopAttacking(){ isMeleeAttacking = false; Invoke("resetCanAttack", Player.m.weaponManager.currentWeapon.meleeAttackSpeed); }
    private void resetCanAttack() { canAttack = true; }

    public Transform TestMeleeSpherePoint;
    public float TestMeleeSphereRadius;
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(TestMeleeSpherePoint.position, TestMeleeSphereRadius);
    }

}
