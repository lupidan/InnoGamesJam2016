using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Assets/Tile", order = 1)]
public class TileDefinition : ScriptableObject {

	public enum Type {
		Grass,
		Mountain,
		Forest,
        Swamp
	}

	public string identifier;
	public Type type;
	public bool walkable;
	public int cost;
	public int attackBonus;
	public int defenseBonus;
	
}
