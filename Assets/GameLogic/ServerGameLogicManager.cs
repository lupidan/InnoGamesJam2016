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
        Debug.Log("Player with id "+ playerId+" has joined!");
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
        foreach (var player in CurrentGameState.players.Values)
        {
            
			if (player.units.Values.Any(unit => unit.Definition.type.Equals(UnitDefinition.Type.King) && unit.healthPoints<=0))
            {
                remainingAlivePlayers.Remove(player.id);
            }
        }

        if (remainingAlivePlayers.Count <= 1 && CurrentGameState.PlayerCount > 1)
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

        var allUnits = newGameState.players.Values.SelectMany(player => player.units.Values).ToList();
        var allUnitsById = new Dictionary<int, Unit>();
        foreach (var unit in allUnits)
        {
            allUnitsById[unit.unitId] = unit;
        }

        var gameActionsByTurn = GetGameActionsPerTurn(allUnitsById);
        var gameActionResults = new List<GameResultAction>();

        var blockedUnitIds = new List<int>();
        string battlelog = "";
        foreach (var unit in allUnits)
        {
            battlelog += ("before: unit " + unit.unitId + " h " + unit.healthPoints + " @ " + unit.position.x + ", " + unit.position.y + "\n");
        }
        foreach (var gameActions in gameActionsByTurn)
        {
            // move all units
            EvaluateUnitMovements(gameActions, allUnitsById, gameActionResults, blockedUnitIds);
        }

        // execute battles
        foreach (var battlePairing in DetermineBattlePairings(allUnits))
        {
            ExecuteBattleForPairing(battlePairing, gameActionResults);
        }

        // king loses health if left alone
        var kings = newGameState.players.Values
            .SelectMany(player =>
                    player.units.Values
                        .Where(unit => unit.Definition.type.Equals(UnitDefinition.Type.King)
                                        && unit.healthPoints > 0).ToList())
            .ToList();

        foreach (var king in kings)
        {
            var kingPosX = king.position.x;
            var kingPosY = king.position.y;
            var playerId = king.owningPlayerId;
            bool foundNeighbor = false;

            foreach (Unit unit in allUnits)
            {
                if (unit.owningPlayerId != playerId || unit == king)
                {
                    continue;
                }

                var distance = Math.Abs(unit.position.x - kingPosX) + Math.Abs(unit.position.y - kingPosY);
                if (distance <= 4)
                {
                    foundNeighbor = true;
                }
            }

            if (!foundNeighbor)
            {
                var oldHitpoints = king.healthPoints;
                king.healthPoints -= 1;
                gameActionResults.Add(new GameHitpointChangeResultAction(king.unitId, oldHitpoints, king.healthPoints));
                if (king.healthPoints <= 0)
                    gameActionResults.Add(new GameUnitDeathResultAction(king.unitId));
            }
        }

        foreach (var unit in allUnits)
        {
            battlelog += ("after: unit " + unit.unitId + " h " + unit.healthPoints + " @ " + unit.position.x + ", " + unit.position.y + "\n");
        }
        Debug.Log(battlelog);

        newGameState.ResultsFromLastPhase = gameActionResults;

        return newGameState;
    }

    private static void EvaluateUnitMovements(List<GameAction> gameActions, Dictionary<int, Unit> allUnitsById, List<GameResultAction> gameActionResults, List<int> blockedUnitIds)
    {
        foreach (var gameAction in gameActions)
        {
            if (gameAction.moveToPositions.Count > 0)
            {
                var movingUnitId = gameAction.UnitId;
                if (blockedUnitIds.Contains(movingUnitId))
                {
                    continue;
                }

                var desiredDestinationPosition = gameAction.moveToPositions.Last();
                var movingUnit = allUnitsById[movingUnitId];

                var isDestinationBlocked = allUnitsById.Values.Any(
                    unit =>
                        unit.unitId != movingUnitId &&
                        unit.position.x == desiredDestinationPosition.x &&
                        unit.position.y == desiredDestinationPosition.y);

                if (isDestinationBlocked)
                {
                    blockedUnitIds.Add(movingUnitId);
                    continue;
                }

                HandleUnitLookingAt(gameActionResults, movingUnit, desiredDestinationPosition);

                gameActionResults.Add(new GameMoveResultAction(movingUnitId, gameAction.moveToPositions));
                movingUnit.position = desiredDestinationPosition;
            }
        }
    }

    private static void HandleUnitLookingAt(List<GameResultAction> gameActionResults, Unit movingUnit, Position desiredDestinationPosition)
    {
        if (desiredDestinationPosition.x < movingUnit.position.x && movingUnit.facingDirection != Unit.Direction.Left)
        {
            movingUnit.facingDirection = Unit.Direction.Left;
            gameActionResults.Add(new GameRotateResultAction(movingUnit.unitId, Unit.Direction.Left));
        }
        else if (desiredDestinationPosition.x > movingUnit.position.x && movingUnit.facingDirection != Unit.Direction.Right)
        {
            movingUnit.facingDirection = Unit.Direction.Right;
            gameActionResults.Add(new GameRotateResultAction(movingUnit.unitId, Unit.Direction.Right));
        }
    }

    private List<Unit[]> DetermineBattlePairings(List<Unit> allUnits)
    {
        var possibleBattlePairings = (
            from leftUnit in allUnits
            from rightUnit in allUnits
            where leftUnit.owningPlayerId != rightUnit.owningPlayerId && leftUnit.unitId < rightUnit.unitId
            select new[] {leftUnit, rightUnit}).ToList();

        // FIXME: sorting should be based on all affected units
        possibleBattlePairings.Sort((pairingA, pairingB) => pairingA[0].CompareTo(pairingB[0]));

        return possibleBattlePairings;
    }

    private void ExecuteBattleForPairing(Unit[] battlePairing, List<GameResultAction> gameActionResults)
    {
        var leftUnit = battlePairing[0];
        var rightUnit = battlePairing[1];
        var leftOldHitpoints = leftUnit.healthPoints;
        var rightOldHitpoints = rightUnit.healthPoints;

        if (leftUnit.healthPoints <= 0 || rightUnit.healthPoints <= 0)
        {
            return;
        }

        var takenDamage = CalculateDamage(leftUnit, rightUnit);
        var leftDamage = takenDamage[0];
        var rightDamage = takenDamage[1];
        leftUnit.healthPoints = Math.Max(0, leftUnit.healthPoints - leftDamage);
        rightUnit.healthPoints = Math.Max(0, rightUnit.healthPoints - rightDamage);

        if (leftDamage > 0)
        {
            gameActionResults.Add(new GameAttackResultAction(rightUnit.unitId, leftUnit.position));
            HandleUnitLookingAt(gameActionResults, leftUnit, rightUnit.position);
            HandleUnitLookingAt(gameActionResults, rightUnit, leftUnit.position);
        }
        if (rightDamage > 0)
        {
            gameActionResults.Add(new GameAttackResultAction(leftUnit.unitId, rightUnit.position));
            HandleUnitLookingAt(gameActionResults, leftUnit, rightUnit.position);
            HandleUnitLookingAt(gameActionResults, rightUnit, leftUnit.position);
        }
        if (leftDamage > 0)
            gameActionResults.Add(new GameHitpointChangeResultAction(leftUnit.unitId, leftOldHitpoints, leftUnit.healthPoints));
        if (rightDamage > 0)
            gameActionResults.Add(new GameHitpointChangeResultAction(rightUnit.unitId, rightOldHitpoints, rightUnit.healthPoints));
        if (leftUnit.healthPoints <= 0)
            gameActionResults.Add(new GameUnitDeathResultAction(leftUnit.unitId));
        if (rightUnit.healthPoints <= 0)
            gameActionResults.Add(new GameUnitDeathResultAction(rightUnit.unitId));
    }

    private List<List<GameAction>> GetGameActionsPerTurn(Dictionary<int, Unit> allUnitsById)
    {
        var actionCount = CurrentGameState.PlayerGameActions
            .Select(actions => actions.Count)
            .Max();

        var gameActionsByTurn = new List<List<GameAction>>();
        for (var turnNumber = 0; turnNumber < actionCount; turnNumber++)
        {
            // collect all player actions and resolve any collision
            var gameActions = CollectGameActionsForTurn(turnNumber);
            gameActions.Sort((action1, action2) =>
                    allUnitsById[action1.UnitId].CompareTo(allUnitsById[action2.UnitId]));

            gameActionsByTurn.Add(gameActions);
        }
        return gameActionsByTurn;
    }

    private List<GameAction> CollectGameActionsForTurn(int turnNumber)
    {
        var gameActions = new List<GameAction>();
        for (var playerId = 0; playerId < CurrentGameState.PlayerCount; playerId++)
        {
            var gameActionsFromPlayer = CurrentGameState.PlayerGameActions[playerId];
            if (turnNumber < gameActionsFromPlayer.Count)
            {
                gameActions.Add(gameActionsFromPlayer[turnNumber]);
            }
        }
        return gameActions;
    }

    public int[] CalculateDamage(Unit fighterLeft, Unit fighterRight)
    {
        var result = new int[2] {0, 0};

        if (fighterLeft.healthPoints <= 0 || fighterRight.healthPoints <= 0)
        {
            return result;
        }

        result[1] = Math.Max(
            (int)Math.Ceiling(
                GetAttackStrengthAtPosition(
                fighterLeft.Definition.attackPattern,
                fighterLeft.position,
                fighterRight.position)
                * 1.0f - (fighterRight.Definition.DefenseAgainst(fighterLeft.Definition.type) / 10.0)
                ),
            0);

        result[0] = Math.Max(
            (int)Math.Ceiling(
                GetAttackStrengthAtPosition(
                fighterRight.Definition.attackPattern,
                fighterRight.position,
                fighterLeft.position)
                * 1.0f - (fighterLeft.Definition.DefenseAgainst(fighterLeft.Definition.type) / 10.0)
                ),
            0);

        return result;
    }

    public int GetAttackStrengthAtPosition(AttackPatternDefinition pattern, Position pos1, Position pos2)
    {
        int distX = pos1.x - pos2.x + pattern.Width / 2;
        int distY = pos1.y - pos2.y + pattern.Height / 2;

        if (distX < 0 || distX >= pattern.Width || distY < 0 || distY >= pattern.Height)
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
