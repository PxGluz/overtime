using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ListDisplay : MonoBehaviour
{
    [Header("Static References")]
    [HideInInspector]public GameObject buttonsEmpty, modelsEmpty, textsEmpty;
    [HideInInspector]public GameObject selectButtonPrefab, textPrefab;
    [HideInInspector]public Material hologramMaterial;
    [HideInInspector]public bool forceClose;

    public float rotationSpeed;
    public float closeSpeed;

    [HideInInspector]public int currentIndex = -1;
    [HideInInspector]public LoadoutTab openTab = null;
    [HideInInspector]public PlantingSpot planningTab = null;
    private Vector3 destination;

    public void ForceUpdateChoice()
    {
        if (currentIndex != -1)
        {
            if (openTab && openTab.loadoutChoice[currentIndex].isUnlocked && buttonsEmpty.GetComponent<ChoiceManager>().GetChoice() == currentIndex)
                openTab.selectedChoice = currentIndex;

            if (planningTab && planningTab.loadoutChoice[currentIndex].isUnlocked && buttonsEmpty.GetComponent<ChoiceManager>().GetChoice() == currentIndex)
                planningTab.UpdateChoice(currentIndex);
        }
    }
    
    public void ResetList(List<LoadoutTab.LoadoutChoice> choicesList, LoadoutTab lTab=null, PlantingSpot pSpot=null)
    {
        if (currentIndex != -1)
            ForceUpdateChoice();
        openTab = lTab;
        planningTab = pSpot;
        currentIndex = -1;
        transform.localScale = Vector3.right + Vector3.up;
        foreach (Transform child in buttonsEmpty.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in modelsEmpty.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in textsEmpty.transform)
        {
            Destroy(child.gameObject);
        }
        /*GameObject none = Instantiate(new GameObject(), modelsEmpty.transform.position, modelsEmpty.transform.rotation);
        none.transform.SetParent(modelsEmpty.transform);
        none = Instantiate(selectButtonPrefab, buttonsEmpty.transform);
        none.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
        none = Instantiate(textPrefab, textsEmpty.transform);
        none.GetComponent<TextMeshProUGUI>().text = "None";*/
        foreach (LoadoutTab.LoadoutChoice choice in choicesList)
        {
            GameObject currentObject = Instantiate(choice.model, modelsEmpty.transform);
            Queue<Transform> modelQueue = new Queue<Transform>();
            modelQueue.Enqueue(currentObject.transform);
            while (modelQueue.Count > 0)
            {
                Transform currentChild = modelQueue.Dequeue();
                foreach (Transform child in currentChild)
                    modelQueue.Enqueue(child);
                if (currentChild.TryGetComponent(out MeshRenderer meshRend))
                {
                    Material[] tempMats = new Material[meshRend.materials.Length];
                    for (int i = 0; i < tempMats.Length; i++)
                        tempMats[i] = hologramMaterial;
                    meshRend.materials = tempMats;
                }
            }

            GameObject currentButton = Instantiate(selectButtonPrefab, buttonsEmpty.transform);
            if (!choice.isUnlocked)
            {
                currentButton.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Locked";
                currentButton.GetComponent<Collider>().enabled = false;
            }

            GameObject currentText = Instantiate(textPrefab, textsEmpty.transform);
            currentText.GetComponent<TextMeshProUGUI>().text = choice.choiceName;
        }

        StartCoroutine(WaitForAFrameActions());
    }

    IEnumerator WaitForAFrameActions()
    {
        yield return 0;
        int margin = openTab ? openTab.selectedChoice : planningTab.selectedChoice;
        for(int i = 0; i <= margin; i++)
            MoveRight();
        buttonsEmpty.GetComponent<ChoiceManager>().UpdateChoice();
        buttonsEmpty.GetComponent<ChoiceManager>().ChangeChoice(margin);
    }
    
    public void MoveLeft()
    {
        if (currentIndex == 0)
            currentIndex = buttonsEmpty.transform.childCount - 1;
        else
            currentIndex--;
        foreach (Transform child in buttonsEmpty.transform)
        {
            if (child.GetSiblingIndex() != currentIndex)
                child.gameObject.SetActive(false);
            else
                child.gameObject.SetActive(true);
        }
        foreach (Transform child in modelsEmpty.transform)
        {
            if (child.GetSiblingIndex() != currentIndex)
                child.gameObject.SetActive(false);
            else
                child.gameObject.SetActive(true);
        }
        foreach (Transform child in textsEmpty.transform)
        {
            if (child.GetSiblingIndex() != currentIndex)
                child.gameObject.SetActive(false);
            else
                child.gameObject.SetActive(true);
        }
    }
    
    
    public void MoveRight()
    {
        if (currentIndex == buttonsEmpty.transform.childCount - 1)
            currentIndex = 0;
        else
            currentIndex++;
        foreach (Transform child in buttonsEmpty.transform)
        {
            if (child.GetSiblingIndex() != currentIndex)
                child.gameObject.SetActive(false);
            else
            {
                child.gameObject.SetActive(true);
            }
        }
        foreach (Transform child in modelsEmpty.transform)
        {
            if (child.GetSiblingIndex() != currentIndex)
                child.gameObject.SetActive(false);
            else
            {
                child.gameObject.SetActive(true);
            }
        }
        foreach (Transform child in textsEmpty.transform)
        {
            if (child.GetSiblingIndex() != currentIndex)
                child.gameObject.SetActive(false);
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, destination, closeSpeed);
        if (!forceClose && Vector3.Angle(transform.right, transform.position - new Vector3(Player.m.transform.position.x, transform.position.y, Player.m.transform.position.z)) > 90 && currentIndex != -1 && Vector3.Distance(transform.position, Player.m.transform.position) < 5)
        {
            transform.localScale = Vector3.right + Vector3.up + Vector3.forward * transform.localScale.z;
            destination = Vector3.one;
        }
        else
        {
            if ((destination == Vector3.right + Vector3.up || destination == Vector3.zero) && Vector3.Distance(destination, transform.localScale) < 0.01f)
            {
                forceClose = false;
                destination = Vector3.zero;
                transform.localScale = Vector3.zero;
            }
            else
            {
                if ((openTab || planningTab) && currentIndex != -1 &&
                    currentIndex == buttonsEmpty.GetComponent<ChoiceManager>().GetChoice())
                {
                    ForceUpdateChoice();
                }
                currentIndex = -1;
                destination = Vector3.right + Vector3.up;
            }
        }
        foreach (Transform child in modelsEmpty.transform)
            child.Rotate(Vector3.up * rotationSpeed);
    }
}
