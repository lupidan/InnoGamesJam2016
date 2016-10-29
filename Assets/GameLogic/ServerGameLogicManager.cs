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

    public void InitializeNewGame()
    {
        CurrentGameState = new GameState();
    }

    public void ReceivedActionsFromPlayer(int playerId, List<GameAction> actions)
    {
        CurrentGameState.PendingPlayerIDs.Remove(playerId);

        if (!CurrentGameState.AllPlayersMoved())
        {
            return;
        }

        EvaluatePlayerMovedAndAdvance();
        if (CurrentGameState.CurrentPhase != GamePhase.Finished)
        {
            CurrentGameState.RefreshAllPlayersPending();
        }

        GameStateHasUpdated();
    }

    private void EvaluatePlayerMovedAndAdvance()
    {
        // TODO: some game handling here :-)
        switch (CurrentGameState.CurrentPhase)
        {
            case GamePhase.WaitingForStart:
                CurrentGameState.CurrentPhase = GamePhase.Planning;
                break;
            case GamePhase.Planning:
                CurrentGameState.CurrentPhase = GamePhase.Revision;
                break;
            case GamePhase.Revision:
                CurrentGameState.CurrentPhase = new System.Random().Next() % 4 == 0
                    ? GamePhase.Finished
                    : GamePhase.Planning;
                break;
        }
    }

    private void GameStateHasUpdated()
    {
        if (UpdateHandlers != null)
        {
            UpdateHandlers();
        }
    }
}
