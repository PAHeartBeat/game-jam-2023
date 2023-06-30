using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisShape : MonoBehaviour
{
	private Vector3 offset;
	private bool isDragging = false;

	public bool isInCannon = false;
	public GameObject spawnedObjectPrefab; // Prefab to spawn when dragging

	private GameObject spawnedObject; // Instance of the spawned object

	private void OnMouseDown() {
		if (!isInCannon) {
			// Calculate the offset between the shape's position and the mouse position
			offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
			offset.z = 0f;

			// Set the dragging flag
			isDragging = true;

			// Disable physics during drag
			EnablePhysics(false);

			// Spawn the object to drag
			spawnedObject = Instantiate(spawnedObjectPrefab, transform.position, Quaternion.identity);
			spawnedObject.transform.localScale = new Vector3(200f, 14f, 1f); // Set the scale
			spawnedObject.layer = LayerMask.NameToLayer("UI"); // Set the layer to UI
		}
	}



	private void OnMouseDrag() {
		if (isDragging) {
			Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
			newPosition.z = 0f;
			transform.position = newPosition;

			// Update the position of the spawned object
			spawnedObject.transform.position = newPosition;
		}
	}

	private void OnMouseUp() {
		return;
		if (isDragging) {
			// Enable physics after drag
			EnablePhysics(true);

			// Reset the dragging flag
			isDragging = false;

			// Check if the shape is dropped onto the cannon assembly
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				if (hit.collider.CompareTag("CannonAssembly")) {
					// Do something when the shape is dropped onto the cannon assembly
					// Example: Call a method on the cannon assembly to load the shape
					CannonAssembly cannonAssembly = hit.collider.gameObject.GetComponent<CannonAssembly>();
					if (cannonAssembly != null) {
						cannonAssembly.LoadShape(spawnedObject);
					}
				}
			}

			// Destroy the spawned object
			Destroy(spawnedObject);
		}
	}

	public void ResetPosition() {
		transform.localPosition = Vector3.zero;
	}

	public void EnablePhysics(bool enable) {
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		rigidbody.isKinematic = !enable;
	}
}
