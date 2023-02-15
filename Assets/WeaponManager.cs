using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Weapon[] WeaponsList;



    public void ChangeWeapon(string name)
    {
        foreach (Weapon weapon in WeaponsList)
        {
            if (name.ToLower() == weapon.name.ToLower())
            {
                DeactivateNonSelectedWeapons(weapon.name);

                Player.m.AttackType = weapon.type;

                break;
            }
        }
    }

    private void DeactivateNonSelectedWeapons(string selectedWeapon)
    {
        foreach (Weapon weapon in WeaponsList)
        {
            if (selectedWeapon == weapon.name)
                weapon.WeaponModelOnPlayer.SetActive(true);
            else
                weapon.WeaponModelOnPlayer.SetActive(false);
        }
    }
}
