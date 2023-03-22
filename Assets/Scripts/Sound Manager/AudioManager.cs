using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class AudioManager : MonoBehaviour
{	
	public static AudioManager AM;

    public AudioMixer audioMixer;

    [Serializable]
    public class ListAndGroup
    {
        public string groupName;
        public AudioMixerGroup MixerGroup;
        public List<Sound> soundList = new List<Sound>();
    }
    public List<ListAndGroup> SoundTypeLists = new List<ListAndGroup>();

    [HideInInspector]
    public List<Sound> listOfAllSounds = new List<Sound>();

    public float RangeReference = 1f;

    void Awake()
    {

        if (AM == null)
            AM = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var listWithGroup in SoundTypeLists)
        {
            foreach(var sound in listWithGroup.soundList)
            {
                if (sound.name == "")
                    continue;

                listOfAllSounds.Add(sound);
                
                sound.source = gameObject.AddComponent<AudioSource>();
                
                SetTheAudioSettings(sound);
            }

        }

        Play("Gremory");
    }

    // This function is called by the "GiveMeSounds" script on Start to create it's audio sources
    public AudioSource INeedSoundSource(string neededSoundName, GameObject target)
    {
        Sound s = listOfAllSounds.Find(sound => sound.name == neededSoundName);

        if (s == null)
            return null;

        Sound newSound = new Sound();
        newSound.name = s.name;
        newSound.clip = s.clip;
        newSound.volume = s.volume;
        newSound.pitch = s.pitch;
        newSound.loop = s.loop;
        newSound.is3DSound= s.is3DSound;
        newSound.soundRange= s.soundRange;
        newSound.useLinearVolumeRollOff = s.useLinearVolumeRollOff;

        newSound.source = target.AddComponent<AudioSource>();

        SetTheAudioSettings(newSound);

        return newSound.source;
    }

    private void SetTheAudioSettings(Sound s)
    {
        foreach (var listWithGroup in SoundTypeLists)
        {
            Sound searchSound = listWithGroup.soundList.Find(sound => sound.name == s.name);

            if (searchSound != null)
                s.source.outputAudioMixerGroup = listWithGroup.MixerGroup;
            else
                continue;
        }

        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        if (s.is3DSound)
        {
            s.source.spatialBlend = 1;
            s.source.maxDistance = s.soundRange;

            if (s.useLinearVolumeRollOff)
                s.source.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    public void Play(string name)
    {
        Sound s = listOfAllSounds.Find(sound => sound.name == name);

        if (s == null || s.source == null)
        {
            print("Didn't find the sound: " + name);
            return;
        }

        //s.source.Play();
        s.source.PlayOneShot(s.source.clip);
    }

    public void Stop(string name)
    {
        Sound s = listOfAllSounds.Find(sound => sound.name == name);

        if (s == null || s.source == null)
        {
            print("Didn't find the sound: " + name);
            return;
        }

        s.source.Stop();
    }

    public bool isPlaying(string name)
    {
        Sound s = listOfAllSounds.Find(sound => sound.name == name);

        if (s == null || s.source == null)
        {
            print("Didn't find the sound: " + name);
            return false;
        }

        return s.source.isPlaying;
    }

    public float GetSoundLength(string name)
    {
        Sound s = listOfAllSounds.Find(sound => sound.name == name);

        if (s == null || s.source == null)
        {
            print("Didn't find the sound: " + name);
            return 0;
        }

        return s.source.clip.length;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangeReference);
    }

}
