using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public delegate void StateUpdatedHandler();

public delegate void PlayerMayInteractHandler();

/// <summary>
/// Basic game logic manager for the client-side behaviour,
/// allowing for simulation and holding of a game state
/// </summary>
/// [RequireComponent(typeof(Animator))]
///
[RequireComponent(typeof(MapGenerator))]
public class ClientGameLogicManager : MonoBehaviour
{
    public GameState CurrentServerSideState;

    public event StateUpdatedHandler StateUpdatedHandler;
    public event PlayerMayInteractHandler PlayerMayInteractHandler;

    private List<GameResultAction> _resultActionQueue;

    public List<GameAction> QueuedGameActions;

    public static ClientGameLogicManager GetClientLogicFromScene()
    {
        return GameObject.Find("NetworkClient").GetComponent<ClientGameLogicManager>();
    }

    public void Start()
    {
        ClearQueuedActions();
    }

    public void ClearQueuedActions()
    {
        QueuedGameActions = new List<GameAction>();
    }

    public void RemoveQueuedActionForUnitId(int unitId)
    {
        var toDelete = QueuedGameActions.Where(action => action.UnitId == unitId).ToList();
        foreach (var action in toDelete)
        {
            QueuedGameActions.Remove(action);
        }
    }

    public void ReceivedNewGameState(GameState gameState)
    {
        CurrentServerSideState = gameState;
        _resultActionQueue = new List<GameResultAction>(CurrentServerSideState.ResultsFromLastPhase);
        ClearQueuedActions();

        // Update unit controllers with new game state
        foreach (Player player in CurrentServerSideState.players.Values)
        {
            foreach (Unit unit in player.units.Values)
            {
                var unitController = FindUnitControllerById(unit.unitId);
                unitController.unitData = unit;
            }
        }

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
            if (PlayerMayInteractHandler != null)
            {
                PlayerMayInteractHandler();
            }
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
        }
        else if (resultAction is GameRotateResultAction)
        {
            RemoveFirstActionQueueElement();
            var direction = ((GameRotateResultAction) resultAction).direction;
            unitController.PlayRotateAnimation(direction, PlayNextAnimation);
        }
        else if (resultAction is GameAttackResultAction)
        {
            RemoveFirstActionQueueElement();
            var targetPosition = ((GameAttackResultAction) resultAction).target;
            unitController.PlayAttackAnimation(targetPosition, PlayNextAnimation);
        }
        else if (resultAction is GameHitpointChangeResultAction)
        {
            RemoveFirstActionQueueElement();
            var oldHitpointValue = ((GameHitpointChangeResultAction) resultAction).oldHitpointValue;
            var newHitpointValue = ((GameHitpointChangeResultAction) resultAction).newHitpointValue;
            unitController.PlayHitpointChange(oldHitpointValue, newHitpointValue, PlayNextAnimation);
        }
        else if (resultAction is GameUnitDeathResultAction)
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

    public void AddQueuedActionForUnitId(int unitDataUnitId, List<Position> pathToDestination)
    {
        QueuedGameActions.Add(new GameAction(unitDataUnitId, pathToDestination));
    }
}