using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImiBagPulaInUnity : MonoBehaviour
{
    /** Rewind script **/
    public GameObject level;
    private GameObject save = null;
    private void Awake()
    {
        level = GameObject.Find("Level");
        if (level == null)
            Debug.Log("set up an empty called \"Level\" and put everything in the scene besides PlayerPackage and RewindManager in it.");
    }
    void Update()
    {
        // Save: Instantiate a clone of the level, disable it and save it.
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("saved");
            save = Instantiate(level);
            save.SetActive(false);
        }
        // Rewind: Delete current level, enable the save and renew the save.
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (save == null)
                Debug.Log("no save");
            else
            {
                Destroy(level);
                level = save;
                level.SetActive(true);

                save = Instantiate(level);
                save.SetActive(false);
            }
        } 
    }
}
