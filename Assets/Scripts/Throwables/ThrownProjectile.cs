using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrownProjectile : MonoBehaviour
{
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public GameObject myPickUp;

    [HideInInspector]
    public Interactable interactable;
    [HideInInspector]
    public Outline outline;
    [HideInInspector]
    public SphereCollider sphereCollider;
    [HideInInspector]
    public bool isInPickUpState;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        outline = GetComponent<Outline>();
        sphereCollider = GetComponent<SphereCollider>();
        
        if (outline != null) 
            outline.enabled = false;

        PickUpSetActive(true);
    }

    public void PickUpSetActive(bool value)
    {
        isInPickUpState = value;

        if (interactable != null)
            interactable.enabled = value;

        if (sphereCollider != null)
            sphereCollider.enabled = value;

        if (value)
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        else
            gameObject.layer = LayerMask.NameToLayer("Default");

    }
}
