using UnityEngine;
using System.Collections;

public class IngameSubmitButtonManager : MonoBehaviour {

    public void SubmitTurn()
    {
        var actions = ClientGameLogicManager.GetClientLogicFromScene().QueuedGameActions;
        ClientNetworkingManager.GetClientNetworkingManager().SendActions(actions);

        // FIXME: deactivate UI
    }

}
