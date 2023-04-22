using System;
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
        public GameObject ammoBar = null;
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
        [SerializeField]
        [Range(0.0f, 0.2f)]
        [Tooltip("This is the time between bullets when shooting more than one bullets per tap")]
        public float gunTimeBetweenShots = 0.05f;
        public bool gunAllowButtonHold;

        [Header("Loadout Related")] 
        public bool isUnlocked;
    }

    [System.Serializable]
    public class WeaponData
    {
        public string name;
        public bool isUnlocked;

        public WeaponData(string name, bool isUnlocked)
        {
            this.name = name;
            this.isUnlocked = isUnlocked;
        }
    }

    [HideInInspector]
    public Animator weaponAnimator;
    [HideInInspector]
    public Weapon currentWeapon;
    public Weapon[] WeaponsList;

    [Header("Animation Settings")]
    public float smoothTime;

    [Header("Animation Related")]
    private Transform animationPoint;
    public bool weaponIsInPlace;

    public void SaveWeapons()
    {
        List<WeaponData> saveData = new List<WeaponData>();
        foreach (Weapon weapon in WeaponsList)
            saveData.Add(new WeaponData(weapon.name, weapon.isUnlocked));

        SerializationManager.Save("weapons", saveData);
    }

    private void Start()
    {
        weaponAnimator = GetComponent<Animator>();
        ChangeWeapon("Fists");
        weaponIsInPlace = true;

        object data = SerializationManager.Load("weapons");
        if (data != null)
        {
            List<WeaponData> weaponDatas = data as List<WeaponData>;
            foreach (WeaponData weaponData in weaponDatas)
            {
                foreach (Weapon weapon in WeaponsList)
                    if (weapon.name.Equals(weaponData.name))
                        weapon.isUnlocked = weaponData.isUnlocked;
            }
        }

        LoadAllGuns();
    }

    public void ChangeWeapon(string name, int quantity = 1, bool dropCurrentWeapon = true, Transform interactableObject = null)
    {
        if (name != "Fists")
            AudioManager.AM.Play("pickup");

        if (currentWeapon.name != "" && currentWeapon.name != "Fists" && name != "Fists" && dropCurrentWeapon)
            Player.m.playerThrow.DropWeapon();

        foreach (Weapon weapon in WeaponsList)
        {
            if (name.ToLower() == weapon.name.ToLower())
            {
                DeactivateNonSelectedWeapons(weapon.name);

                currentWeapon = weapon;

                // Weapon animation start

                if (interactableObject)
                {
                    foreach (Transform child in currentWeapon.WeaponModelOnPlayer.transform)
                        if (child.name == "AnimationPoint")
                        {
                            animationPoint = child;
                            break;
                        }

                    weaponIsInPlace = false;

                    smoothDampVelocityRef = new Vector3();
                    time = 0.0f;
                    previousRotation = interactableObject.rotation;

                    animationPoint.position = interactableObject.position;
                    animationPoint.rotation = interactableObject.rotation;

                }

                // Weapon animation end

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
                    Player.m.playerShooting.UpdateGunAmmoDisplay();
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
                return CreateCopyObjOfWeapon(weapon);
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

    private Vector3 smoothDampVelocityRef;
    private Quaternion previousRotation;
    private float time;

    private void Update()
    {
        if (animationPoint == null)
            return;

        if (!weaponIsInPlace && Vector3.Distance(animationPoint.position, animationPoint.parent.position) < 0.1f)
        {
            weaponIsInPlace = true;
        }
        else 
        {
            time += Time.deltaTime;

            animationPoint.position = Vector3.SmoothDamp(animationPoint.position, animationPoint.parent.position, ref smoothDampVelocityRef, smoothTime);
            animationPoint.rotation = Quaternion.Slerp(previousRotation, animationPoint.parent.rotation, time / smoothTime);

        }
    }

    public enum AnimationType
    {
        None, GenericRanged, GenericMelee
    }

    public enum AttackType
    {
        melee, ranged, throwing
    }

    private Weapon CreateCopyObjOfWeapon(Weapon weapon)
    {
        Weapon weaponCopy = new Weapon();
        weaponCopy.name = weapon.name;
        weaponCopy.WeaponModelOnPlayer = weapon.WeaponModelOnPlayer;
        weaponCopy.WeaponPrefab = weapon.WeaponPrefab;
        weaponCopy.attackType = weapon.attackType;
        weaponCopy.animationType = weapon.animationType;
        weaponCopy.throwDamage = weapon.throwDamage;
        weaponCopy.throwForce = weapon.throwForce;
        weaponCopy.throwUpwardForce = weapon.throwUpwardForce;
        weaponCopy.DamageSphereOrigin = weapon.DamageSphereOrigin;
        weaponCopy.meleeDamage = weapon.meleeDamage;
        weaponCopy.DamageSphereRadius = weapon.DamageSphereRadius;
        weaponCopy.meleeAttackSpeed = weapon.meleeAttackSpeed;
        weaponCopy.shootPoint = weapon.shootPoint;
        weaponCopy.bulletDamage = weapon.bulletDamage;
        weaponCopy.gunShootForce = weapon.gunShootForce;
        weaponCopy.gunUpwardForce = weapon.gunUpwardForce;
        weaponCopy.gunTimeBetweenShooting = weapon.gunTimeBetweenShooting;
        weaponCopy.gunSpread = weapon.gunSpread;
        weaponCopy.gunReloadTime = weapon.gunReloadTime;
        weaponCopy.gunMagazineSize = weapon.gunMagazineSize;
        weaponCopy.gunBulletsPerTap = weapon.gunBulletsPerTap;
        weaponCopy.gunTimeBetweenShots = weapon.gunTimeBetweenShots;
        weaponCopy.gunAllowButtonHold = weapon.gunAllowButtonHold;
        weaponCopy.isUnlocked = weapon.isUnlocked;
        return weaponCopy;
    }

}
