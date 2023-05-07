using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoDisplayOnGuns : MonoBehaviour
{
    [SerializeField]private GameObject ammo;
    void Start()
    {
        Interactable inter = GetComponent<Interactable>();
        foreach (WeaponManager.Weapon weapon in Player.m.weaponManager.WeaponsList)
            if (inter.itemName == weapon.name)
            {
                ammo.transform.localScale = new Vector3((float)inter.quantity / (float)weapon.gunMagazineSize, 1, 1);
            }

    }
}
