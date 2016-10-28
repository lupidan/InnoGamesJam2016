using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Assets/Unit", order = 1)]
public class UnitDefinition : ScriptableObject
{

    public enum Type
    {
        Heavy,
        Range,
        Melee
    }

    public string identifier;
    public string unitName;
    public Type type;
    public int maxHealth;
    public int attack;
    public int defense;

}
