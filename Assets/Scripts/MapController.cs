using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public void OnBeginDrag(PointerEventData eventData) {
		Debug.Log(eventData.worldPosition);
	}

	public void OnDrag(PointerEventData eventData) {
		Debug.Log(eventData.worldPosition);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log(eventData.worldPosition);
	}

}
