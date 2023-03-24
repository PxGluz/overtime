using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRight : MonoBehaviour
{
    public bool left;
    public ListDisplay listDisplay;

    private bool checker;

    void Update()
    {
        if (!Input.GetKey(Player.m.interact.interactKey))
            checker = false;
        if (!checker)
        {
            if(left)
                listDisplay.MoveLeft();
            else
                listDisplay.MoveRight();
            checker = true;
            enabled = false;
        }

    }
}
