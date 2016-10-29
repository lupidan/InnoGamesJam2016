using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public enum GamePhase
{
    WaitingForStart,
    Planning,
    Revision,
    Finished
}

[System.Serializable]
public class GameState
{
    public readonly int PlayerCount;

    public GamePhase CurrentPhase;
    public List<int> PendingPlayerIDs;
    public Map Map;
    public List<List<GameAction>> PlayerGameActions;

    public GameState(int desiredPlayerCount)
    {
        PlayerCount = desiredPlayerCount;
        CurrentPhase = GamePhase.WaitingForStart;
        ResetPhaseToWaitingForAllPlayers();
    }

    public static GameState Clone(GameState other)
    {
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, other);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return (GameState) binaryFormatter.Deserialize(memoryStream);
    }

    public void ResetPhaseToWaitingForAllPlayers()
    {
        PendingPlayerIDs = new List<int>();
        PlayerGameActions = new List<List<GameAction>>();
        for (var i = 0; i < PlayerCount; i++)
        {
            PendingPlayerIDs.Add(i);
            PlayerGameActions.Add(new List<GameAction>());
        }
    }

    public bool DidAllPlayersMove()
    {
        return PendingPlayerIDs.Count == 0;
    }

    public void SetGameActionForPlayer(int playerId, List<GameAction> actions)
    {
        PlayerGameActions[playerId] = actions;
    }
}