using UnityEngine;

[System.Serializable]
public class Unit
{
	public enum Direction {
		Right,
		Left,
		Up,
		Down
	}

	public UnitDefinition Definition;
	public Direction facingDirection;
	public Position position;
	public int healthPoints;
	
}
