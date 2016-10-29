using System;
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
    public int heavyDefense;
    public int meeleDefense;
    public int rangeDefense;
    public int kingDefense;
    public int maxMovements;
    public int maxCheatingMovements;

    public AttackPatternDefinition attackPattern;

    public int DefenseAgainst(Type type)
    {
        switch (type)
        {
            case Type.Heavy:
                return heavyDefense;
            case Type.Melee:
                return meeleDefense;
            case Type.Range:
                return rangeDefense;
            case Type.King:
                return kingDefense;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }
}
