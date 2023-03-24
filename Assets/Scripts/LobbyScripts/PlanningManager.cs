using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningManager : MonoBehaviour
{

    [Header("StaticReferences")] 
    public Transform contractsRoot, layoutRoot;
    public ChoiceManager choiceManager;

    // Update is called once per frame
    void Update()
    {
        enabled = false;
    }
}
