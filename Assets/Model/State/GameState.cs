using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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
    public int WinningPlayerId;

    [System.NonSerialized]
    public MapPatternDefinition MapPattern;

    public GameState(int desiredPlayerCount, MapPatternDefinition mapPattern)
    {
        MapPattern = mapPattern;

        WinningPlayerId = -1;
        MapPattern = mapPattern;
        PlayerCount = desiredPlayerCount;
        CurrentPhase = GamePhase.WaitingForStart;
        ResultsFromLastPhase = new List<GameResultAction>();
        ResetPhaseToWaitingForAllPlayers();

        // manually deploy units
        players= new Dictionary<int, Player>();

        for (var i = 0; i < PlayerCount; i++)
        {
            var player = new Player {name = "Player " + (i + 1), id = i};
            player.units = new Dictionary<int, Unit>();
            players.Add(i, player);
        }

        var unitCounter = 0;
        var unitContainerGameObject = GameObject.Find("UnitContainer");
        var unitContainerTransform = unitContainerGameObject.gameObject.transform;
        for (int i = 0; i < unitContainerTransform.childCount; i++)
        {
            var unitTransform = unitContainerTransform.GetChild(i);
            var unitTransformGameObject = unitTransform.gameObject;
            var unitController = unitTransformGameObject.GetComponent<UnitController>();

            var unitData = unitController.unitData;
            var unitName = unitTransformGameObject.name;
            if (unitName.Contains("Red") && PlayerCount > 1)
            {
                unitData.owningPlayerId = 1;
            }
            else
            {
                unitData.owningPlayerId = 0;
            }

            if (unitName.Contains("Melee"))
            {
                unitData.definitionId = "meele_unit";
            } else if (unitName.Contains("Range"))
            {
                unitData.definitionId = "range_unit";
            } else if (unitName.Contains("Tank"))
            {
                unitData.definitionId = "heavy_unit";
            } else if (unitName.Contains("King"))
            {
                unitData.definitionId = "king_unit";
            }

            unitData.unitId = unitCounter++;
            unitData.position.x = (int)unitController.gameObject.transform.position.x;
            unitData.position.y = (int)unitController.gameObject.transform.position.y;
            unitData.healthPoints = unitData.Definition.maxHealth;
            players[unitData.owningPlayerId].units.Add(unitData.unitId, unitData);
            unitTransformGameObject.name = "units_" + unitData.unitId;
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
            //if (i==0)
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