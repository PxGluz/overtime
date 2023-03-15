using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{	
	public AudioMixerGroup MasterMixer;
    public AudioMixer audioMixer;

    public List<Sound> sounds;


	public static AudioManager AM;

    void Awake()
    {	

    	if ( AM == null)
    		AM = this;
    	else{
    		Destroy(gameObject);
    		return;
    	}

        foreach (Sound s in sounds)
        {
        	s.source = gameObject.AddComponent<AudioSource>();
        	s.source.clip = s.clip;
        	s.source.outputAudioMixerGroup = MasterMixer;

        	s.source.volume = s.volume;
        	s.source.pitch = s.pitch;
        	s.source.loop = s.loop;
        }
        //AudioManager.AM.audioMixer.SetFloat("volume", 69f);
    }

    public void Play(string name)
    {
    	Sound s = sounds.Find ( sound => sound.name == name);

    	if (s == null)
    		return;

    	s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = sounds.Find ( sound => sound.name == name);

        if (s == null)
            return;

        s.source.Stop();
    }


}
