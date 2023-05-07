using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestoyOnLoad : MonoBehaviour
{
    private static DoNotDestoyOnLoad instance = null;
    public static DoNotDestoyOnLoad Instance
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
}
