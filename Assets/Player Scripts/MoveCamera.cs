using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    void Update()
    {
        transform.position = Player.m.playerCam.cameraPosition.position;
    }
}
