using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private List<GameResultAction> _resultActionQueue;

    public void ReceivedNewGameState(GameState gameState)
    {
        if (CurrentServerSideState == null)
        {
            //LevelGenerator generator = new LevelGenerator();
            //generator

        }
        CurrentServerSideState = gameState;
        CurrentVisibleState = GameState.Clone(gameState);
        _resultActionQueue = new List<GameResultAction>(CurrentServerSideState.ResultsFromLastPhase);

        // invoke the first animation which will go through the queue via callbacks
        PlayNextAnimation();

        Debug.LogError("New game state phase is " + gameState.CurrentPhase);
        if (StateUpdatedHandler != null)
        {
            StateUpdatedHandler();
        }
    }

    // Helper to retrieve a unit controller by its unitID from the scene
    private UnitController FindUnitControllerById(int unitId)
    {
        return GameObject.Find(Unit.UnitControllerNameForId(unitId)).GetComponent<UnitController>();
    }


    // Play the next animation on any UnitController or inform the game that the phase is finished
    public void PlayNextAnimation()
    {
        if (_resultActionQueue.Count == 0)
        {
            // FIXME: inform UI about finished phase
            return;
        }

        var resultAction = _resultActionQueue[0];
        var unitController = FindUnitControllerById(resultAction.unitId);

        if (resultAction is GameMoveResultAction)
        {
            var moveAction = (GameMoveResultAction) resultAction;
            var toPosition = moveAction.movements[0];
            moveAction.movements.RemoveAt(0);
            if (moveAction.movements.Count == 0)
            {
                RemoveFirstActionQueueElement();
            }
            unitController.PlayMoveAnimation(toPosition, PlayNextAnimation);
        } else if (resultAction is GameRotateResultAction)
        {
            RemoveFirstActionQueueElement();
            var direction = ((GameRotateResultAction) resultAction).direction;
            unitController.PlayRotateAnimation(direction, PlayNextAnimation);
        } else if (resultAction is GameAttackResultAction)
        {
            RemoveFirstActionQueueElement();
            var targetPosition = ((GameAttackResultAction) resultAction).target;
            unitController.PlayAttackAnimation(targetPosition, PlayNextAnimation);
        } else if (resultAction is GameHitpointChangeResultAction)
        {
            RemoveFirstActionQueueElement();
            var newHitpointValue = ((GameHitpointChangeResultAction) resultAction).newHitpointValue;
            unitController.PlayHitpointChange(newHitpointValue, PlayNextAnimation);
        } else if (resultAction is GameUnitDeathResultAction)
        {
            RemoveFirstActionQueueElement();
            unitController.PlayDeathAnimation(PlayNextAnimation);
        }
    }

    // remove the first element from the result action queue
    private void RemoveFirstActionQueueElement()
    {
        _resultActionQueue.RemoveAt(0);
    }
}
