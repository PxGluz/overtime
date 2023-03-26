using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
        public DamageType damageType;
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
        [SerializeField]
        [Range(0.0f, 0.2f)]
        [Tooltip("This is the time between bullets when shooting more than one bullets per tap")]
        public float gunTimeBetweenShots = 0.05f;
        public bool gunAllowButtonHold;

        [Header("Loadout Related")] 
        public bool isUnlocked;
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

    private void Start()
    {
        weaponAnimator = GetComponent<Animator>();
        ChangeWeapon("Fists");
        LoadAllGuns();
        //foreach (Transform child in currentWeapon.WeaponModelOnPlayer.transform)
        //    if (child.name == "AnimationPoint")
        //        animationPoint = child;
        weaponIsInPlace = true;
    }

    public void ChangeWeapon(string name, int quantity = 1, bool dropCurrentWeapon = true, Transform interactableObject = null)
    {

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
        weaponCopy.damageType= weapon.damageType;
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

    // Function for setting items to their place
    /*
    public void SendItemToPosition()
    {
        if (inPlace)
            return;

        animationPoint.position = Vector3.SmoothDamp(
            animationPoint.position, animationPoint.parent.position, ref ref1, smoothTime);

        Vector3 tempDest = animationPoint.parent.eulerAngles;
        Vector3 tempAnim = animationPoint.eulerAngles;

        if (tempDest.x >= 180)
            tempDest -= Vector3.right * 360f;
        if (tempDest.y >= 180)
            tempDest -= Vector3.up * 360f;
        if (tempDest.z >= 180)
            tempDest -= Vector3.forward * 360f;
        
        if (tempAnim.x >= 180)
            tempAnim -= Vector3.right * 360f;
        if (tempAnim.y >= 180)
            tempAnim -= Vector3.up * 360f;
        if (tempAnim.z >= 180)
            tempAnim -= Vector3.forward * 360f;
        
        tempAnim = Vector3.SmoothDamp(
            tempAnim, tempDest, ref ref2, smoothTime);
        animationPoint.transform.eulerAngles = tempAnim;

        if ((tempAnim - tempDest).magnitude <= 0.1f) 
            inPlace = true;
    }
    */

    /*
     foreach (Transform child in currentWeapon.WeaponModelOnPlayer.transform)
                        if (child.name == "AnimationPoint")
                            animationPoint = child;

                    animationPoint.position = interactableObject.position;
                    animationPoint.eulerAngles = interactableObject.eulerAngles;

                    inPlace = false;
     */

}
