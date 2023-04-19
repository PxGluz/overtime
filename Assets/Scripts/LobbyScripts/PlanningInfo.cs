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
    public Queue<Level.LevelInfo> remainingLevels = new Queue<Level.LevelInfo>();

    public static PlanningInfo instance;
    
    private void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(OnLevelLoadCoroutine());
    }

    IEnumerator OnLevelLoadCoroutine()
    {
        Player.m.scoringSystem.combo.gameObject.transform.parent.gameObject.SetActive(false);
        yield return 0;
        if (SceneManager.GetActiveScene().buildIndex == 0)
            yield break;     
        Player.m.scoringSystem.combo.gameObject.transform.parent.gameObject.SetActive(true);
        Player.m.scoringSystem.enabled = true;
        //TODO: set difficulty of level
        Player.m.weaponManager.ChangeWeapon(Player.m.weaponManager.WeaponsList[loadoutChoices[0]].name, quantity:Player.m.weaponManager.WeaponsList[loadoutChoices[0]].gunMagazineSize);
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
                    GameObject plantedWeapon = Instantiate(Player.m.weaponManager.WeaponsList[plantingSpot[1]].WeaponPrefab,
                        child.position, child.rotation);
                    plantedWeapon.GetComponent<Interactable>().quantity = Player.m.weaponManager.WeaponsList[plantingSpot[1]].gunMagazineSize;
                }
            if (plantingSpot[0] != -1)
                plantingSpot[0] -= pSpot.transform.childCount;
        }
    }

    public void UpdatePlanning(List<int> plant,List<int> load, int diff, Queue<Level.LevelInfo> remLevels)
    {
        plantingSpot = plant;
        loadoutChoices = load;
        difficulty = diff;
        remainingLevels = remLevels;
    }

    public void KeepLastWeapon()
    {
        loadoutChoices[0] = Array.IndexOf(Player.m.weaponManager.WeaponsList, Player.m.weaponManager.currentWeapon);
    }

    public string GetNextScene()
    {
        return remainingLevels.Count > 0 ? remainingLevels.Dequeue().levelScene : "MainMenuLobby";
    }
    
    void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnLevelLoad;
        DontDestroyOnLoad(gameObject);
    }
}
