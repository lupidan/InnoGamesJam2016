using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NetworkingManagerDebugUIThingy : MonoBehaviour
{
    private bool _isAtStartup = true;
    private bool _isConnected = false;
    private bool _isWaiting = false;

    public ServerNetworkingManager Server;
    public ClientNetworkingManager Client;
    public ClientGameLogicManager GameLogic;

    public string ServerHostname = "127.0.0.1";

    void Awake()
    {
        GameLogic.StateUpdatedHandler += OnStateChanged;
        Client.ErrorHandlers += OnConnectionError;
        Client.ConnectedHandler += OnConnected;
    }

    void OnConnectionError()
    {
        _isAtStartup = true;
        _isConnected = false;
    }

    void OnStateChanged()
    {
        _isWaiting = false;
    }

    void OnConnected()
    {
        _isConnected = true;
    }

    void Update()
    {
        if (_isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Server.StartServer();
                Client.ConnectLocally();
                _isAtStartup = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Client.ConnectToHost(ServerHostname);
                _isAtStartup = false;
            }
        }
        else if (_isConnected && !_isWaiting)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _isWaiting = true;
                Client.SendActions(new List<GameAction>());
            }
        }
    }

    void OnGUI()
    {
        if (_isAtStartup)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press S for server");
            GUI.Label(new Rect(2, 30, 150, 100), "Press C for client");
        }
        else if (_isConnected && _isWaiting)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Waiting for other player...");
        }
        else if (_isConnected && !_isWaiting)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press P to advance game state");
        }
        else
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Connecting...");
        }
    }
}
