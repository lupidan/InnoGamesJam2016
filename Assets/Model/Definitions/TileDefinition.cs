using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Assets/Tile", order = 1)]
public class TileDefinition : ScriptableObject {

	public enum Type {
		Normal,
		Block,
        Swamp
	}

	public string identifier;
	public Type type;
	public bool walkable;
	public int cost;
	public int attackBonus;
	public int defenseBonus;
	
}
