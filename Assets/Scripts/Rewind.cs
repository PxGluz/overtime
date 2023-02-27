using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Rewind : MonoBehaviour
{


    //Ver 2

    [HideInInspector]
    public List<GameObject> TimeMoments = new List<GameObject>();
    public int CurrentMomentInTime = 0;

    [HideInInspector]
    public List<GameObject> rootObjects;


    public List<PlayerMoment> PlayerMoments = new List<PlayerMoment>();

    [System.Serializable]
    public class PlayerMoment{
        public Vector3 position;
        public float[] cameraXandY;

        public float health;
        public string weaponName;
        public int bulletsLeft;
    }

    private void CreatePlayerMoment()
    {
        PlayerMoment playerMoment = new PlayerMoment();

        playerMoment.position = Player.m.transform.position;
        playerMoment.cameraXandY = new float[]{ Player.m.playerCam.xRotation, Player.m.playerCam.yRotation };

        playerMoment.health = Player.m.currentHealth;
        playerMoment.weaponName = Player.m.weaponManager.currentWeapon.name;

        if (Player.m.weaponManager.GetWeaponType(playerMoment.weaponName) == "shoot")
        {
            playerMoment.bulletsLeft = Player.m.playerShooting.bulletsleft;
        }

        PlayerMoments.Add(playerMoment);
    }

    private void SetPlayerToMoment()
    {
        Player.m.SetPlayerHealth(PlayerMoments[CurrentMomentInTime - 1].health);

        Player.m.playerRigidBody.position = PlayerMoments[CurrentMomentInTime - 1].position;
        //Player.m.transform.Translate(PlayerMoments[CurrentMomentInTime - 1].position - Player.m.transform.position);
        //Player.m.transform.position = PlayerMoments[CurrentMomentInTime - 1].position;
        Player.m.playerCam.xRotation = PlayerMoments[CurrentMomentInTime - 1].cameraXandY[0];
        Player.m.playerCam.yRotation = PlayerMoments[CurrentMomentInTime - 1].cameraXandY[1];

        /*
        Player.m.weaponManager.currentWeapon.name = "";
        Player.m.weaponManager.ChangeWeapon(PlayerMoments[CurrentMomentInTime - 1].weaponName);

        if (Player.m.weaponManager.GetWeaponType(PlayerMoments[CurrentMomentInTime - 1].weaponName) == "shoot")
        {
            Player.m.playerShooting.bulletsleft = PlayerMoments[CurrentMomentInTime - 1].bulletsLeft;
        }
        */

    }



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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            GoToPreviousMomentInTime(CurrentMomentInTime - 1);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            GoToPreviousMomentInTime(CurrentMomentInTime);
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

        CreatePlayerMoment();
    }
    
    public void GoToPreviousMomentInTime(int MomentToGoTo)
    {
        // Delete current moment

        // get root objects in scene
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; i++)
        {
            if (LayerMask.LayerToName(rootObjects[i].layer) == "Player")
                continue;

            if (rootObjects[i] == this)
                continue;

            if (TimeMoments.Contains(rootObjects[i]))
                continue;

            Destroy(rootObjects[i]);
            
        }

        Destroy(TimeMoments[CurrentMomentInTime - 1]);
        TimeMoments.RemoveAt(CurrentMomentInTime - 1);

        CurrentMomentInTime--;

        if (CurrentMomentInTime > 1 && MomentToGoTo == CurrentMomentInTime - 1 + 1)
        {
            // This returns to the previos moment in time
            TimeMoments[CurrentMomentInTime - 1].SetActive(true);
            PlayerMoments.RemoveAt(CurrentMomentInTime);
        }
        else
        {
            // This resets the current moment in time
            GameObject newMoment = Instantiate(TimeMoments[CurrentMomentInTime - 1]);
            TimeMoments.Add(newMoment);
            newMoment.SetActive(true);
            newMoment.name = "RewindPoint " + CurrentMomentInTime;

            CurrentMomentInTime++;
        }

        SetPlayerToMoment();

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
