using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivity = 1f;

    public Transform orientation;
    public Transform cameraPosition;

    [HideInInspector]
    public float xRotation, yRotation;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;

        // limit player look up and down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }


    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
