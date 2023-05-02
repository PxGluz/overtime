using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    void Update()
    {
        //transform.position = Player.m.playerCam.cameraPosition.position;
        transform.position = cameraPosition.position;
    }
}
