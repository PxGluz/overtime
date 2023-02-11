using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string itemName;
    public bool canBePickedUp = false;
    public bool JustDestoy = false;
    public bool TriggerFunction = false;
    
    [HideInInspector]public Outline myOutline;

    public void Start()
    {
        TryGetComponent<Outline>(out myOutline);
        
        if (myOutline != null)
            myOutline.enabled= false;
    }
}
