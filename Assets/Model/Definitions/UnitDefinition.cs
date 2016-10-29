using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Assets/Unit", order = 1)]
public class UnitDefinition : ScriptableObject
{

    public enum Type
    {
        Heavy=0,
        Melee=1,
        Range=3,
        King=4
    }

    public string identifier;
    public string unitName;
    public Type type;
    public int maxHealth;
    public int attack;
    public int[] defense=new int[4]{0,0,0,0};
    public int maxMovements;
    public int maxCheatingMovements;

    public AttackPatternDefinition attackPattern;

}
