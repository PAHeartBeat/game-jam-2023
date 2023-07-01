using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	// Singleton instance
	private static AudioManager instance;

	// Sound effect players
	private Dictionary<string, AudioSource> sfxPlayers = new Dictionary<string, AudioSource>();

	// Music player
	private AudioSource musicPlayer;

	// UI sound player
	private AudioSource uiSoundPlayer;

	// Crossfade transition duration
	public float crossfadeDuration = 1f;

	// Global volume settings
	[Range(0f, 1f)]
	public float masterVolume = 1f;
	[Range(0f, 1f)]
	public float musicVolume = 1f;
	[Range(0f, 1f)]
	public float sfxVolume = 1f;
	[Range(0f, 1f)]
	public float uiSoundVolume = 1f;

	public AudioClip[] sfxClips;

	// Music persistence across scenes
	private bool persistMusic = true;

	// Singleton instance property
	public static AudioManager Instance {
		get {
			// If instance does not exist, find or create it
			if (instance == null) {
				instance = FindObjectOfType<AudioManager>();

				// If instance still doesn't exist, create a new GameObject with AudioManager
				if (instance == null) {
					GameObject singletonObject = new GameObject();
					instance = singletonObject.AddComponent<AudioManager>();
					singletonObject.name = "AudioManager (Singleton)";
				}
			}

			// Return the instance
			return instance;
		}
	}

	private void Awake() {
		// Make sure there's only one instance of AudioManager
		if (instance != null && instance != this) {
			Destroy(gameObject);
			return;
		}

		// Assign the instance
		instance = this;

		// Create audio source components for music, game sound effects, and UI sound effects
		musicPlayer = CreateAudioSource("MusicPlayer", transform, musicVolume, true);
		uiSoundPlayer = CreateAudioSource("UISoundPlayer", transform, uiSoundVolume);

		// Create audio source components for each sound effect
		foreach (AudioClip clip in sfxClips) {
			AudioSource sfxPlayer = CreateAudioSource(clip.name, transform, sfxVolume);
			sfxPlayers.Add(clip.name, sfxPlayer);
		}

		// Persist AudioManager across scenes
		if (persistMusic)
			DontDestroyOnLoad(gameObject);
	}

	// Create an audio source component
	private AudioSource CreateAudioSource(string name, Transform parent, float volume, bool loop = false) {
		GameObject obj = new GameObject(name);
		obj.transform.parent = parent;
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.volume = volume;
		audioSource.loop = loop;
		return audioSource;
	}

	public void PlaySFX(string clipName) {
		AudioClip clip = System.Array.Find(sfxClips, x => x.name == clipName);
		if (clip != null) {
			AudioSource sfxPlayer = sfxPlayers[clipName];
			sfxPlayer.PlayOneShot(clip);
		} else {
			Debug.LogWarning("AudioManager: Sound effect " + clipName + " not found!");
		}
	}

	// Play music clip
	public void PlayMusic(AudioClip clip) {
		StartCoroutine(TransitionMusic(clip, crossfadeDuration));
	}

	// Play UI sound clip
	public void PlayUISound(AudioClip clip) {
		uiSoundPlayer.PlayOneShot(clip);
	}

	// Stop all audio clips
	public void StopAll() {
		musicPlayer.Stop();
		foreach (AudioSource sfxPlayer in sfxPlayers.Values) {
			sfxPlayer.Stop();
		}
		uiSoundPlayer.Stop();
	}

	// Pause all audio clips
	public void PauseAll() {
		musicPlayer.Pause();
		foreach (AudioSource sfxPlayer in sfxPlayers.Values) {
			sfxPlayer.Pause();
		}
		uiSoundPlayer.Pause();
	}

	// Resume all audio clips
	public void ResumeAll() {
		musicPlayer.UnPause();
		foreach (AudioSource sfxPlayer in sfxPlayers.Values) {
			sfxPlayer.UnPause();
		}
		uiSoundPlayer.UnPause();
	}

	// Fade in music clip
	public void FadeInMusic(AudioClip clip, float fadeDuration) {
		StartCoroutine(FadeInMusicCoroutine(clip, fadeDuration));
	}

	// Fade out music clip
	public void FadeOutMusic(float fadeDuration) {
		StartCoroutine(FadeOutMusicCoroutine(fadeDuration));
	}

	// Crossfade between current and new music clip
	private IEnumerator TransitionMusic(AudioClip clip, float fadeDuration) {
		if (musicPlayer.clip == null) {
			musicPlayer.clip = clip;
			musicPlayer.Play();
		} else {
			AudioSource newMusicPlayer = CreateAudioSource("NewMusicPlayer", transform, musicVolume, true);
			newMusicPlayer.clip = clip;
			newMusicPlayer.Play();

			float currentTime = 0f;
			float startVolume = musicPlayer.volume;

			while (currentTime < fadeDuration) {
				currentTime += Time.deltaTime;
				musicPlayer.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);
				newMusicPlayer.volume = Mathf.Lerp(0f, musicVolume, currentTime / fadeDuration);
				yield return null;
			}

			musicPlayer.Stop();
			Destroy(musicPlayer.gameObject);

			musicPlayer = newMusicPlayer;
			musicPlayer.name = "MusicPlayer";
		}
	}

	// Coroutine for fading in music
	private IEnumerator FadeInMusicCoroutine(AudioClip clip, float fadeDuration) {
		if (musicPlayer.isPlaying) {
			FadeOutMusic(fadeDuration);
			yield return new WaitForSeconds(fadeDuration);
		}

		musicPlayer.clip = clip;
		musicPlayer.volume = 0f;
		musicPlayer.Play();

		float currentTime = 0f;

		while (currentTime < fadeDuration) {
			currentTime += Time.deltaTime;
			musicPlayer.volume = Mathf.Lerp(0f, musicVolume, currentTime / fadeDuration);
			yield return null;
		}
	}

	// Coroutine for fading out music
	private IEnumerator FadeOutMusicCoroutine(float fadeDuration) {
		float currentTime = 0f;
		float startVolume = musicPlayer.volume;

		while (currentTime < fadeDuration) {
			currentTime += Time.deltaTime;
			musicPlayer.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);
			yield return null;
		}

		musicPlayer.Stop();
	}
}
