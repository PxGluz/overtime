using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using static WeaponManager;

public class WeaponManager : MonoBehaviour
{

    [System.Serializable]
    public class Weapon
    {
        [Header("Properties: ")]
        public GameObject WeaponModelOnPlayer;
        public GameObject WeaponPrefab;
        public string name;
        [Tooltip("melee,shoot,throw")]
        public string type;
        public float damage = 10;
        public int charges;
        public float useCooldown = 0.5f;
        [Tooltip("GenericMelee, GenericRanged")]
        public string animationType;

        [Header("Throwing: ")]
        public float throwDamage = 10;
        public float throwForce = 20;
        public float throwUpwardForce = 1;

        [Header("Melee: ")]
        public Transform DamageSphereOrigin;
        public float DamageSphereRadius;
    }

    [HideInInspector]
    public Animator weaponAnimator;
    public Weapon currentWeapon;
    public Weapon[] WeaponsList;


    private void Start()
    {
        weaponAnimator = GetComponent<Animator>();
        ChangeWeapon("Fists");

    }

    public void ChangeWeapon(string name)
    {

        if (currentWeapon.name != "Fists" && currentWeapon.name != "" && name != "Fists")
            Player.m.playerThrow.DropWeapon();

        foreach (Weapon weapon in WeaponsList)
        {
            if (name.ToLower() == weapon.name.ToLower())
            {
                DeactivateNonSelectedWeapons(weapon.name);

                currentWeapon = weapon;

                Player.m.AttackType = weapon.type;

                if (weapon.type == "melee")
                {
                    /*
                    if (weapon.DamageSphereOrigin != null)
                    {
                        Player.m.playerMelee.DamageSphereOrigin = weapon.DamageSphereOrigin;
                        Player.m.playerMelee.DamageSphereRadius = weapon.DamageSphereRadius;
                    }
                    */ 

                    if (Player.m.weaponManager.currentWeapon.animationType == "GenericMelee")
                    {
                        weaponAnimator.Play("GenericMelee");
                    }
                    else
                    {
                        weaponAnimator.Play(weapon.name);
                    }
                }
                break;
            }
        }
    }

    private void DeactivateNonSelectedWeapons(string selectedWeapon)
    {
        GameObject selectedWeaponModelOnPlayer = null;
        foreach (Weapon weapon in WeaponsList)
        {
            if (selectedWeapon.ToLower() == weapon.name.ToLower())
                selectedWeaponModelOnPlayer = weapon.WeaponModelOnPlayer;
            else
                weapon.WeaponModelOnPlayer.SetActive(false);
        }

        if (selectedWeaponModelOnPlayer != null)
            selectedWeaponModelOnPlayer.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;

        //if (!Application.isPlaying || Player.m.AttackType != "melee") return;


        //Gizmos.DrawWireSphere(currentWeapon.DamageSphereOrigin.position, currentWeapon.DamageSphereRadius);


    }
}
