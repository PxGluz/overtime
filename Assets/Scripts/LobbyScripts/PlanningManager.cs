using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningManager : MonoBehaviour
{

    [Header("Static References")]
    public Transform contractsRoot;
    public Transform layoutRoot;
    public Transform choiceRoot;
    public Material planningMaterial;
    public GameObject plantingSpotPrefab;

    [Header("AnimationRelated")] public float animationSpeed;
    [Header("StaticReferences")] public GameObject planningPrefab;

    [HideInInspector] public List<Level.LevelInfo> levelToDisplay;
    [HideInInspector] public bool coroutineRunning;
    [HideInInspector] public Contract currentContract;

    private Vector3 initialLayoutRotation;
    private float initialScaleY;

    /// <summary>
    /// return a list of two elements representing the planting spot that has a weapon selected on it.
    /// </summary>
    /// <returns>A list with two elements
    /// [0]: index of planting spot ();
    /// [1] index of weapon chosen (0 means none);</returns>
    public List<int> GetPlanning()
    {
        List<int> plantingChoice = new List<int>();
        foreach (Transform child in choiceRoot)
        {
            if (child.GetComponentInChildren<PlantingSpot>().selectedChoice != 0)
            {
                plantingChoice.Add(child.GetSiblingIndex());
                plantingChoice.Add(child.GetComponentInChildren<PlantingSpot>().selectedChoice);
                break;
            }
        }
        if (plantingChoice.Count == 0)
        {
            plantingChoice.Add(-1);
            plantingChoice.Add(0);
        }
        return plantingChoice;
    }

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
        }
        if (choiceRoot == null)
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
        transform.eulerAngles = Vector3.zero;
        foreach (Transform child in layoutRoot)
            Destroy(child.gameObject);
        foreach (Transform child in choiceRoot)
            Destroy(child.gameObject);
        layoutRoot.parent.eulerAngles = initialLayoutRotation;
        layoutRoot.position = layoutRoot.parent.position;
        layoutRoot.localScale = new Vector3(1, 0, 1);
        choiceRoot.GetComponent<PlantingSpotLogic>().minDistanceObject = null;
    }

    public void RotateLayout(Transform reference)
    {
        float angle = Vector3.Angle(reference.right, -transform.right);
        transform.eulerAngles += new Vector3(0, angle, 0);
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
        layoutRoot.parent.eulerAngles += Vector3.right * 90f;
        LevelConstructor.InsertPlanning(layoutRoot.gameObject, choiceRoot, planningPrefab);
        yield return 0;
        RotateLayout(currentContract.transform);
    }

    // Update is called once per frame
    void Update()
    {
        layoutRoot.localScale = Vector3.Lerp(layoutRoot.localScale, new Vector3(layoutRoot.localScale.x, initialScaleY, layoutRoot.localScale.z), animationSpeed);
        if (!coroutineRunning)
            StartCoroutine(UpdateFunctions());
    }
}
