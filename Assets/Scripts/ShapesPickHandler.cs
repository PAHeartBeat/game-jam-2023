using UnityEngine;
using UnityEngine.EventSystems;

public class ShapesPickHandler : MonoBehaviour, IDragHandler, IEndDragHandler {
	public Camera mainCamera;
	
	public void OnDrag(PointerEventData eventData) {
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Vector3.Distance(transform.position, mainCamera.transform.position);
		transform.position = mainCamera.ScreenToWorldPoint(mousePosition);
	}

	public void OnEndDrag(PointerEventData eventData) {
		transform.localPosition = Vector3.zero;
	}
}
