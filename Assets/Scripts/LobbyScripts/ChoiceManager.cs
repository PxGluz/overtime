using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    private int currentChoice;
    //[HideInInspector]
    public Material selectedColor, unselectedColor;

    public void ChangeChoice(int newChoice)
    {
        currentChoice = newChoice;
        foreach (Transform child in transform)
            if (child.GetSiblingIndex() != currentChoice)
                child.GetComponent<Renderer>().material = unselectedColor;
            else
                child.GetComponent<Renderer>().material = selectedColor;
    }

    public int GetChoice()
    {
        return currentChoice;
    }

    public void UpdateChoice()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Interactable inter))
            {
                inter.isChoice = true;
                inter.scriptToStart = this;
            }
            else
                Debug.LogError(child.name + " does not have Interactable.cs and should not be under a choice manager!");
        }
    }
    
    private void Start()
    {
        UpdateChoice();
    }
}
