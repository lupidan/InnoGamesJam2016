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

	public int unitId;
	public string definitionId;
	public Direction facingDirection;
	public Position position;
	public int healthPoints;

	[System.NonSerialized]
	private UnitDefinition _definition;
	public UnitDefinition Definition {
		get {
			if (!_definition)
				_definition = DefinitionDatabase.Instance.UnitForId(definitionId);
			return _definition;
		}
	}
	
}
