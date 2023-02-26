using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Interact : MonoBehaviour
{

    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.Mouse0;
    public float pickupRange = 2f;

    private Interactable itemBeingPickedUp;

    void Update()
    {
        SelectItemBeingPickedUpFromRay();

        if (itemBeingPickedUp != null)
        {
            if (Input.GetKeyDown(interactKey))
                UseInteractable();
        }
       
    }

    private void SelectItemBeingPickedUpFromRay()
    {
        Ray ray = Player.m.MainCamera.ViewportPointToRay(Vector3.one / 2f);
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, pickupRange, interactableLayer))
        {
            var hitItem = hitInfo.collider.GetComponent<Interactable>();

            if (hitItem == null || hitItem.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            {
                ResetItemBeingPickedUp();
            }
            else if (hitItem != null && hitItem != itemBeingPickedUp)
            {

                if (itemBeingPickedUp != hitItem)
                    ResetItemBeingPickedUp();

                itemBeingPickedUp = hitItem;

                if (itemBeingPickedUp.myOutline != null)
                    itemBeingPickedUp.myOutline.enabled = true;
            }
        }
        else
            ResetItemBeingPickedUp();

    }

    private void ResetItemBeingPickedUp()
    {
        if (itemBeingPickedUp == null)
            return;

        if (itemBeingPickedUp.myOutline != null)
            itemBeingPickedUp.myOutline.enabled = false;

        itemBeingPickedUp = null;
    }

    private void UseInteractable()
    {
        // Aici sunt functiile care se executa odata ce pickup-ul s-a terminat

        itemBeingPickedUp.TriggerFunction = true;

        if (itemBeingPickedUp.isWeaponPickUp)
        {
            Player.m.weaponManager.ChangeWeapon(itemBeingPickedUp.itemName,  itemBeingPickedUp.quantity);
            
            Destroy(itemBeingPickedUp.gameObject);

            return;
        }

        //if (itemBeingPickedUp.canBePickedUp)
        //    MoveItemToInventory();
        //else if (itemBeingPickedUp.JustDestoy)
        //    Destroy(itemBeingPickedUp.gameObject);

    }


    /*
    private void MoveItemToInventory()
    {
        Debug.Log(itemBeingPickedUp.itemName);
        switch (itemBeingPickedUp.itemName)
        {
            case:""
            break;
        }
    Destroy(itemBeingPickedUp.gameObject);
        itemBeingPickedUp = null;
    }
    */


}
