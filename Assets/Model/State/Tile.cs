using UnityEngine;

[System.Serializable]
public class Tile {

	public int tileId;
	public string definitionId;
	public Position position;

	[System.NonSerialized]
	private TileDefinition _definition;
	public TileDefinition Definition {
		get {
			if (!_definition)
				_definition = DefinitionDatabase.Instance.TileForId(definitionId);
			return _definition;
		}
	}
}
