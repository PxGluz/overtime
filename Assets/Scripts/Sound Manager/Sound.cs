using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{	
	public string name;

	public AudioClip clip;

	[Range(0,1f)]
	public float volume = 1f;
	[Range(.1f,3f)]
	public float pitch = 0f;

	public bool loop = false;

	[HideInInspector]
	public AudioSource source;
}
