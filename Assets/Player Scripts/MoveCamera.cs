using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    
    //public Transform cameraposition

    // Update is called once per frame
    void Update()
    {
        //print (Player.m);
        transform.position = Player.m.playerCam.cameraPosition.position;
    }
}
