using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;

    [Header("For all sounds:")]
    [Range(0, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    public bool loop;
    [Header("For 3D sounds:")]
    public bool is3DSound;
    [Range(0, 1000f)]
    public float soundRange = 10f;
    public bool useLinearVolumeRollOff = false;

}

