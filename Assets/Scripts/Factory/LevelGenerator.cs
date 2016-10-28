using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

	[SerializeField]
	private GameObject[] prefabs;
	private Dictionary<string, GameObject> unitPrefabCache;

	void Awake()
	{
		unitPrefabCache = InitCacheFromPrefabs(prefabs);
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

	private T InstantiatePrefab<T>(string identifier) where T : Component
	{
		GameObject prefab;
		if (unitPrefabCache.TryGetValue(identifier, out prefab))
		{
			GameObject instantiatedGameObject = Instantiate(prefab);
			return instantiatedGameObject.GetComponent<T>();
		}
		return null;
	}

	public UnitController CreateUnit(Unit unit)
	{
		UnitController unitController = InstantiatePrefab<UnitController>(unit.definitionId);
		//unitController.unit = unit;
		unitController.transform.position = new Vector3(unit.position.x, unit.position.y, 0.0f);
		return unitController;
	}

	public TileController CreateTile(Tile tile)
	{
		TileController tileController = InstantiatePrefab<TileController>(tile.definitionId);
		tileController.tile = tile;
		tileController.transform.position = new Vector3(tile.position.x, tile.position.y, 0.0f);
		return tileController;
	}
}
