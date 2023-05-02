using System.Collections.Generic;
using UnityEngine;

public class PlantingSpot : MonoBehaviour
{
    [HideInInspector] public List<LoadoutTab.LoadoutChoice> loadoutChoice = new List<LoadoutTab.LoadoutChoice>();
    [HideInInspector] public int selectedChoice = 0;
    [HideInInspector] public ListDisplay listDisplay;
    [HideInInspector] public PlantingSpotLogic pLogic;
    [HideInInspector] public bool toggle;

    private Vector3 offset = new(0, 1, 0);
    private bool checker = true;

    public void UpdateChoice(int newChoice)
    {
        if (newChoice != 0)
        {
            selectedChoice = newChoice;
            foreach (Transform child in pLogic.transform)
            {
                PlantingSpot currentPSpot = child.GetComponentInChildren<PlantingSpot>();
                if (currentPSpot != this)
                    currentPSpot.selectedChoice = 0;
            }
        }
    }

    void Start()
    {
        loadoutChoice = new List<LoadoutTab.LoadoutChoice>();
        GameObject empty = new GameObject();
        LoadoutTab.LoadoutChoice emptyChoice = new LoadoutTab.LoadoutChoice();
        emptyChoice.model = empty;
        emptyChoice.choiceName = "None";
        emptyChoice.isUnlocked = true;
        loadoutChoice.Add(emptyChoice);
        WeaponManager.Weapon[] weaponList = Player.m.weaponManager.WeaponsList;
        for (int i = 1; i < weaponList.Length; i++)
        {
            if (weaponList[i].isDebug)
                continue;
            LoadoutTab.LoadoutChoice choice = new LoadoutTab.LoadoutChoice();
            foreach (Transform child in weaponList[i].WeaponModelOnPlayer.transform)
                if (child.name == "AnimationPoint")
                    choice.model = child.gameObject;
            choice.choiceName = weaponList[i].name;
            choice.isUnlocked = weaponList[i].isUnlocked;
            loadoutChoice.Add(choice);
        }
        enabled = false;
    }

    void Update()
    {
        if (!Input.GetKey(Player.m.interact.interactKey))
            checker = false;
        if (!checker)
        {
            toggle = !toggle;
            checker = true;
            if (toggle)
            {
                pLogic.currentPlanting = transform.parent.GetSiblingIndex();
                foreach (Transform child in pLogic.transform)
                {
                    PlantingSpot currentPSpot = child.GetComponentInChildren<PlantingSpot>();
                    if (currentPSpot != this)
                        currentPSpot.toggle = false;
                }
                listDisplay.gameObject.SetActive(true);
                listDisplay.forceClose = false;
                listDisplay.transform.position = transform.position + offset;
                listDisplay.ResetList(loadoutChoice, pSpot: this);
                enabled = false;
            }
            else
            {
                listDisplay.forceClose = true;
                enabled = false;
            }
        }
    }
}
