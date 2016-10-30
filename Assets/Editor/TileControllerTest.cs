using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;
using System;

using PositionList = System.Collections.Generic.List<Position>;

public class TileControllerTest {

	[Test]
	public void PathFromPositionToPositionTest()
	{
		//Arrange
		Position origin = new Position(2,0);
		Position destination = new Position (0, 4);

		List<Position> path = TileController.PathFromPositionToPosition (origin, destination);
		Position[] expectedPathArr = new Position[] {
			new Position (2, 1),
			new Position (2, 2),
			new Position (1, 2),
			new Position (1, 3),
			new Position (0, 3),
			new Position (0, 4)
		};
		PositionList expectedPath = new PositionList (expectedPathArr);
		//foreach (var item in path) {
		//	Debug.Log (item.ToString());
		//}
		Assert.AreEqual(expectedPath, path);
	}
}
