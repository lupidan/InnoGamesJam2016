using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class MovementManagerTest {

	[Test]
	public void EditorTest()
	{
		//Arrange
		var gameObject = new GameObject();

		//Act
		//Try to rename the GameObject
		var newGameObjectName = "My game object";
		gameObject.name = newGameObjectName;

		//Assert
		//The object has a new name
		Assert.AreEqual(newGameObjectName, gameObject.name);
	}

	[Test]
	public void IsPositionValidTest() {
		Debug.Log ("Testing is position valid");
		var manager = new MovementManager ();
		Assert.IsTrue (manager.isPositionValid (new Position (0, 0), 1, 1));
		Assert.IsFalse (manager.isPositionValid (new Position (0, 1), 1, 1));
		Assert.IsFalse (manager.isPositionValid (new Position (1, 0), 1, 1));
		Assert.IsFalse (manager.isPositionValid (new Position (-1, 0), 1, 1));
		Assert.IsFalse (manager.isPositionValid (new Position (0, -1), 1, 1));
	}

	[Test]
	public void calculateRealCostsTest() {
		var manager = new MovementManager ();
		int[,] costs = {							{1,1,1,1,1},
													{1,1,1,1,1},
													{1,1,1,1,1},
													{1,1,1,1,1},
													{1,1,1,1,1}};

		int[,] expectedRealCostsWithMaxCosts2 = {	{-1,-1,2,-1,-1},
													{-1,2,1,2,-1},
													{2,1,0,1,2},
													{-1,2,1,2,-1},
													{-1,-1,2,-1,-1}};		
		
		int[,] expectedRealCostsWithMaxCosts4 = {	{4,3,2,3,4},
													{3,2,1,2,3},
													{2,1,0,1,2},
													{3,2,1,2,3},
													{4,3,2,3,4}};		
		Position startPosition = new Position(2,2);
		int[,] realCostsWithMaxCost1 = manager.calculateRealCostsMatrix (costs, startPosition, 2);
		Debug.Log ("Real costs;\n" + intMatrixToString(realCostsWithMaxCost1));
		Assert.AreEqual (expectedRealCostsWithMaxCosts2, realCostsWithMaxCost1);
		int[,] realCostsWithMaxCost2 = manager.calculateRealCostsMatrix (costs, startPosition, 4);
		//Debug.Log ("Real costs;\n" + intMatrixToString(realCostsWithMaxCost2));
		Assert.AreEqual (expectedRealCostsWithMaxCosts4, realCostsWithMaxCost2);
	}

	[Test]
	public void calculateRealCostsTestWithObstacles() {
		var manager = new MovementManager ();
		int[,] costs = {							{1,1,1,1,1},
													{1,0,0,0,1},
													{1,1,1,0,1},
													{0,0,0,0,1},
													{1,1,1,1,1}};

		int[,] expectedRealCosts = {				{4,5,6,7,8},
													{3,-1,-1,-1,9},
													{2,1,0,-1,10},
													{-1,-1,-1,-1,11},
													{16,15,14,13,12}};
		Position startPosition = new Position(2,2);
		int[,] realCosts = manager.calculateRealCostsMatrix (costs, startPosition, 0);
		Debug.Log ("Real costs with obstacles;\n" + intMatrixToString(realCosts));	
		Assert.AreEqual (expectedRealCosts, realCosts);
	}

	public string intMatrixToString(int[,] intMatrix) {
		string result = "";
		for (int x = 0; x < intMatrix.GetLength (0); x++) {
			for (int y = 0; y < intMatrix.GetLength (1); y++) {
				result += intMatrix [x, y];
				result += " ";
			}
			result += "\n";
		}
		return result;
	}
}
