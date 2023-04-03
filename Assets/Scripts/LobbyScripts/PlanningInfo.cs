using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanningInfo : MonoBehaviour
{
    public List<int> plantingSpot = new List<int>();
    public List<int> loadoutChoices = new List<int>();
    public int difficulty;

    private void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(OnLevelLoadCoroutine());
    }

    IEnumerator OnLevelLoadCoroutine()
    {
        Player.m.scoringSystem.combo.gameObject.transform.parent.gameObject.SetActive(false);
        yield return 0;
        if (plantingSpot.Count == 0)
            yield break;     
        Player.m.scoringSystem.combo.gameObject.transform.parent.gameObject.SetActive(true);
        Player.m.scoringSystem.enabled = true;
        //TODO: set difficulty of level
        Player.m.weaponManager.ChangeWeapon(Player.m.weaponManager.WeaponsList[loadoutChoices[0]].name);
        if (plantingSpot[0] != -1)
        {
            GameObject pSpot = GameObject.Find("PSpots");
            if (pSpot == null)
            {
                Debug.LogError("No object called pSpots in this level, planting spots here will be skipped");
                yield break;
            }
            foreach (Transform child in pSpot.transform)
                if (child.GetSiblingIndex() == plantingSpot[0])
                {
                    plantingSpot[0] = -1;
                    Instantiate(Player.m.weaponManager.WeaponsList[plantingSpot[1]].WeaponPrefab,
                        child.position, child.rotation);
                }
            if (plantingSpot[0] != -1)
                plantingSpot[0] -= pSpot.transform.childCount;
        }
    }

    public void UpdatePlanning(List<int> plant,List<int> load, int diff)
    {
        plantingSpot = plant;
        loadoutChoices = load;
        difficulty = diff;
    }

    public void KeepLastWeapon()
    {
        loadoutChoices[0] = Array.IndexOf(Player.m.weaponManager.WeaponsList, Player.m.weaponManager.currentWeapon);
    }
    
    void Awake()
    {
        SceneManager.sceneLoaded += OnLevelLoad;
        DontDestroyOnLoad(gameObject);
    }
}
