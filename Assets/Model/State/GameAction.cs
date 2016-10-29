using System.Collections.Generic;

[System.Serializable]
public class GameAction
{
    public int UnitId;
    public Position moveToPosition;
}

[System.Serializable]
public class GameActions
{
    public int playerID;
    public List<GameAction> actions;
}