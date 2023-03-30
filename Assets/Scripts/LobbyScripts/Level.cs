using System.Collections;
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
    [HideInInspector]public Transform details;
    public Contract contract;

    
    [HideInInspector]public bool isClosed;
    [HideInInspector]public LevelInfo levelInfo;

    private Vector3 destination;
    
    void Start()
    {
        levelInfo.script = this;
        destination = new Vector3(1f, 0f, 1f);
    }

    // TODO: Add option to deselect
    void Update()
    {
        details.localScale = Vector3.Lerp(details.localScale, destination, contract.animationSpeed * 2);
        if (!isClosed)
        {
            destination = new Vector3(1f, 0f, 1f);
            if (contract.selectedLevel.Count != 0 && contract.selectedLevel.Count == 1 && contract.selectedLevel[0].script != null)
            {
                contract.selectedLevel[0].script.isClosed = false;
                contract.selectedLevel[0].script.enabled = false;
            }
            foreach (Transform child in details)
                if (child.TryGetComponent(out Collider col))
                    col.enabled = false;
            if (Vector3.Distance(details.localScale, destination) < 0.001f)
            {
                isClosed = true;
                List<LevelInfo> tempList = new List<LevelInfo>();
                tempList.Add(levelInfo);
                contract.selectedLevel = tempList;
                contract.BuildPlanning();
            }
        }
        else
        {
            destination = Vector3.one;
            if (Vector3.Distance(details.localScale, destination) < 0.001f)
            {
                foreach (Transform child in details)
                    if (child.TryGetComponent(out Collider col))
                        col.enabled = true;
            }
        }
    }
}
