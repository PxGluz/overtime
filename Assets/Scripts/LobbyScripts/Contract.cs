using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Contract : MonoBehaviour
{
    private bool closing;
    private Vector3 destination;
    private List<Collider> collidersList = new List<Collider>();
    
    [Header("Static References")]
    [HideInInspector]public Transform hologram, details;
    [HideInInspector]public Collider graphics;
    [HideInInspector]public TextMeshProUGUI levelName, levelScore;
    [HideInInspector]public GameObject levelLayout, layoutCenter;
    [HideInInspector]public Material layoutMaterial;

    [Header("Animation Related")]
    public float animationSpeed;

    [Header("Levels")] 
    public List<Level.LevelInfo> levelList;

    [Header("Important for loading correct level")]
    [HideInInspector] public ChoiceManager difficulty; // Use GetChoice() to get the selected difficulty
    [HideInInspector] public Level.LevelInfo selectedLevel; // Contains levelScene
    // TODO: Add loadout and planning references for loading purposes

    private IEnumerator Start()
    {
        selectedLevel = null;
        
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
                empty.gameObject.AddComponent(typeof(MeshCollider));
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

    // Update is called once per frame
    void Update()
    {
        if (selectedLevel != null)
        {
            levelName.text = selectedLevel.levelName;
            levelScore.text = "Score: " + selectedLevel.highscore;
        }
        if (graphics.enabled)
        {
            foreach (Contract contract in transform.parent.GetComponentsInChildren<Contract>())
                contract.closing = true;
            closing = false;
            levelLayout.SetActive(true);
            foreach (Collider collider in collidersList)
                collider.enabled = true;
            graphics.enabled = false;
        }
        
        if (Vector3.Distance(Player.m.transform.position, transform.position) > 10f)
        {
            closing = true;
        }

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
                if (selectedLevel != null)
                {
                    selectedLevel.script.details.localScale = new Vector3(1f, 0f, 1f);
                    selectedLevel.script.isClosed = false;
                    selectedLevel.script.enabled = false;
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
