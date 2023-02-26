using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        [Tooltip("How much damage this weapon does")]
        public float damage = 10;
        [Tooltip("GenericMelee, GenericRanged")]
        public string animationType;

        [Header("Throwing: ")]
        public float throwDamage = 10;
        public float throwForce = 20;
        public float throwUpwardForce = 1;

        [Header("Melee: ")]
        public Transform DamageSphereOrigin;
        public float DamageSphereRadius;
        public float meleeAttackSpeed;


        [Header("Ranged: ")]
        public Transform shootPoint;
        [Tooltip("Speed of the bullet")]
        public float gunShootForce;
        [Tooltip("How much does the bullet go upward")]
        public float gunUpwardForce;
        [Tooltip("Shooting cooldown")]
        public float gunTimeBetweenShooting;
        public float gunSpread;
        [Tooltip("How much time does it take to reload")]
        public float gunReloadTime;
        [Tooltip("Total number of bullets in reload")]
        public int gunMagazineSize;
        [Tooltip("How many bullets are created on click")]
        public int gunBulletsPerTap = 1;
        [SerializeField][Range(0.0f, 0.2f)]
        [Tooltip("This is the time between bullets when shooting more than one bullets per tap")]
        public float gunTimeBetweenShots = 0.05f;
        public bool gunAllowButtonHold;
    }

    [HideInInspector]
    public Animator weaponAnimator;
    public Weapon currentWeapon;
    public Weapon[] WeaponsList;


    private void Start()
    {
        weaponAnimator = GetComponent<Animator>();
        ChangeWeapon("Fists");
        LoadAllGuns();

    }

    public void ChangeWeapon(string name, int quantity = 1)
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

                if (weapon.type == "shoot")
                {
                    Player.m.playerShooting.bulletsleft = quantity;

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

        //Gizmos.color = Color.yellow;

        //if (!Application.isPlaying || Player.m.AttackType != "melee") return;


        //Gizmos.DrawWireSphere(currentWeapon.DamageSphereOrigin.position, currentWeapon.DamageSphereRadius);


    }

    public string GetWeaponType(string weaponName)
    {
        foreach (Weapon weapon in WeaponsList)
        {
            if (weapon.name == weaponName)
            {
                return weapon.type;
            }
        }
        return "null";
    }

    public Weapon GetWeaponByName(string weaponName)
    {
        foreach (Weapon weapon in WeaponsList)
        {
            if (weapon.name == weaponName)
            {
                return weapon;
            }
        }
        Weapon nullWeapon = new Weapon();
        nullWeapon.name = "null";
        return nullWeapon;
    }

    public void LoadAllGuns()
    {
        object[] AllInteractablesInScene = FindObjectsOfType(typeof(Interactable));
        foreach (Interactable interactable in (Interactable[])AllInteractablesInScene)
        {
            //Interactable g = (Interactable)o;
            LoadGun(interactable);
            //Debug.Log(interactable.name);
        }
    }

    public void LoadGun(Interactable interactable)
    {
        interactable.quantity = GetWeaponByName(interactable.itemName).gunMagazineSize;
    }
}
