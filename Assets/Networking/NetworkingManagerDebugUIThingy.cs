using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NetworkingManagerDebugUIThingy : MonoBehaviour
{
    private bool _isAtStartup = true;
    private bool _isConnected = false;
    private bool _isWaiting = false;

    public ServerNetworkingManager _server;
    public ClientNetworkingManager _client;

    public GameState GameState;
    public string ServerHostname = "127.0.0.1";

    void Awake()
    {
        _client.StateReceivers += OnStateReceived;
        _client.ErrorHandlers += OnConnectionError;
        _client.ConnectedHandler += OnConnected;
    }

    void OnConnectionError()
    {
        _isAtStartup = true;
        _isConnected = false;
    }

    void OnStateReceived(GameState gameState)
    {
        _isWaiting = false;
        GameState = gameState;
        Debug.LogError("Received game state " + GameState.CurrentState);
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
                _server.StartServer();
                _client.ConnectLocally();
                _isAtStartup = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _client.ConnectToHost(ServerHostname);
                _isAtStartup = false;
            }
        }
        else if (_isConnected && !_isWaiting)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _isWaiting = true;
                _client.SendActions(new List<GameAction>());
                Debug.LogError("Sent game state " + GameState.CurrentState);
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
