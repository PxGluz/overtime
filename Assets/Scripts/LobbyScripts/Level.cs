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


    void Update()
    {
        details.localScale = Vector3.Lerp(details.localScale, destination, contract.animationSpeed * 2);
        if (!isClosed)
        {
            destination = new Vector3(1f, 0f, 1f);
            if (contract.selectedLevel != null && contract.selectedLevel.script != null)
            {
                contract.selectedLevel.script.isClosed = false;
                contract.selectedLevel.script.enabled = false;
            }
            foreach (Transform child in details)
                if (child.TryGetComponent(out Collider col))
                    col.enabled = false;
            if (Vector3.Distance(details.localScale, destination) < 0.001f)
            {
                isClosed = true;
                contract.selectedLevel = levelInfo;
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
