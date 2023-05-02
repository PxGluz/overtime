using System.Collections;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    private static MusicSystem instance = null;
    public NeedSounds soundMaster;
    public static MusicSystem Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        soundMaster = GetComponent<NeedSounds>();
        soundMaster.StandardPlay("BackGroundMusic");
        //functionality here
    }
}
