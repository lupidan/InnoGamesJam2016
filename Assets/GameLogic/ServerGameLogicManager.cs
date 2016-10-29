using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void GameStateUpdatedHandler();

/// <summary>
/// Basic logic manager class for the server-side game logic
/// </summary>
public class ServerGameLogicManager : MonoBehaviour
{
    public GameState CurrentGameState;

    public GameStateUpdatedHandler UpdateHandlers;

    public bool HasGameStarted
    {
        get { return CurrentGameState.CurrentPhase != GamePhase.WaitingForStart; }
    }

    public void InitializeNewGame()
    {
        CurrentGameState = new GameState(2);
    }

    public void InitializeNewSinglePlayerGame()
    {
        CurrentGameState = new GameState(1);
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
        if (CurrentGameState.CurrentPhase != GamePhase.Finished)
        {
            CurrentGameState.ResetPhaseToWaitingForAllPlayers();
        }

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
    }

    private void EvaluateRevisionPhase()
    {
        CurrentGameState = ProcessPlayerActions();
        CurrentGameState.CurrentPhase = GamePhase.Planning;

/*
        CurrentGameState.CurrentPhase = new System.Random().Next() % 4 == 0
            ? GamePhase.Finished
            : GamePhase.Planning;
*/
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

        newGameState.ResultsFromLastPhase = new List<GameResultAction>();

        // DEBUG
        newGameState.ResultsFromLastPhase.Add(new GameAttackResultAction(3, new Position(1, 2)));
        List<Position> positions = new List<Position>();
        positions.Add(new Position(0,0));
        positions.Add(new Position(1,0));
        positions.Add(new Position(2,0));
        positions.Add(new Position(3,0));
        positions.Add(new Position(4,0));
        positions.Add(new Position(5,0));
        positions.Add(new Position(6,0));
        newGameState.ResultsFromLastPhase.Add(new GameMoveResultAction(1, positions));
        newGameState.ResultsFromLastPhase.Add(new GameAttackResultAction(1, new Position(1, 2)));
        newGameState.ResultsFromLastPhase.Add(new GameUnitDeathResultAction(2));
        // END DEBUG

        return newGameState;
    }

    private void GameStateHasUpdated()
    {
        if (UpdateHandlers != null)
        {
            UpdateHandlers();
        }
    }
}
