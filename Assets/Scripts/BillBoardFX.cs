using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardFX : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Player.m.MainCamera.transform);
    }
}