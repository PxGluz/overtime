using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardFX : MonoBehaviour
{
	private Transform camTransform;

	Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        camTransform = Player.m.MainCamera.transform;
    }

    void Update()
    {
     	transform.rotation = camTransform.rotation * originalRotation;   
    }
}