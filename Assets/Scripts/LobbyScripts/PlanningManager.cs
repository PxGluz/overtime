using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanningManager : MonoBehaviour
{

    [Header("Static References")] 
    public Transform contractsRoot;
    public Transform layoutRoot;
    public ChoiceManager choiceManager;
    public Material planningMaterial;
    public GameObject plantingSpotPrefab;

    [HideInInspector]public List<Level.LevelInfo> levelToDisplay;

    private bool coroutineRunning;
    private Vector3 initialLayoutRotation;

    private void Start()
    {
        if (contractsRoot != null)
        {
            foreach (Transform child in contractsRoot)
                if (child.TryGetComponent(out Contract cont))
                    cont.planningManager = this;
        }
        else
        {
            Debug.LogWarning("contractRoot not set in PlanningManager");
            coroutineRunning = true;
        }
        if (layoutRoot == null)
        {
            Debug.LogWarning("layoutRoot not set: no reference for layout construction");
            coroutineRunning = true;
        }
        else
        {
            initialLayoutRotation = layoutRoot.eulerAngles;
        }
        if (choiceManager == null)
        {
            Debug.LogWarning("choiceManager not set: planting spots will not work");
            coroutineRunning = true;
        }
        if (planningMaterial == null)
        {
            Debug.LogWarning("planningMaterial not set: planning layout will not work");
            coroutineRunning = true;
        }
        if (plantingSpotPrefab == null)
        {
            Debug.LogWarning("plantingSpotPrefab not set: planting spots will not work");
            coroutineRunning = true;
        }
        enabled = false;
    }

    public void ResetLayout()
    {
        foreach(Transform child in layoutRoot)
            Destroy(child.gameObject);
        layoutRoot.eulerAngles = initialLayoutRotation;
    }
    
    private IEnumerator UpdateFunctions()
    {
        coroutineRunning = true;
        ResetLayout();
        yield return 0;
        if (levelToDisplay.Count > 0)
        {
            LevelConstructor.ConstructLevel(
                layoutRoot.gameObject, 
                Mathf.Abs(layoutRoot.position.x - transform.position.x), 
                Mathf.Abs(layoutRoot.position.z - transform.position.z),
                planningMaterial,
                levelToDisplay);
        }
        else
            Debug.LogError("levelToDisplay has no levels! It was not passed down after enabling the script!");
        //TODO: rotate the layoutRoot to match plane before adding planting spots
        //yield return 0;
        /*foreach (Transform level in layoutRoot)
        {
            GameObject currentPlantingSpotPivot = level.Find("PlantingSpot").gameObject;
            if (currentPlantingSpotPivot)
            {
                GameObject currentPlantingSpot = Instantiate(plantingSpotPrefab,
                    currentPlantingSpotPivot.transform.position, currentPlantingSpotPivot.transform.rotation,
                    choiceManager.transform);
                //TODO: add planting spot logic
            }
        }*/
        choiceManager.UpdateChoice();
        choiceManager.ChangeChoice(0);
        coroutineRunning = false;
        enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!coroutineRunning)
            StartCoroutine(UpdateFunctions());
    }
}
