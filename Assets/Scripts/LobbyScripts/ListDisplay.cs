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

    public float rotationSpeed;

    [HideInInspector]public static int currentIndex = -1;
    [HideInInspector]public static LoadoutTab openTab;

    public static void ForceUpdateChoice()
    {
        openTab.selectedChoice = currentIndex;
    }
    
    public void ResetList(List<LoadoutTab.LoadoutChoice> choicesList, LoadoutTab lTab)
    {
        if (currentIndex != -1)
            ForceUpdateChoice();
        openTab = lTab;
        currentIndex = -1;
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
        GameObject none = Instantiate(new GameObject(), modelsEmpty.transform.position, modelsEmpty.transform.rotation);
        none.transform.SetParent(modelsEmpty.transform);
        none = Instantiate(selectButtonPrefab, buttonsEmpty.transform);
        none.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
        none = Instantiate(textPrefab, textsEmpty.transform);
        none.GetComponent<TextMeshProUGUI>().text = "None";
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
                    meshRend.material = hologramMaterial;
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
        MoveRight();
        buttonsEmpty.GetComponent<ChoiceManager>().UpdateChoice();
        buttonsEmpty.GetComponent<ChoiceManager>().ChangeChoice(openTab.selectedChoice);
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
        foreach (Transform child in modelsEmpty.transform)
            child.Rotate(Vector3.up * rotationSpeed);
    }
}
