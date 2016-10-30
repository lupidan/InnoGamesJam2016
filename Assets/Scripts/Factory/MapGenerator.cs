using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	[SerializeField]
	private GameObject[] prefabs;
	private Dictionary<string, GameObject> unitPrefabCache;

    [SerializeField]
    private MapPatternDefinition mapPatternDefinition;

	void Awake()
	{
		unitPrefabCache = InitCacheFromPrefabs(prefabs);
	}

    void Start()
    {
        CreateTiles();
    }

	private Dictionary<string, GameObject> InitCacheFromPrefabs(GameObject[] prefabs)
	{
		Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();
		for (int i = 0; i < prefabs.Length; i++)
		{
			cache[prefabs[i].name] = prefabs[i];
		}
		return cache;
	}

	public T InstantiatePrefab<T>(string identifier) where T : Component
	{
		GameObject prefab = InstantiatePrefab(identifier);
		if (prefab != null)
		{
			return prefab.GetComponent<T>();
		}
		return null;
	}

	public GameObject InstantiatePrefab(string identifier)
	{
		GameObject prefab;
		if (unitPrefabCache.TryGetValue(identifier, out prefab))
		{
			return Instantiate(prefab);
		}
		return null;
	}

	public UnitController CreateUnit(Unit unit)
	{
		UnitController unitController = InstantiatePrefab<UnitController>(unit.definitionId);
		unitController.unitData = unit;
		unitController.transform.position = new Vector3(unit.position.x, unit.position.y, -2.0f);
		unitController.name = "unit_" + unit.unitId;
		return unitController;
	}

	public TileController CreateTile(Tile tile)
	{
		TileController tileController = InstantiatePrefab<TileController>(tile.definitionId);
		tileController.tileData = tile;
		tileController.transform.position = new Vector3(tile.position.x, tile.position.y, -1.0f);
		tileController.name = "tile_" + tile.position.x + "_" + tile.position.y;

	    tileController.transform.parent = gameObject.transform;
		return tileController;
	}

	private List<TileController> CreateTiles()
	{
	    var offset = mapPatternDefinition.offset;

	    List<TileController> tileControllers = new List<TileController>();
	    for (int row = 0; row < mapPatternDefinition.Height; row++)
	    {
	        for (int col = 0; col < mapPatternDefinition.Width; col++)
	        {
	            Tile tile = new Tile();
	            tile.position.x = col - offset.x;
	            tile.position.y = row - offset.y;
	            tile.definitionId = mapPatternDefinition.GetData((uint)col, (uint)row);
	            TileController tileController = CreateTile(tile);
	            tileControllers.Add(tileController);
	        }
	    }
		return tileControllers;
	}

}
