using System;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [System.Serializable]
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
    public int owningPlayerId;

	[System.NonSerialized]
	private UnitDefinition _definition;
	public UnitDefinition Definition {
		get {
			if (!_definition)
				_definition = DefinitionDatabase.Instance.UnitForId(definitionId);
			return _definition;
		}
	}

    public static string UnitControllerNameForId(int unitId)
    {
        return string.Format("unit_{0}", unitId);
    }
}
