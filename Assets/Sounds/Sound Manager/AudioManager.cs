using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


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
            foreach (var sound in listWithGroup.soundList)
            {
                if (sound.name == "")
                    continue;

                listOfAllSounds.Add(sound);

                sound.source = gameObject.AddComponent<AudioSource>();

                SetTheAudioSettings(sound);
            }
        }

        SetDialogueAudioSettings();

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
        newSound.is3DSound = s.is3DSound;
        newSound.soundRange = s.soundRange;
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

    public void StandardPlay(string name)
    {
        Sound s = listOfAllSounds.Find(sound => sound.name == name);

        if (s == null || s.source == null)
        {
            print("Didn't find the sound: " + name);
            return;
        }

        s.source.Play();
        //s.source.PlayOneShot(s.source.clip);
    }


    // Dialogue system from here:
    public AudioMixerGroup DialogueMixerGroup;
    public List<DialogueLineGroup> dialogueLineGroups = new List<DialogueLineGroup>();

    [System.Serializable]
    public class DialogueLineGroup
    {
        public string GroupName;
        public List<DialogueLine> linesList = new List<DialogueLine>();
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string lineText;
        public int ID;
        public AudioClip clip;
        [HideInInspector]
        public AudioSource source;
        [Range(0, 1f)]
        public float volume = 1f;
        [Range(-3f, 3f)]
        public float pitch = 1f;
    }

    private void SetDialogueAudioSettings()
    {
        foreach (var dialogueGroup in dialogueLineGroups)
        {
            foreach (var dialogueLine in dialogueGroup.linesList)
            {
                if (dialogueLine.clip == null)
                    continue;

                dialogueLine.source = gameObject.AddComponent<AudioSource>();
                dialogueLine.source.outputAudioMixerGroup = DialogueMixerGroup;

                dialogueLine.source.clip = dialogueLine.clip;
                dialogueLine.source.volume = dialogueLine.volume;
                dialogueLine.source.pitch = dialogueLine.pitch;
            }
        }
    }

    public List<DialogueLine> GetDialogueGroupByName(string groupName)
    {
        List<DialogueLine> listToReturn = dialogueLineGroups.Find(group => group.GroupName == groupName).linesList;

        if (listToReturn != null)
        {
            return listToReturn;
        }

        return null;
    }

    public void PlayFromDialogueGroupByID(string groupName, int givenID)
    {
        List<DialogueLine> listToReturn = dialogueLineGroups.Find(group => group.GroupName == groupName).linesList;

        if (listToReturn == null)
        {
            print("audio group " + groupName + " not found");
            return;
        }

        listToReturn.Find(sound => sound.ID == givenID).source.Play();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            PlayFromDialogueGroupByID("tutorial", 1);
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangeReference);
    }

}
