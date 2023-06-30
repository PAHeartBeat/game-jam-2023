using UnityEngine;
using UnityEngine.EventSystems;

public class ShapesPickHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
	public Vector3 dragOffset; // Offset to apply when dragging

	public void OnDrag(PointerEventData eventData) {
		transform.localPosition = Input.mousePosition + dragOffset;
	}

	public void OnEndDrag(PointerEventData eventData) {
		transform.localPosition = Vector3.zero;
	}
}
