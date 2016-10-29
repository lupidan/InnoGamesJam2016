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
    public const int PlayerCount = 2;

    public GamePhase CurrentPhase;
    public List<int> PendingPlayerIDs;
    public Map Map;

    public GameState()
    {
        CurrentPhase = GamePhase.WaitingForStart;
        RefreshAllPlayersPending();
    }

    public static GameState Clone(GameState other)
    {
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, other);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return (GameState) binaryFormatter.Deserialize(memoryStream);
    }

    public void RefreshAllPlayersPending()
    {
        PendingPlayerIDs = new List<int>();
        for (var i = 0; i < PlayerCount; i++)
        {
            PendingPlayerIDs.Add(i);
        }
    }

    public bool AllPlayersMoved()
    {
        return PendingPlayerIDs.Count == 0;
    }
}