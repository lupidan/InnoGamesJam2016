using UnityEngine;

public class SoundController : MonoBehaviour {

	private static SoundController _instance = null;
	public static SoundController Instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<SoundController>();
			}
			return _instance;
		}
	}

	public AudioSource generalAudioSource;
	public AudioSource backgroundAudioSource;
	public AudioClip[] audioClips;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public AudioClip AudioClipForName(string audioFilename) {
		for (int i = 0; i < audioClips.Length; i++)
		{
			if (audioClips[i].name == audioFilename)
				return audioClips[i];
		}
		return null;
	}

	public void PlayAudioClip(string audioFilename) {
		PlayAudioClip(AudioClipForName(audioFilename));
	}

	public void PlayAudioClip(AudioClip audioClip) {
		if (audioClip == null)
			return;

		generalAudioSource.PlayOneShot(audioClip);
	}

	public void PlayBackgroundClip(string audioFilename) {
		PlayBackgroundClip(AudioClipForName(audioFilename));
	}

	public void PlayBackgroundClip(AudioClip audioClip) {
		if (audioClip == null)
			return;

		backgroundAudioSource.Stop();
		backgroundAudioSource.clip = audioClip;
		backgroundAudioSource.loop = true;
		backgroundAudioSource.Play();
	}
	
}
