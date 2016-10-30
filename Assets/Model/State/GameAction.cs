using System.Collections.Generic;

[System.Serializable]
public class GameAction
{
    public int UnitId;
    public List<Position> moveToPositions;

    public GameAction(int unitId, List<Position> moveToPositions)
    {
        UnitId = unitId;
        this.moveToPositions = moveToPositions;
    }
}

[System.Serializable]
public class GameActions
{
    public int playerID;
    public List<GameAction> actions;
}