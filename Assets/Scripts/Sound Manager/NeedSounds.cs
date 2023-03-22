using System;
using UnityEngine;

public class NeedSounds : MonoBehaviour
{
    [System.Serializable]
    public class MySounds
    {
        public string name;
        [HideInInspector]
        public AudioSource source;
    }

    [SerializeField] public MySounds[] mySounds;

    void Start()
    {
        for (int i = 0; i < mySounds.Length; i++)
        {
            if (mySounds[i].name != "")
            {
                AudioSource s = AudioManager.AM.INeedSoundSource(mySounds[i].name, gameObject);

                if (s != null)
                    mySounds[i].source = s;
            }
        }

    }

    public void Play(string name)
    {
        MySounds s = Array.Find(mySounds, sound => sound.name == name);


        if (s == null || s.source == null) {
            print("Didn't find the sound: " + name);
            return;
        }

        //s.source.Play();
        s.source.PlayOneShot(s.source.clip);
    }
}
