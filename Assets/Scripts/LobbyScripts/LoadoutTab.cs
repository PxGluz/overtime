using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutTab : MonoBehaviour
{

    public class LoadoutChoice
    {
        public GameObject model;
        public string choiceName;
        public bool isUnlocked;
    }

    [HideInInspector]
    public List<LoadoutChoice> loadoutChoice = new List<LoadoutChoice>();

    public ListDisplay listDisplay;

    // Update is called once per frame
    void Update()
    {
        WeaponManager.Weapon[] weaponList = Player.m.weaponManager.WeaponsList;
        for (int i = 1; i < weaponList.Length; i++)
        {
            LoadoutChoice choice = new LoadoutChoice();
            foreach (Transform child in weaponList[i].WeaponModelOnPlayer.transform)
                if (child.name == "AnimationPoint")
                    choice.model = child.gameObject;
            choice.choiceName = weaponList[i].name;
            choice.isUnlocked = weaponList[i].isUnlocked;
            loadoutChoice.Add(choice);
        }
        listDisplay.ResetList(loadoutChoice);
        enabled = false;
    }
}
