using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager: MonoBehaviour
{
	public MovementManager ()
	{
	}

	public List<Position> getReachablePositions(List<TileController> allTiles, Unit unit) {
		return new List<Position> ();
	}
}

