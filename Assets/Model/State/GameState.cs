using System;

public enum GameEngineState
{
    ServerWaiting,
    P1Moving,
    P2Moving,
    InitialAnimation,
    P1Revising,
    P2Revising,
    RevisedAnimation,
    End
}

[System.Serializable]
public class GameState
{
    public GameEngineState CurrentState;
    public Map Map;
}