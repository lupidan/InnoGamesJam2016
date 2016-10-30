using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TileController :
	MonoBehaviour,
	IPointerEnterHandler,
    IPointerExitHandler {

	public static TileController HighlightedTile { get; private set; }

	public static void SetTilesAtPositionsReachable(List<Position> positions) {
		for (int i = 0; i < positions.Count; i++)
		{
			GameObject tileGameObject = GameObject.Find("tile_" + positions[i].x + "_" + positions[i].y);
			if (tileGameObject)
            	tileGameObject.GetComponent<TileController>().SetReachable(true);
		}
	}

	public static void SetAllTilesUnreachable() {
		TileController[] tileControllers = FindObjectsOfType<TileController>();
        for (int i = 0; i < tileControllers.Length; i++)
        {
            tileControllers[i].SetReachable(false);
        }
	}

	public static List<Position> PathFromPositionToPosition(Position origin, Position destiny) {
		List<Position> path = new List<Position>();
		Position iterator = origin;
		while (destiny.x != iterator.x || destiny.y != iterator.y) {
			if (iterator.x < destiny.x)
				iterator.x += 1;
			else if (iterator.x > destiny.x)
				iterator.x -= 1;
			else if (iterator.y < destiny.y)
				iterator.y += 1;
			else if (iterator.y > destiny.y)
				iterator.y -= 1;

			Position copy = new Position(iterator.x, iterator.y);
			path.Add(copy);
		}
		return path;
	}

	public Tile tileData;
	public bool isReachable { get; private set; }

	public void OnPointerEnter(PointerEventData eventData) {
		if (HighlightedTile != this && isReachable)
			HighlightedTile = this;
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (HighlightedTile == this && isReachable)
			HighlightedTile = null;
	}

	public void SetReachable(bool reachable) {
		isReachable = reachable;
		GetComponent<SpriteRenderer>().color = isReachable ? Color.black : Color.white;
	}
	
}
