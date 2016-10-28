using UnityEngine;
using System.Collections.Generic;

public class DefinitionDatabase : MonoBehaviour {

	private static DefinitionDatabase _instance;
	public static DefinitionDatabase Instance {
		get {
			if (!_instance)
				_instance = FindObjectOfType<DefinitionDatabase>();
			return _instance;
		}
	}

	[SerializeField]
	private UnitDefinition[] units;
	[SerializeField]
	private TileDefinition[] tiles;
	private Dictionary<string, UnitDefinition> unitDictionary;
	private Dictionary<string, TileDefinition> tileDictionary;
	
	void Awake ()
	{
		InitializeUnitDatabase();
		InitializeTileDictionary();
	}

	public UnitDefinition UnitForId(string identifier)
	{
		return unitDictionary[identifier];
	}

	public TileDefinition TileForId(string identifier)
	{
		return tileDictionary[identifier];
	}

	private void InitializeUnitDatabase()
	{
		unitDictionary = new Dictionary<string, UnitDefinition>();
		for (int i = 0; i < units.Length; i++)
		{
			unitDictionary[units[i].identifier] = units[i];
		}
	}

	private void InitializeTileDictionary()
	{
		tileDictionary = new Dictionary<string, TileDefinition>();
		for (int i = 0; i < tiles.Length; i++)
		{
			tileDictionary[tiles[i].identifier] = tiles[i];
		}
	}
}
