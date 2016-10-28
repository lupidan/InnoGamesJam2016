using System;
using System.Collections.Generic;

public enum GameEngineState
{
    ServerWaiting,
    Planning,
    Revision,
    End
}

[System.Serializable]
public class GameState
{
    public const int PlayerCount = 2;

    public GameEngineState CurrentState;
    public List<int> PendingPlayerIDs;
    public Map Map;

    public GameState()
    {
        CurrentState = GameEngineState.ServerWaiting;
        RefreshAllPlayersPending();
    }

    public void RefreshAllPlayersPending()
    {
        PendingPlayerIDs = new List<int>();
        for (var i = 0; i < PlayerCount; i++)
        {
            PendingPlayerIDs.Add(i);
        }
    }

    public bool MayAdvanceState()
    {
        return PendingPlayerIDs.Count == 0;
    }
}