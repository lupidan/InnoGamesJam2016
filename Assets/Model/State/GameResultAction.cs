using System.Collections.Generic;

[System.Serializable]
public class GameResultAction
{
    public int unitId;

    public GameResultAction(int unitId)
    {
        this.unitId = unitId;
    }
}

[System.Serializable]
public class GameMoveResultAction : GameResultAction
{
    public List<Position> movements;

    public GameMoveResultAction(int unitId, List<Position> movements) : base(unitId)
    {
        this.movements = movements;
    }
}

[System.Serializable]
public class GameRotateResultAction : GameResultAction
{
    public Unit.Direction direction;

    public GameRotateResultAction(int unitId, Unit.Direction direction) : base(unitId)
    {
        this.direction = direction;
    }
}

[System.Serializable]
public class GameAttackResultAction : GameResultAction
{
    public Position target;

    public GameAttackResultAction(int unitId, Position target) : base(unitId)
    {
        this.target = target;
    }
}

[System.Serializable]
public class GameHitpointChangeResultAction : GameResultAction
{
    public int oldHitpointValue;
    public int newHitpointValue;

    public GameHitpointChangeResultAction(int unitId, int oldHitpointValue, int newHitpointValue) : base(unitId)
    {
        this.oldHitpointValue = oldHitpointValue;
        this.newHitpointValue = newHitpointValue;
    }
}

[System.Serializable]
public class GameUnitDeathResultAction : GameResultAction
{
    public GameUnitDeathResultAction(int unitId) : base(unitId)
    {
    }
}

