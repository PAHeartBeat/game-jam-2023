using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
	public string scene;
	public AudioClip uimusic;

	private void Start() {
		AudioManager.Instance.PlayMusic(uimusic);
	}

	public void ChangeScene() {
		UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
	}
}
