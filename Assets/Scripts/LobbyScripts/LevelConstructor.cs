using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelConstructor : MonoBehaviour
{
    public static void ConstructLevel(GameObject startingPoint, float sizeX, float sizeZ, Material layoutMaterial, List<Level.LevelInfo> levelList = null, Level.LevelInfo level = null, bool collidersOff = true)
    {
        if (levelList == null)
        {
            if (level == null)
            {
                Debug.LogWarning("ConstructLevel was called but there are no levels provided");
                return;
            }
            levelList = new List<Level.LevelInfo>();
            levelList.Add(level);
        }
        GameObject minZ = startingPoint, maxZ = startingPoint, minX = startingPoint, maxX = startingPoint;
        
        GameObject prevLevel = startingPoint;
        foreach (Level.LevelInfo currentLevel in levelList)
        {
            GameObject appearingLevel = Instantiate(currentLevel.levelLayout, prevLevel.transform.position, prevLevel.transform.rotation, startingPoint.transform);
            Queue<Transform> childrenQueue = new Queue<Transform>();
            childrenQueue.Enqueue(appearingLevel.transform);
            while (childrenQueue.Count > 0)
            {
                Transform currentChild = childrenQueue.Dequeue();
                foreach (Transform child in currentChild)
                    childrenQueue.Enqueue(child);
                if (currentChild.name.Equals("Exit"))
                    prevLevel = currentChild.gameObject;
                else if (!currentChild.name.Contains("Plane") && !currentChild.name.Contains("PlantingSpot"))
                {
                    if (currentChild.position.y + currentChild.lossyScale.x / 2 > maxX.transform.position.y) maxX = currentChild.gameObject;
                    if (currentChild.position.y - currentChild.lossyScale.x / 2 < minX.transform.position.y) minX = currentChild.gameObject;
                    if (currentChild.position.z + currentChild.lossyScale.z / 2 > maxZ.transform.position.z) maxZ = currentChild.gameObject;
                    if (currentChild.position.z - currentChild.lossyScale.z / 2 < minZ.transform.position.z) minZ = currentChild.gameObject;
                    if (currentChild.TryGetComponent(out Renderer rend))
                        rend.material = layoutMaterial;
                    if (collidersOff && currentChild.TryGetComponent(out Collider col))
                        Destroy(col);
                }
            }
        }
        float dimX = Mathf.Abs((minX.transform.position.y - minX.transform.lossyScale.x) - (maxX.transform.position.y + maxX.transform.lossyScale.x)), 
              dimZ = Mathf.Abs((minZ.transform.position.z - minZ.transform.lossyScale.z) - (maxZ.transform.position.z + maxZ.transform.lossyScale.z));
        float resizeCoef = dimX > dimZ ? sizeX / dimX : sizeZ / dimZ;
        
        
        
        startingPoint.transform.localScale = new Vector3(resizeCoef, startingPoint.transform.localScale.y ,resizeCoef);
        Vector3 differential = 
        new Vector3(startingPoint.transform.position.x, startingPoint.transform.position.y + sizeX / 2, startingPoint.transform.position.z - sizeZ / 2) - 
        new Vector3(startingPoint.transform.position.x, ((maxX.transform.position + minX.transform.position)/2).y, ((maxZ.transform.position + minZ.transform.position)/2).z);
        startingPoint.transform.position += differential;
    }

    public static void InsertPlanning(GameObject levelLayout, Transform choiceRoot, GameObject planningPrefab)
    {
        Queue<Transform> childrenQueue = new Queue<Transform>();
        childrenQueue.Enqueue(levelLayout.transform);
        while (childrenQueue.Count > 0)
        {
            Transform currentChild = childrenQueue.Dequeue();
            foreach (Transform child in currentChild)
                childrenQueue.Enqueue(child);
            if (currentChild.name.Contains("PlantingSpot"))
                Instantiate(planningPrefab, currentChild.position, choiceRoot.rotation, choiceRoot);
        }
        /*choiceManager.UpdateChoice();
        choiceManager.ChangeChoice(0);*/
    }
}
