using UnityEngine;
using System.Collections;

public delegate void StateUpdatedHandler();

/// <summary>
/// Basic game logic manager for the client-side behaviour,
/// allowing for simulation and holding of a game state
/// </summary>
public class ClientGameLogicManager : MonoBehaviour
{
    public GameState CurrentServerSideState;
    public GameState CurrentVisibleState;

    public event StateUpdatedHandler StateUpdatedHandler;

    public void ReceivedNewGameState(GameState gameState)
    {
        CurrentServerSideState = gameState;
        CurrentVisibleState = GameState.Clone(gameState);

        Debug.LogError("New game state phase is " + gameState.CurrentPhase);

        if (StateUpdatedHandler != null)
        {
            StateUpdatedHandler();
        }
    }
}
