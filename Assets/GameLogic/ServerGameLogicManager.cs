using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

public delegate void GameStateUpdatedHandler();

/// <summary>
/// Basic logic manager class for the server-side game logic
/// </summary>
public class ServerGameLogicManager : MonoBehaviour
{
    public GameState CurrentGameState;

    public GameStateUpdatedHandler UpdateHandlers;

    public MapPatternDefinition mapPattern;

    public UnitDefinition heavy;

    public UnitDefinition meele;

    public UnitDefinition range;

    public UnitDefinition king;

    public bool HasGameStarted
    {
        get { return CurrentGameState.CurrentPhase != GamePhase.WaitingForStart; }
    }

    public void InitializeNewGame()
    {
        CurrentGameState = new GameState(2, mapPattern);
    }

    public void InitializeNewSinglePlayerGame()
    {
        CurrentGameState = new GameState(1, mapPattern);
    }

    public void PlayerHasJoined(int playerId)
    {
        if (CurrentGameState.CurrentPhase == GamePhase.WaitingForStart)
        {
            CurrentGameState.PendingPlayerIDs.Remove(playerId);
            if (CurrentGameState.DidAllPlayersMove())
            {
                CurrentGameState.CurrentPhase = GamePhase.Planning;
                CurrentGameState.ResetPhaseToWaitingForAllPlayers();
                GameStateHasUpdated();
            }
        }
    }

    public void ReceivedActionsFromPlayer(int playerId, List<GameAction> actions)
    {
        CurrentGameState.PendingPlayerIDs.Remove(playerId);
        CurrentGameState.SetGameActionForPlayer(playerId, actions);

        if (!CurrentGameState.DidAllPlayersMove())
        {
            return;
        }

        EvaluatePlayerMovedAndAdvance();
        GameStateHasUpdated();
    }

    private void EvaluatePlayerMovedAndAdvance()
    {
        switch (CurrentGameState.CurrentPhase)
        {
            case GamePhase.Planning:
                EvaluatePlanningPhase();
                break;
            case GamePhase.Revision:
                EvaluateRevisionPhase();
                break;
            default:
                Debug.LogError("Tried to evaluate invalid game phase: " + CurrentGameState.CurrentPhase);
                break;
        }

        CurrentGameState.ResetPhaseToWaitingForAllPlayers();
    }

    private void EvaluateRevisionPhase()
    {
        CurrentGameState = ProcessPlayerActions();

        CurrentGameState.CurrentPhase = GamePhase.Planning;
        DeterminePossibleWinner();
    }

    private void DeterminePossibleWinner()
    {
        var remainingAlivePlayers = new List<int>(CurrentGameState.players.Keys);
        foreach (Player player in CurrentGameState.players.Values)
        {
            var hasKing = false;
            foreach (var unit in player.units.Values)
            {
                if (unit.Definition.Equals(king))
                {
                    hasKing = true;
                }
            }


            if (!hasKing)
            {
                remainingAlivePlayers.Remove(player.id);
            }
        }

        if (remainingAlivePlayers.Count <= 1)
        {
            CurrentGameState.CurrentPhase = GamePhase.Finished;
            if (remainingAlivePlayers.Count == 1)
            {
                CurrentGameState.WinningPlayerId = remainingAlivePlayers[0];
            }
        }
    }

    private void EvaluatePlanningPhase()
    {
        var newGameState = ProcessPlayerActions();
        CurrentGameState.ResultsFromLastPhase = newGameState.ResultsFromLastPhase;
        CurrentGameState.CurrentPhase = GamePhase.Revision;
    }

    private GameState ProcessPlayerActions()
    {
        var newGameState = GameState.Clone(CurrentGameState);
        var gameActionsByTurn = GetGameActionsPerTurn();
        var gameActionResults = new List<GameResultAction>();

        var allUnits = CurrentGameState.players.Values.SelectMany(player => player.units.Values).ToList();
        var allUnitsById = new Dictionary<int, Unit>();
        foreach (var unit in allUnits)
        {
            allUnitsById[unit.unitId] = unit;
        }

        foreach (var gameActions in gameActionsByTurn)
        {
            // move all units
            EvaluateUnitMovements(gameActions, allUnitsById, gameActionResults);

            // execute battles
            foreach (var battlePairing in DetermineBattlePairings(allUnits))
            {
                ExecuteBattleForPairing(battlePairing, gameActionResults);
            }
        }

        newGameState.ResultsFromLastPhase = gameActionResults;

        // DEBUG
//        newGameState.ResultsFromLastPhase.Add(new GameAttackResultAction(3, new Position(1, 2)));
//        List<Position> positions = new List<Position>();
//        positions.Add(new Position(0,0));
//        positions.Add(new Position(1,0));
//        positions.Add(new Position(2,0));
//        positions.Add(new Position(3,0));
//        positions.Add(new Position(4,0));
//        positions.Add(new Position(5,0));
//        positions.Add(new Position(6,0));
//        newGameState.ResultsFromLastPhase.Add(new GameMoveResultAction(3, positions));
//        newGameState.ResultsFromLastPhase.Add(new GameAttackResultAction(3, new Position(1, 2)));
//        newGameState.ResultsFromLastPhase.Add(new GameUnitDeathResultAction(3));
        // END DEBUG

        return newGameState;
    }

    private static void EvaluateUnitMovements(List<GameAction> gameActions, Dictionary<int, Unit> allUnitsById, List<GameResultAction> gameActionResults)
    {
        foreach (var gameAction in gameActions)
        {
            if (gameAction.moveToPositions.Count > 0)
            {
                var movingUnitId = gameAction.UnitId;
                var destinationPosition = gameAction.moveToPositions.Last();
                var movingUnit = allUnitsById[movingUnitId];

                gameActionResults.Add(new GameMoveResultAction(movingUnitId, gameAction.moveToPositions));
                movingUnit.position = destinationPosition;
            }
        }
    }

    private List<Unit[]> DetermineBattlePairings(List<Unit> allUnits)
    {
        var possibleBattlePairings = (
            from leftUnit in allUnits
            from rightUnit in allUnits
            where leftUnit.owningPlayerId != rightUnit.owningPlayerId && leftUnit.unitId < rightUnit.unitId
            select new[] {leftUnit, rightUnit}).ToList();

        possibleBattlePairings.Sort(delegate(Unit[] pairingA, Unit[] pairingB)
        {
            // FIXME: better sort criteria for battle pairings
            return pairingA[0].healthPoints - pairingB[0].healthPoints;
        });

        return possibleBattlePairings;
    }

    private void ExecuteBattleForPairing(Unit[] battlePairing, List<GameResultAction> gameActionResults)
    {
        var leftUnit = battlePairing[0];
        var rightUnit = battlePairing[1];
        var takenDamage = CalculateDamage(leftUnit, rightUnit);
        var leftDamage = takenDamage[0];
        var rightDamage = takenDamage[1];
        leftUnit.healthPoints = Math.Max(0, leftUnit.healthPoints - leftDamage);
        rightUnit.healthPoints = Math.Max(0, rightUnit.healthPoints - rightDamage);

        if (leftDamage > 0)
            gameActionResults.Add(new GameAttackResultAction(rightUnit.unitId, leftUnit.position));
        if (rightDamage > 0)
            gameActionResults.Add(new GameAttackResultAction(leftUnit.unitId, rightUnit.position));
        if (leftDamage > 0)
            gameActionResults.Add(new GameHitpointChangeResultAction(leftUnit.unitId, leftUnit.healthPoints));
        if (rightDamage > 0)
            gameActionResults.Add(new GameHitpointChangeResultAction(rightUnit.unitId, rightUnit.healthPoints));
        if (leftUnit.healthPoints <= 0)
            gameActionResults.Add(new GameUnitDeathResultAction(leftUnit.unitId));
        if (rightUnit.healthPoints <= 0)
            gameActionResults.Add(new GameUnitDeathResultAction(rightUnit.unitId));
    }

    private List<List<GameAction>> GetGameActionsPerTurn()
    {
        var actionCount = CurrentGameState.PlayerGameActions
            .Select(actions => actions.Count)
            .Max();

        var gameActionsByTurn = new List<List<GameAction>>();
        for (var turnNumber = 0; turnNumber < actionCount; turnNumber++)
        {
            // collect all player actions and resolve any collision
            // FIXME: Collision resolution
            var gameActions = new List<GameAction>();
            for (var playerId = 0; playerId < CurrentGameState.PlayerCount; playerId++)
            {
                var gameActionsFromPlayer = CurrentGameState.PlayerGameActions[turnNumber];
                if (turnNumber < gameActionsFromPlayer.Count)
                {
                    gameActions.Add(gameActionsFromPlayer[turnNumber]);
                }
            }
            gameActionsByTurn.Add(gameActions);
        }
        return gameActionsByTurn;
    }

    public int[] CalculateDamage(Unit fighterLeft, Unit fighterRight)
    {
        var result = new int[2] {0, 0};

        result[0] = Math.Max(
            GetAttackStrengthAtPosition(
                fighterLeft.Definition.attackPattern,
                fighterLeft.position,
                fighterRight.position) - fighterRight.Definition.DefenseAgainst(fighterLeft.Definition.type),
            0);

        result[1] = Math.Max(
            GetAttackStrengthAtPosition(
                fighterRight.Definition.attackPattern,
                fighterRight.position,
                fighterLeft.position) - fighterLeft.Definition.DefenseAgainst(fighterRight.Definition.type),
            0);

        return result;
    }

    public int GetAttackStrengthAtPosition(AttackPatternDefinition pattern, Position pos1, Position pos2)
    {
        int distX = pos1.x - pos2.x + pattern.Width / 2;
        int distY = pos1.y - pos2.y + pattern.Height / 2;

        if (distX < 0 || distX > pattern.Width || distY < 0 || distY > pattern.Height)
        {
            return 0;
        }
        return Math.Max(pattern.GetData((uint)distX,(uint) distY), 0);
    }

    private void GameStateHasUpdated()
    {
        if (UpdateHandlers != null)
        {
            UpdateHandlers();
        }
    }
}
