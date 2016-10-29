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
    public MapPatternDefinition mapPattern;

    public GameState(
        int desiredPlayerCount,
        MapPatternDefinition mapPattern,
        UnitDefinition meeleDefinition,
        UnitDefinition heavyDefinition,
        UnitDefinition rangeDefinition
    )
    {
        this.mapPattern = mapPattern;
        PlayerCount = desiredPlayerCount;
        CurrentPhase = GamePhase.WaitingForStart;
        ResultsFromLastPhase = new List<GameResultAction>();
        ResetPhaseToWaitingForAllPlayers();

        // manually deploy units
        players= new Dictionary<int, Player>();

        int unitCounter = 0;
        for (int i = 0; i < PlayerCount; i++)
        {
            Player player= new Player();
            player.name = "Player "+1;

            Unit heavy= new Unit();
            heavy.unitId = unitCounter++;
            heavy.owningPlayerId = i;
            heavy.facingDirection = Unit.Direction.Right;
            heavy.position = new Position(2, 2);


            player.units = new Dictionary<int, Unit>();
            player.units.Add(heavy.unitId, heavy);
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