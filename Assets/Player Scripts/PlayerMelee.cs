using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{

    // The script is made with melee animations in mind
    // the variables that should be controlled during animations are: DamagePoint and isMeleeAttacking

    [Header("Important variables: ")]
    public KeyCode meleeKey = KeyCode.Mouse0;
    public bool isMeleeAttacking = false;
    public bool canAttack = true;

    // The AttackIndex system is in place so the same attack can't hit an enemy more than one, each time the player inputs an attack a new attackIndex is generated,
    // when the enemy is hit, it remebers the last AttackIndex of the attack that hit it, because the player melee does damage during multiple frames, the same attack 
    // can deal damage to an enemy each frame the variable isMeleeAttacking is true, this results in the enemy being hit potentially hundreds of times by the same attack,
    // the CurrentMeleeIndex and lastMeleeIndex in the Enemy Script prevents that 
    [HideInInspector]
    public int CurrentMeleeIndex = 0;

    void Update()
    {
        // stop the melee script if the player isn't in the melee attack type
        if (Player.m.AttackType != "melee")
            return;

        // start attacking
        if (Input.GetKey(meleeKey) && canAttack)
        {
            
            Player.m.weaponManager.weaponAnimator.SetTrigger("StartAttack");

            canAttack = false;
            
            isMeleeAttacking = true;

            CurrentMeleeIndex++;
            if (CurrentMeleeIndex >= 1000000)
                CurrentMeleeIndex = 0;

            Invoke("stopAttacking", 0.5f);
        }

        // check for enemies in melee range
        if (isMeleeAttacking)
        {
            DealDamageFromDamagePoint(Player.m.weaponManager.currentWeapon.DamageSphereOrigin, Player.m.weaponManager.currentWeapon.DamageSphereRadius);
        }

    }

    public void DealDamageFromDamagePoint(Transform origin, float radius)
    {

        //detect enemy
        Collider[] hitObjects = Physics.OverlapSphere(origin.position, radius, Player.m.enemyLayer);

        foreach (Collider obj in hitObjects)
        {
            switch (LayerMask.LayerToName(obj.gameObject.layer))
            {
                case "Enemy":
                    EnemyMaster enemy = obj.gameObject.GetComponentInParent<EnemyMaster>();
                    if (enemy != null)
                    {
                        print(obj.gameObject.name); 

                        if (CurrentMeleeIndex != enemy.lastMeleeIndex)
                        {
                            enemy.lastMeleeIndex = CurrentMeleeIndex;
                            enemy.TakeDamage(Player.m.weaponManager.currentWeapon.meleeDamage, obj.gameObject, transform.forward * 30f);
                        }
                    }
                    break;

            }

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
