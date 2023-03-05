using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    [System.Serializable]
    public class Weapon
    {
        public string name;
        [Header("Properties: ")]
        public GameObject WeaponModelOnPlayer;
        public GameObject WeaponPrefab;
        [Tooltip("melee,ranged,throwing")]
        public AttackType attackType;
        [Tooltip("GenericMelee, GenericRanged")]
        public AnimationType animationType;

        [Header("Throwing: ")]
        public float throwDamage = 10;
        public float throwForce = 20;
        public float throwUpwardForce = 1;

        [Header("Melee: ")]
        public Transform DamageSphereOrigin;
        public float meleeDamage;
        public float DamageSphereRadius;
        public float meleeAttackSpeed;


        [Header("Ranged: ")]
        public Transform shootPoint;
        public float bulletDamage;
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

    public void ChangeWeapon(string name, int quantity = 1, bool dropCurrentWeapon = true)
    {

        if (currentWeapon.name != "" && currentWeapon.name != "Fists" && name != "Fists" && dropCurrentWeapon)
            Player.m.playerThrow.DropWeapon();

        foreach (Weapon weapon in WeaponsList)
        {
            if (name.ToLower() == weapon.name.ToLower())
            {
                DeactivateNonSelectedWeapons(weapon.name);

                currentWeapon = weapon;

                Player.m.AttackType = weapon.attackType.ToString();

                if (weapon.attackType.ToString() == "melee")
                {
                    if (Player.m.weaponManager.currentWeapon.animationType.ToString() == "GenericMelee")
                        weaponAnimator.Play("GenericMelee");
                    else
                        weaponAnimator.Play(weapon.name);
                }

                if (weapon.attackType.ToString() == "ranged")
                {
                    Player.m.playerShooting.bulletsleft = quantity;
                    Player.m.playerShooting.AmmoDisplayParent.SetActive(true);
                }
                else
                {
                    Player.m.playerShooting.AmmoDisplayParent.SetActive(false);
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

    public string GetWeaponType(string weaponName)
    {
        foreach (Weapon weapon in WeaponsList)
        {
            if (weapon.name == weaponName)
            {
                return weapon.attackType.ToString();
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
            interactable.quantity = GetWeaponByName(interactable.itemName).gunMagazineSize;
    }


    public enum AnimationType
    {
        None, GenericRanged, GenericMelee
    }

    public enum AttackType
    {
        melee, ranged, throwing
    }
}
