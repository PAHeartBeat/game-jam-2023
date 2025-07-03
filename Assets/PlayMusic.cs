using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
	public AudioClip gameMusic;

	private void Start() {
		AudioManager.Instance.PlayMusic(gameMusic);
	}
}
