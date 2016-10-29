using System.Collections.Generic;

[System.Serializable]
public class GameResultAction
{
    public int unitId;
}

[System.Serializable]
public class GameMoveResultAction : GameResultAction
{
    public List<Position> movements;
}

[System.Serializable]
public class GameRotateResultAction : GameResultAction
{
    public Unit.Direction direction;
}

[System.Serializable]
public class GameAttackResultAction : GameResultAction
{
    public Position target;
}

[System.Serializable]
public class GameHitpointChangeResultAction : GameResultAction
{
    public int newHitpointValue;
}

[System.Serializable]
public class GameUnitDeathResultAction : GameResultAction
{
}

