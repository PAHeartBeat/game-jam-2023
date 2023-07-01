using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StayOnSplashScreen : MonoBehaviour {
	public Image fadeImage;
	public float fadeDuration = 1f;
	public float delayBeforeSceneChange = 5f;
	public string nextSceneName = "Menu";

	// Start is called before the first frame update
	void Start() {
		StartCoroutine(FadeOutAndChangeScene());
	}

	IEnumerator FadeOutAndChangeScene() {
		yield return new WaitForSeconds(delayBeforeSceneChange);

		float elapsedTime = 0f;
		Color originalColor = fadeImage.color;
		Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

		while (elapsedTime < fadeDuration) {
			float alpha = Mathf.Lerp(originalColor.a, targetColor.a, elapsedTime / fadeDuration);
			fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		fadeImage.color = targetColor;
		UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
	}
}
