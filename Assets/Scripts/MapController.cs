using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour,
	IBeginDragHandler,
	IDragHandler,
	IEndDragHandler {

	private Vector3 initialCameraPosition = Vector3.zero;

	public void OnBeginDrag(PointerEventData eventData) {
		initialCameraPosition = Camera.main.transform.position;
	}

	public void OnDrag(PointerEventData eventData) {
		Vector3 worldPressPosition = Camera.main.ScreenToWorldPoint(eventData.pressPosition);
		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
		Vector3 delta = worldPressPosition - worldPosition;
		delta.z = 0.0f;
		Vector3 cameraPosition = initialCameraPosition + delta;
		Camera.main.transform.position = cameraPosition;
	}

	public void OnEndDrag(PointerEventData eventData) {}

}
