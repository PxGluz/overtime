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
    [HideInInspector]public Transform hologram;
    [HideInInspector]public Collider graphics;
    [HideInInspector]public TextMeshProUGUI levelName, levelScore;

    [Header("Animation Related")]
    public float animationSpeed;

    [Header("Important for loading correct level")]
    [HideInInspector] public ChoiceManager difficulty; // Use GetChoice() to get the selected difficulty
    [HideInInspector] public Level.LevelInfo selectedLevel; // Contains levelScene
    // TODO: Add loadout and planning references for loading purposes

    private void Start()
    {
        selectedLevel = null;
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
