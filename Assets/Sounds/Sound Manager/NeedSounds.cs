using System;
using System.Collections;
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

    public void StandardPlay(string name)
    {
        MySounds s = Array.Find(mySounds, sound => sound.name == name);


        if (s == null || s.source == null)
        {
            print("Didn't find the sound: " + name);
            return;
        }

        s.source.Play();
        //s.source.PlayOneShot(s.source.clip);
    }
    public void Stop(string name)
    {
        MySounds s = Array.Find(mySounds, sound => sound.name == name);

        if (s == null || s.source == null)
        {
            print("Didn't find the sound: " + name);
            return;
        }

        s.source.Stop();
    }

    public IEnumerator FadeOut(string name, float FadeTime)
    {
        AudioSource audioSource = Array.Find(mySounds, sound => sound.name == name).source;
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
