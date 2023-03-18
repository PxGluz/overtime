using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class Interactable : MonoBehaviour
{
    public string itemName;
    public bool isWeaponPickUp = false;
    [HideInInspector] public bool isChoice;
    public int quantity;
    public bool TriggerFunction = false;
    
    public Transform myAttackPoint;
    public MonoBehaviour scriptToStart;
    
    [HideInInspector]public Outline myOutline;

    public void Start()
    {
        TryGetComponent<Outline>(out myOutline);

        if (myOutline != null)
        {
            myOutline.OutlineColor = new Color(myOutline.OutlineColor.r, myOutline.OutlineColor.g, myOutline.OutlineColor.b, 0);
            if (isWeaponPickUp)
                myOutline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    private void Update()
    {
        if (TriggerFunction && scriptToStart != null)
        {
            TriggerFunction = false;
            scriptToStart.enabled = true;
            if (isChoice)
            {
                ChoiceManager choiceManager = (ChoiceManager)scriptToStart;
                choiceManager.ChangeChoice(transform.GetSiblingIndex());
            }
        }
    }
}
