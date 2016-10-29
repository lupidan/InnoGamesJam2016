using UnityEngine;
using UnityEngine.EventSystems;

public class TileController :
	MonoBehaviour,
	IPointerEnterHandler,
    IPointerExitHandler {

	public static TileController HighlightedTile { get; private set; }

	public Tile tileData;

	public void OnPointerEnter(PointerEventData eventData) {
		if (HighlightedTile != this)
			HighlightedTile = this;
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (HighlightedTile == this)
			HighlightedTile = null;
	}
	
}
