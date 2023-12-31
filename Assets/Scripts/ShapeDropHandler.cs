using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeDropHandler : MonoBehaviour,IDropHandler
{
	public Transform cannonTransform; // Reference to the transform of the cannon object
	public float maxDistance = 2f; // Maximum distance allowed for the drop

	public void OnDrop(PointerEventData eventData) {
		if (eventData.pointerDrag != null && Vector3.Distance(eventData.pointerDrag.transform.position, cannonTransform.position) <= maxDistance) {
			// The dropped item is within the maximum distance from the cannon object
			Debug.Log("Dropped within the maximum distance from the cannon!");
			// Add your desired code logic here for when the item is dropped within the distance

			eventData.pointerDrag.transform.gameObject.SetActive(false);
		}
	}
}
