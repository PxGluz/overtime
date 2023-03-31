using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Contract : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public float highscore;

        public LevelData(string levelName, float highscore)
        {
            this.levelName = levelName;
            this.highscore = highscore;
        }
    }
    private Vector3 destination;
    private List<Collider> collidersList = new List<Collider>();
    private bool canPlan = true;
    
    [Header("Static References")]
    [HideInInspector]public Transform hologram, details;
    [HideInInspector]public Collider graphics;
    [HideInInspector]public TextMeshProUGUI levelName, levelScore;
    [HideInInspector]public GameObject levelLayout, layoutCenter;
    [HideInInspector]public Material layoutMaterial;
    [HideInInspector]public PlanningManager planningManager;
    [HideInInspector]public Transform loadoutTabsRoot;

    [Header("Animation Related")]
    public float animationSpeed;
    [HideInInspector]public bool closing;

    [Header("Levels")] 
    public List<Level.LevelInfo> levelList;

    [Header("Important for loading correct level")]
    [HideInInspector] public ChoiceManager difficulty; // Use GetChoice() to get the selected difficulty
    [HideInInspector] public List<Level.LevelInfo> selectedLevel = new List<Level.LevelInfo>(); // Contains levelScene
    // TODO: Add planning references for loading purposes

    /// Call this function in order to get loadout choices as a list depending on the number of tabs
    public List<int> GetLoadoutChoices()
    {
        if (loadoutTabsRoot == null)
        {
            Debug.LogError("GetLoadoutChoices was called but loadoutTabsRoot is not set: returning null");
            return null;
        }
        ListDisplay.ForceUpdateChoice();
        List<int> tabsChoices = new List<int>();
        foreach (Transform tab in loadoutTabsRoot)
            if (tab.TryGetComponent(out LoadoutTab loadoutTab))
                tabsChoices.Add(loadoutTab.selectedChoice);
        return tabsChoices;
    }

    private IEnumerator Start()
    {
        object data = SerializationManager.Load("levels");
        if (data != null)
        {
            List<LevelData> levelDatas = data as List<LevelData>;
            foreach (LevelData levelData in levelDatas)
            {
                foreach (Level.LevelInfo level in levelList)
                    if (level.levelName.Equals(levelData.levelName))
                        level.highscore = levelData.highscore;
            }
        }
        if (loadoutTabsRoot == null)
            Debug.LogWarning("loadoutTabsRoot not set: GetLoadoutChoices will not work");
        selectedLevel = new List<Level.LevelInfo>();
        
        // Creating level layout
        LevelConstructor.ConstructLevel(
            levelLayout,  
            2 * Mathf.Abs(layoutCenter.transform.position.y - levelLayout.transform.position.y), 
            2 * Mathf.Abs(layoutCenter.transform.position.z - levelLayout.transform.position.z),
            layoutMaterial, 
            levelList
            );
        
        foreach (Transform level in levelLayout.transform)
        {
            if (level != layoutCenter.transform)
            {
                Queue<Transform> levelChildren = new Queue<Transform>();
                List<CombineInstance> combine = new List<CombineInstance>();
                levelChildren.Enqueue(level);
                GameObject empty = Instantiate(new GameObject());
                while (levelChildren.Count > 0)
                {
                    Transform currentChild = levelChildren.Dequeue();
                    foreach (Transform child in currentChild)
                        levelChildren.Enqueue(child);
                    if (currentChild.TryGetComponent(out MeshFilter mFilter))
                    {
                        CombineInstance temp = new CombineInstance();
                        temp.mesh = mFilter.sharedMesh;
                        temp.transform = mFilter.transform.localToWorldMatrix;
                        combine.Add(temp);
                        mFilter.transform.SetParent(empty.transform);
                    }
                }
                CombineInstance[] combineArray = combine.ToArray();
                MeshFilter meshFilter = empty.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
                meshFilter.mesh.CombineMeshes(combineArray);
                empty.gameObject.layer = 7; // Interactable layer
                if (!levelList[level.GetSiblingIndex()].isLocked)
                    empty.gameObject.AddComponent(typeof(MeshCollider));
                else
                    canPlan = false;
                empty.gameObject.AddComponent(typeof(Outline));
                Level lv = empty.gameObject.AddComponent(typeof(Level)) as Level;
                levelList[level.GetSiblingIndex()].script = lv;
                lv.levelInfo = levelList[level.GetSiblingIndex()];
                lv.contract = this;
                lv.details = details;
                lv.enabled = false;
                Interactable inter = empty.gameObject.AddComponent(typeof(Interactable)) as Interactable;
                inter.scriptToStart = lv;
                empty.transform.SetParent(level);
            }
        }
        enabled = false;
        yield return 0;
        
        // Getting all colliders
        Queue<Transform> childList = new Queue<Transform>();
        childList.Enqueue(hologram);
        while (childList.Count > 0)
        {
            Transform currentChild = childList.Dequeue();
            foreach (Transform child in currentChild)
                childList.Enqueue(child);
            if (currentChild.TryGetComponent(out Collider col))
                collidersList.Add(col);
        }
        difficulty = hologram.GetComponentInChildren<ChoiceManager>();
        levelLayout.SetActive(false);
        hologram.localScale = new Vector3(1f, 0f, 1f);
    }

    public void BuildPlanning()
    {
        if (canPlan)
        {
            planningManager.coroutineRunning = false;
            planningManager.levelToDisplay = selectedLevel;
            planningManager.currentContract = this;
        }
        else
            planningManager.ResetLayout();
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (selectedLevel.Count != 0 && selectedLevel.Count == 1)
        {
            levelName.text = selectedLevel[0].levelName;
            levelScore.text = "Score: " + selectedLevel[0].highscore;
        }
        if (graphics.enabled)
        {
            selectedLevel = levelList;
            BuildPlanning();
            foreach (Contract contract in transform.parent.GetComponentsInChildren<Contract>())
                contract.closing = true;
            closing = false;
            levelLayout.SetActive(true);
            foreach (Collider collider in collidersList)
                collider.enabled = true;
            graphics.enabled = false;
        }
        
        /*if (Vector3.Distance(Player.m.transform.position, transform.position) > 10f)
        {
            closing = true;
        }*/

        if (closing)
        {
            if (destination == Vector3.one)
                foreach (Collider collider in collidersList)
                    collider.enabled = false;
            destination = new Vector3(1f, 0f, 1f);
            if (Vector3.Distance(hologram.localScale, destination) < 0.001f)
            {
                hologram.localScale = destination;
                graphics.enabled = true;
                if (selectedLevel.Count != 0 && selectedLevel.Count == 1)
                {
                    selectedLevel[0].script.details.localScale = new Vector3(1f, 0f, 1f);
                    selectedLevel[0].script.isClosed = false;
                    selectedLevel[0].script.deselecting = false;
                    selectedLevel[0].script.enabled = false;
                    selectedLevel = null;
                }
                levelLayout.SetActive(false);
                enabled = false;
            }
        }
        else
        {
            destination = Vector3.one;
        }

        hologram.localScale = Vector3.Lerp(hologram.localScale, destination, animationSpeed);
    }
}
