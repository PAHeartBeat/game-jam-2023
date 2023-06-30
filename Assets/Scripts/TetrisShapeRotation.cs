using UnityEngine;

public class TetrisShapeRotation : MonoBehaviour {
	private void Update() {
		// Rotate the shape by 90 degrees if the rotation input is detected
		if (Input.GetKeyDown(KeyCode.Space)) {
			this.transform.Rotate(Vector3.right, 90f, Space.Self);
		}
	}
}
