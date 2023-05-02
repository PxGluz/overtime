using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [System.Serializable]
    public class LevelInfo
    {
        [HideInInspector]
        public Level script;

        public bool isLocked;
        public string levelName;
        public float highscore;
        public string levelScene;
        public GameObject levelLayout;
    }

    [Header("Static References")]
    [HideInInspector] public Transform details;
    public Contract contract;
    public Material normalMat, highlightedMat;

    [HideInInspector] public GameObject layoutParent;
    [HideInInspector] public bool isClosed, deselecting;
    [HideInInspector] public LevelInfo levelInfo;

    private Vector3 destination;

    void Start()
    {
        levelInfo.script = this;
        destination = new Vector3(1f, 0f, 1f);
    }

    public void ChangeHighlight(Material newMat)
    {
        foreach (Transform piece in layoutParent.transform)
        {
            if (piece.TryGetComponent(out MeshRenderer meshRend))
            {
                Material[] tempMats = new Material[meshRend.materials.Length];
                for (int i = 0; i < tempMats.Length; i++)
                    tempMats[i] = newMat;
                meshRend.materials = tempMats;
            }
        }
    }

    void Update()
    {
        details.localScale = Vector3.Lerp(details.localScale, destination, contract.animationSpeed * 2);
        if (!isClosed)
        {
            destination = new Vector3(1f, 0f, 1f);
            if (contract.selectedLevel.Count > 0)
                foreach (LevelInfo level in contract.selectedLevel)
                {
                    if (level.script != this)
                    {
                        level.script.isClosed = false;
                        level.script.deselecting = false;
                        level.script.enabled = false;
                    }
                }
            foreach (Transform child in details)
                if (child.TryGetComponent(out Collider col))
                    col.enabled = false;
            if (Vector3.Distance(details.localScale, destination) < 0.001f)
            {
                isClosed = true;
                if (!deselecting)
                {
                    List<LevelInfo> tempList = new List<LevelInfo>();
                    tempList.Add(levelInfo);
                    contract.selectedLevel = tempList;
                }
                else
                {
                    contract.selectedLevel = contract.levelList;
                    details.localScale = new Vector3(1f, 0f, 1f);
                    deselecting = false;
                    isClosed = false;
                    enabled = false;
                }
                contract.BuildPlanning();
            }
        }
        else
        {
            if (deselecting)
                isClosed = false;
            else
            {
                destination = Vector3.one;
                if (Vector3.Distance(details.localScale, destination) < 0.001f)
                {
                    foreach (Transform child in details)
                        if (child.TryGetComponent(out Collider col))
                            col.enabled = true;
                    enabled = false;
                    deselecting = true;

                }
            }
        }
    }
}
