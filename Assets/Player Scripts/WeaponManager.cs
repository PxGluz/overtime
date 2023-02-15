using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeaponManager;

public class WeaponManager : MonoBehaviour
{

    [System.Serializable]
    public class Weapon
    {
        [Header("Properties: ")]
        public GameObject WeaponModelOnPlayer;
        public GameObject WeaponPickupPrefab;
        public string name;
        [Tooltip("melee,shoot,throw")]
        public string type;
        public float damage;
        public int charges;

        [Header("Throwing: ")]
        public float throwForce;
        public float throwUpwardForce;
    }

    public Weapon currentWeapon;
    public Weapon[] WeaponsList;


    private void Start()
    {
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
}
