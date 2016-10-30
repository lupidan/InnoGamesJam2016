using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextTurnButton : MonoBehaviour
{
    private ClientNetworkingManager clientNetworkingManager;

    void Start()
    {
        var networkClientGameObject = GameObject.Find("NetworkClient");
        clientNetworkingManager = networkClientGameObject.GetComponent<ClientNetworkingManager>();
    }

    public void GoToNextTurn()
    {
        // List<GameAction> gameActions = new List<GameAction>();
        // clientNetworkingManager.SendActions();
    }
}
