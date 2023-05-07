using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadoutTab : MonoBehaviour
{

    public class LoadoutChoice
    {
        public GameObject model;
        public string choiceName;
        public bool isUnlocked;
    }
    
    [HideInInspector]public List<LoadoutChoice> loadoutChoice = new List<LoadoutChoice>();
    [HideInInspector]public int selectedChoice = 0;
    [HideInInspector]public ListDisplay listDisplay;

    private Vector3 destination;
    private bool checker = true;

    IEnumerator CloseOpenMenu()
    {
        transform.parent.parent.localScale = Vector3.Lerp(transform.parent.parent.localScale, destination, listDisplay.closeSpeed);
        if (Vector3.Distance(transform.position, Player.m.transform.position) < 10)
            destination = Vector3.one;
        else
            destination = Vector3.right + Vector3.forward;
        yield return 0;
        StartCoroutine(CloseOpenMenu());
    }

    void Start()
    {
        StartCoroutine(CloseOpenMenu());
        enabled = false;
    }

    void Update()
    {
        if (!Input.GetKey(Player.m.interact.interactKey))
            checker = false;
        if (!checker)
        {
            loadoutChoice = new List<LoadoutChoice>();
            GameObject empty = new GameObject();
            LoadoutChoice emptyChoice = new LoadoutChoice();
            emptyChoice.model = empty;
            emptyChoice.choiceName = "None";
            emptyChoice.isUnlocked = true;
            loadoutChoice.Add(emptyChoice);
            WeaponManager.Weapon[] weaponList = Player.m.weaponManager.WeaponsList;
            for (int i = 1; i < weaponList.Length; i++)
            {
                if (weaponList[i].isDebug)
                    continue;
                LoadoutChoice choice = new LoadoutChoice();
                foreach (Transform child in weaponList[i].WeaponModelOnPlayer.transform)
                    if (child.name == "AnimationPoint")
                        choice.model = child.gameObject;
                choice.choiceName = weaponList[i].name;
                choice.isUnlocked = weaponList[i].isUnlocked;
                loadoutChoice.Add(choice);
            }
            listDisplay.ResetList(loadoutChoice, lTab:this);
            enabled = false;
            checker = true;
        }
    }
}
