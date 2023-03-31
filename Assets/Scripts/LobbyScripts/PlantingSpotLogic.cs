using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlantingSpotLogic : MonoBehaviour
{
    [HideInInspector] public GameObject minDistanceObject;

    [Header("AnimationRelated")] public float animationSpeed;
    public Vector3 animationPeak;

    [HideInInspector] public int currentPlanting;
    
    void Update()
    {
        if(currentPlanting == -1)
        {
            minDistanceObject = null;
            foreach (Transform child in transform)
            {
                if (minDistanceObject == null)
                    minDistanceObject = child.gameObject;
                Vector3 cameraForward = new Vector3(Player.m.MainCamera.transform.forward.x,
                    Player.m.MainCamera.transform.position.y, 
                    Player.m.MainCamera.transform.forward.z);
                Vector3 childProjection = new Vector3(child.position.x, 
                    Player.m.MainCamera.transform.position.y, 
                    child.position.z);
                Vector3 minProjection = new Vector3(minDistanceObject.transform.position.x,
                    Player.m.MainCamera.transform.position.y, 
                    minDistanceObject.transform.position.z);
                if (Vector3.Angle(Player.m.MainCamera.transform.position - childProjection, cameraForward) > 
                    Vector3.Angle(Player.m.MainCamera.transform.position - minProjection, cameraForward) && 
                    Vector3.Distance(Player.m.MainCamera.transform.position, childProjection) < 5)
                    minDistanceObject = child.gameObject;
            }
        }
        foreach (Transform child in transform)
        {
            Transform childButton = child.GetComponentInChildren<PlantingSpot>().transform;
            if (child.gameObject == minDistanceObject &&
                Vector3.Distance(minDistanceObject.transform.position, Player.m.transform.position) < 5f)
            {
                childButton.localScale =
                    Vector3.Lerp(childButton.localScale, new Vector3(0.15f,0.3f,0.4f), animationSpeed * 2);
                child.localScale = Vector3.Lerp(child.localScale, animationPeak, animationSpeed);
                childButton.GetComponent<Collider>().enabled = true;
            }
            else
            {
                childButton.localScale =
                    Vector3.Lerp(childButton.localScale, new Vector3(0.15f,0,0.4f), animationSpeed * 2);
                child.localScale = Vector3.Lerp(child.localScale, Vector3.one, animationSpeed);
                childButton.GetComponent<Collider>().enabled = false;
            }

            float angleToRotate = Vector3.Angle(
                new Vector3(Player.m.transform.position.x, child.position.y, Player.m.transform.position.z) - child.position,
                child.right
            );
            childButton.eulerAngles = new Vector3(childButton.eulerAngles.x, 
                (childButton.position.z > Player.m.transform.position.z? -1 : 1) * angleToRotate + 90f, 
                                                childButton.eulerAngles.z) ;
        }
    }
}
