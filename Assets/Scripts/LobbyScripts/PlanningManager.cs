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

    [Header("AnimationRelated")] public float animationSpeed;

    [HideInInspector]public List<Level.LevelInfo> levelToDisplay;
    [HideInInspector]public bool coroutineRunning;
    
    private Vector3 initialLayoutRotation;
    private float initialScaleY;

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
            initialScaleY = layoutRoot.localScale.y;
            initialLayoutRotation = layoutRoot.parent.eulerAngles;
            layoutRoot.parent.position += Vector3.up * layoutRoot.lossyScale.y;
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
        if (coroutineRunning)
            enabled = false;
        coroutineRunning = true;
    }

    public void ResetLayout()
    {
        foreach(Transform child in layoutRoot)
            Destroy(child.gameObject);
        layoutRoot.parent.eulerAngles = initialLayoutRotation;
        layoutRoot.position = layoutRoot.parent.position;
        layoutRoot.localScale = new Vector3(1, 0, 1);
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
                Mathf.Abs(layoutRoot.position.x - transform.position.x) * 2, 
                Mathf.Abs(layoutRoot.position.z - transform.position.z) * 2,
                planningMaterial,
                levelToDisplay);
        }
        else
            Debug.LogError("levelToDisplay has no levels! It was not passed down after enabling the script!");

        yield return 0;
        layoutRoot.parent.eulerAngles += Vector3.right * -90f;
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
    }
    
    // Update is called once per frame
    void Update()
    {
        layoutRoot.localScale = Vector3.Lerp(layoutRoot.localScale, new Vector3(layoutRoot.localScale.x, initialScaleY, layoutRoot.localScale.z), animationSpeed);
        if (!coroutineRunning)
            StartCoroutine(UpdateFunctions());
    }
}
