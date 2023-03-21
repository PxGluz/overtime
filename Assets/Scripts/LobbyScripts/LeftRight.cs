using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRight : MonoBehaviour
{
    public bool left;
    public ListDisplay listDisplay;

    void Update()
    {
        if(left)
            listDisplay.MoveLeft();
        else
            listDisplay.MoveRight();
        enabled = false;
    }
}
