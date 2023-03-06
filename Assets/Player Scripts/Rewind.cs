using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Rewind : MonoBehaviour
{
    
    // Ver 3
    [Header("Visual part of the script: ")]
    public GameObject playerGhost;
    private Color ghostColor;

    [Header("Logic part of the script: ")]
    [HideInInspector]
    public List<GameObject> TimeMoments = new List<GameObject>();
    public List<PlayerMoment> PlayerMoments = new List<PlayerMoment>();

    public GameObject currentMomentRoot;
    public int CurrentMomentInTime = 0;

    [HideInInspector]
    public List<GameObject> rootObjects;

    [System.Serializable]
    public class PlayerMoment
    {
        public Vector3 position;
        public float[] cameraXandY;
        public float health;
        public string weaponName;
        public int bulletsLeft;
    }

    public List<SkinnedMeshRenderer> GhostList = new List<SkinnedMeshRenderer>();
    private GameObject GhostRoot;

    private void Start()
    {
        currentMomentRoot = new GameObject();
        currentMomentRoot.name = "Current Moment";

        GhostRoot = new GameObject();
        GhostRoot.name = "GhostRoot";

        Invoke(nameof(CreateNewMomentInTime), 1f);
    }

    private void CreateNewMomentInTime()
    {
        SpawnPlayerGhost();

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

            if (obj.name == "Current Moment")
                continue;

            if (obj.name == "GhostRoot")
                continue;

            obj.transform.parent = currentMomentRoot.transform;
        }

        GameObject newMoment = new GameObject();

        Instantiate(currentMomentRoot, Vector3.zero, Quaternion.identity).transform.parent = newMoment.transform;

        newMoment.name = "RewindPoint " + CurrentMomentInTime;
        newMoment.SetActive(false);
        TimeMoments.Add(newMoment);
        
        CurrentMomentInTime++;


        CreatePlayerMoment();
    }

    private void SpawnPlayerGhost()
    {
        GameObject newGhost = Instantiate(playerGhost, Player.m.playerMovement.orientation.transform.position, Player.m.playerMovement.orientation.transform.rotation);
        newGhost.transform.parent = GhostRoot.transform;
        GhostList.Add(newGhost.GetComponentInChildren<SkinnedMeshRenderer>());

        if (GhostList.Count == 1)
            ghostColor = newGhost.GetComponentInChildren<SkinnedMeshRenderer>().material.color;

        for (int i = 0; i < GhostList.Count; i++)
        {
            GhostList[i].material.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, (float)i / (float)GhostList.Count );//new Color(materialColor.r, materialColor.g, materialColor.b, i/GhostList.Count)
            print(i + "/" + GhostList.Count + "=" + (float)i/ (float)GhostList.Count);
        }
        
    }

    private void RemoveLastPlayerGhost()
    {
        Destroy(GhostList[GhostList.Count - 1]);
        GhostList.RemoveAt(GhostList.Count - 1);

        for (int i = 0; i < GhostList.Count; i++)
        {
            GhostList[i].material.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, (float)i / (float)GhostList.Count);//new Color(materialColor.r, materialColor.g, materialColor.b, i/GhostList.Count)
            print(i + "/" + GhostList.Count + "=" + (float)i / (float)GhostList.Count);
        }
    }

    /// <summary>
    /// if GoToPreviousMoment is 'false', the scene resets to the last moment in time |
    /// if GoToPreviousMoment is 'true', the scene resets to the moment before the last moment in time 
    /// </summary>
    public void GoToPreviousMomentInTime(bool GoToPreviousMoment)
    {
        // Delete the current moment in time

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
            if (rootObjects[i].name == "GhostRoot")
                continue;

            Destroy(rootObjects[i]);
        }

        if (GoToPreviousMoment)
        {
            if (CurrentMomentInTime > 1)
            {

                Destroy(TimeMoments[CurrentMomentInTime - 1]);
                TimeMoments.RemoveAt(CurrentMomentInTime - 1);
                PlayerMoments.RemoveAt(CurrentMomentInTime - 1);
                RemoveLastPlayerGhost();

                CurrentMomentInTime--;

                currentMomentRoot = Instantiate(TimeMoments[CurrentMomentInTime - 1]);
            }
            else
            {
                currentMomentRoot = Instantiate(TimeMoments[CurrentMomentInTime - 1]);
            }
        }
        else
        {
            currentMomentRoot = Instantiate(TimeMoments[CurrentMomentInTime - 1]);
        }
        
        currentMomentRoot.name = "Current Moment";
        currentMomentRoot.SetActive(true);

        SetPlayerToMoment();

    }


    private void CreatePlayerMoment()
    {
        PlayerMoment playerMoment = new PlayerMoment();

        playerMoment.position = Player.m.transform.position;
        playerMoment.cameraXandY = new float[] { Player.m.playerCam.xRotation, Player.m.playerCam.yRotation };

        playerMoment.health = Player.m.currentHealth;
        playerMoment.weaponName = Player.m.weaponManager.currentWeapon.name;

        if (Player.m.weaponManager.GetWeaponType(playerMoment.weaponName) == "ranged")
        {
            playerMoment.bulletsLeft = Player.m.playerShooting.bulletsleft;
        }

        PlayerMoments.Add(playerMoment);
    }

    private void SetPlayerToMoment()
    {
        Player.m.SetPlayerHealth(PlayerMoments[CurrentMomentInTime - 1].health);

        Player.m.playerRigidBody.position = PlayerMoments[CurrentMomentInTime - 1].position;
        Player.m.playerCam.xRotation = PlayerMoments[CurrentMomentInTime - 1].cameraXandY[0];
        Player.m.playerCam.yRotation = PlayerMoments[CurrentMomentInTime - 1].cameraXandY[1];

        if (Player.m.weaponManager.GetWeaponType(PlayerMoments[CurrentMomentInTime - 1].weaponName) == "ranged")
        {
            Player.m.weaponManager.ChangeWeapon(PlayerMoments[CurrentMomentInTime - 1].weaponName, PlayerMoments[CurrentMomentInTime - 1].bulletsLeft, false);
        }
        else
        {
            Player.m.weaponManager.ChangeWeapon(PlayerMoments[CurrentMomentInTime - 1].weaponName, 1, false);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("saved");
            CreateNewMomentInTime();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            GoToPreviousMomentInTime(true);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            GoToPreviousMomentInTime(false);
        }
        
    }


    //Ver 2

    /*
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



        
        //Player.m.weaponManager.currentWeapon.name = "";
        //Player.m.weaponManager.ChangeWeapon(PlayerMoments[CurrentMomentInTime - 1].weaponName);

        
        if (Player.m.weaponManager.GetWeaponType(PlayerMoments[CurrentMomentInTime - 1].weaponName) == "shoot")
        {
            Player.m.weaponManager.ChangeWeapon(PlayerMoments[CurrentMomentInTime - 1].weaponName, PlayerMoments[CurrentMomentInTime - 1].bulletsLeft, false);
            //Player.m.playerShooting.bulletsleft = PlayerMoments[CurrentMomentInTime - 1].bulletsLeft;
        }
        else
        {
            Player.m.weaponManager.ChangeWeapon(PlayerMoments[CurrentMomentInTime - 1].weaponName,1, false);
        }

    }



    private void Start()
    {
        CreateNewMomentInTime();
        Invoke (nameof(CreateNewMomentInTime),1f);
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
            GoToPreviousMomentInTime(true);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            GoToPreviousMomentInTime(false);
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
    
    public void GoToPreviousMomentInTime(bool GoToPreviousMoment)
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



        if (CurrentMomentInTime > 1 && GoToPreviousMoment)
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
    */




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
