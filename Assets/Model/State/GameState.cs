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
    public Dictionary<int, Player> players;
    public List<GameResultAction> ResultsFromLastPhase;
    public List<List<GameAction>> PlayerGameActions;
    public MapPatternDefinition MapPattern;
    public int WinningPlayerId;

    public GameState(int desiredPlayerCount, MapPatternDefinition mapPattern)
    {
        WinningPlayerId = -1;
        MapPattern = mapPattern;
        PlayerCount = desiredPlayerCount;
        CurrentPhase = GamePhase.WaitingForStart;
        ResultsFromLastPhase = new List<GameResultAction>();
        ResetPhaseToWaitingForAllPlayers();

        // manually deploy units
        players= new Dictionary<int, Player>();

        var unitCounter = 0;
        for (var i = 0; i < PlayerCount; i++)
        {
            var player = new Player { name = "Player " + (i + 1), id = i };

            // FIXME: unit initialization
            var heavy = new Unit
            {
                unitId = unitCounter++,
                owningPlayerId = i,
                facingDirection = Unit.Direction.Right,
                position = new Position(2, 2)
            };

            player.units = new Dictionary<int, Unit> {{heavy.unitId, heavy}};

            players.Add(i, player);
        }

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

    public List<GameAction> GetGameActionForPlayer(int playerId)
    {
        return PlayerGameActions[playerId];
    }
}