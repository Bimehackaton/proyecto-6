using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	public List<CustomAudioClip> clips;
	public AudioSource source;

	// Use this for initialization
	void Awake () {
		instance = this;
		DontDestroyOnLoad (gameObject);
	}
	
	public void PlayClip(string clipName){
		CustomAudioClip clip = clips.Find (c => c.clipName.Equals (clipName));
		source.PlayOneShot (clip.clip);
	}
}

[System.Serializable]
public class CustomAudioClip{
	public string clipName;
	public AudioClip clip;

}
