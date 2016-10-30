using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class TileController :
	MonoBehaviour,
	IPointerDownHandler,
    IPointerClickHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{

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

	public static List<Position> PathFromPositionToPosition(Position origin, Position destination) {
		List<Position> path = new List<Position>();
		Position current = origin;
		while (!current.Equals(destination)) {
			//yDistance, xDistance. Reduce what is higher
			int absoluteHorizontalDistance = Math.Abs(current.x - destination.x);
			int absoluteVerticalDistance = Math.Abs (current.y - destination.y);
			int nextX = current.x;
			int nextY = current.y;
			if (absoluteHorizontalDistance >= absoluteVerticalDistance) {
				//reduce horizontal distance
				if (current.x < destination.x) {
					nextX = current.x + 1;
				} else {
					nextX = current.x - 1;
				}
			} else {
				//reduce vertical distance
				if (current.y < destination.y) {
					nextY = current.y + 1;
				} else {
					nextY = current.y - 1;
				}
			}
			Position next = new Position (nextX, nextY);
			path.Add(next);
			current = next;
		}
		return path;
	}

	public Tile tileData;
	public bool isReachable { get; private set; }

	public void OnPointerEnter(PointerEventData eventData) {
	    if (isReachable)
	    {
    	        var currentCurrentSelectedGameObject = UnitController.SelectedUnit;
	        if (currentCurrentSelectedGameObject)
	        {
	            var attackPatternGameObject = GameObject.Find("AttackPatternRenderer");
	            var attackPatternRenderer = attackPatternGameObject.GetComponent<AttackPatternRenderer>();
	            attackPatternRenderer.SetPattern(tileData.position, transform,
	                currentCurrentSelectedGameObject.GetComponent<UnitController>().unitData.Definition.attackPattern);
	        }
	    }

	}

	public void OnPointerExit(PointerEventData eventData) {
	    if (isReachable)
	    {
	        var currentCurrentSelectedGameObject = UnitController.SelectedUnit;
	        if (currentCurrentSelectedGameObject)
	        {
	            var attackPatternGameObject = GameObject.Find("AttackPatternRenderer");
	            var attackPatternRenderer = attackPatternGameObject.GetComponent<AttackPatternRenderer>();
	            attackPatternRenderer.HidePattern();
	        }
	    }
	}

	public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UnitController.SelectedUnit != null)
        {
            UnitController selectedUnit = UnitController.SelectedUnit;
            if (isReachable)
            {
                selectedUnit.SetDestinationTileController(this);
                var attackPatternGameObject = GameObject.Find("AttackPatternRenderer");
                var attackPatternRenderer = attackPatternGameObject.GetComponent<AttackPatternRenderer>();
                attackPatternRenderer.HidePattern();
            }
        }
        UnitController.SelectedUnit = null;
    }

    public void SetReachable(bool reachable) {
		if (!tileData.Definition.walkable)
			return;

		isReachable = reachable;
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		Color newColor = spriteRenderer.color;
		newColor.a = isReachable ? 1.0f : 0.0f;
		spriteRenderer.color = newColor;
	}
}
