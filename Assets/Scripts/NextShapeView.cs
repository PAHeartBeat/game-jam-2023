using UnityEngine;
using UnityEngine.UI;

public class NextShapeView : MonoBehaviour
{
	public Sprite[] sprites;
	public Image icon;
	public TetrisShapeShooter tetrisShapeShooter;

	private void Awake() {
		TetrisShapeShooter.onShapeChange += this.OnShapeChangeMethod;
	}

	private void OnDestroy() {
		TetrisShapeShooter.onShapeChange -= this.OnShapeChangeMethod;
	}

	private void Start() {
		this.OnShapeChangeMethod(this.tetrisShapeShooter._currentShapeIndex);
	}

	public void OnShapeChangeMethod(int index) {
		icon.sprite = sprites[index];
	}
}
