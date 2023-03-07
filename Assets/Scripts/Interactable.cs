using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string itemName;
    public bool isWeaponPickUp = false;
    public int quantity;
    public bool TriggerFunction = false;
    
    public Transform myAttackPoint;
    
    [HideInInspector]public Outline myOutline;

    public void Start()
    {
        TryGetComponent<Outline>(out myOutline);

        if (myOutline != null)
        {
            myOutline.OutlineColor = new Color(myOutline.OutlineColor.r, myOutline.OutlineColor.g, myOutline.OutlineColor.b, 0);
            myOutline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }
}
