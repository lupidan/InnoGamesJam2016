using UnityEngine;
using System.Collections;

public class IngameSubmitButtonManager : MonoBehaviour {

    public void SubmitTurn()
    {
        var actions = ClientGameLogicManager.GetClientLogicFromScene().QueuedGameActions;
        ClientNetworkingManager.GetClientNetworkingManager().SendActions(actions);

        UnitController[] unitControllers = FindObjectsOfType<UnitController>();
        for (int i = 0; i < unitControllers.Length; i++)
        {
            unitControllers[i].SetDestinationTileController(null);
        }
    }

}
