using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class Rewind : MonoBehaviour
{


    //Ver 2

    [HideInInspector]
    public List<GameObject> TimeMoments= new List<GameObject>();
    public int CurrentMomentInTime = 0;

    [HideInInspector]
    public List<GameObject> rootObjects;

    private void Start()
    {
        CreateNewMomentInTime();
        Invoke (nameof(CreateNewMomentInTime),0.1f);
    }

    void Update()
    {
        // Save: Instantiate a clone of the level, disable it and save it.
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("saved");
            CreateNewMomentInTime();
        }
        // Rewind: Delete current level, enable the save and renew the save.
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GoToPreviousMomentInTime();
        }
    }

    
    private void CreateNewMomentInTime()
    {
        GameObject newMoment; 

        if (TimeMoments.Count - 1 < 0)
            newMoment = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        else
            newMoment = Instantiate(TimeMoments[TimeMoments.Count - 1]);

        TimeMoments.Add(newMoment);
        newMoment.name = "RewindPoint " + CurrentMomentInTime;

        // get root objects in scene
        rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        foreach (GameObject obj in rootObjects)
        {
            if (LayerMask.LayerToName(obj.layer) == "Player")
                continue;

            if (obj == this)
                continue;

            if (TimeMoments.Contains(obj))
                continue;

            if (CurrentMomentInTime > 0)
            {
                GameObject copy = Instantiate(obj, obj.transform.position, obj.transform.rotation);
                copy.transform.parent = TimeMoments[CurrentMomentInTime - 1].transform;
            }

            obj.transform.parent = newMoment.transform;
        }

        if (CurrentMomentInTime > 0)
            TimeMoments[CurrentMomentInTime - 1].SetActive(false);

        CurrentMomentInTime++;
    }
    
    public void GoToPreviousMomentInTime()
    {
        // Delete current moment

        // get root objects in scene
        //rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; i++)
        {
            //GameObject obj = rootObjects[i];

            if (LayerMask.LayerToName(rootObjects[i].layer) == "Player")
                continue;

            if (rootObjects[i] == this)
                continue;

            if (TimeMoments.Contains(rootObjects[i]))
                continue;

            Destroy(rootObjects[i]);
            //rootObjects.RemoveAt(rootObjects.Count - 1);
            
        }

        Destroy(TimeMoments[CurrentMomentInTime - 1]);
        TimeMoments.RemoveAt(CurrentMomentInTime - 1);

        CurrentMomentInTime--;

        if (CurrentMomentInTime > 1)
            TimeMoments[CurrentMomentInTime - 1].SetActive(true);
        else
        {
            GameObject newMoment = Instantiate(TimeMoments[0]);
            TimeMoments.Add(newMoment);
            newMoment.SetActive(true);
            newMoment.name = "RewindPoint " + CurrentMomentInTime ;

            CurrentMomentInTime++;
        }

    }



    // Ver 1
    /*
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
    */
}
