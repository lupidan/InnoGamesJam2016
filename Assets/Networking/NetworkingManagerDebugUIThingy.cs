using UnityEngine;
using UnityEngine.Assertions;

public class NetworkingManagerDebugUIThingy : MonoBehaviour
{
    private bool _isAtStartup = true;
    private bool _isConnected = false;
    private NetworkingManager _networkingManager;

    public GameState GameState;
    public string ServerHostname = "127.0.0.1";

    void Awake()
    {
        _networkingManager = GetComponent<NetworkingManager>();
        Assert.IsNotNull(_networkingManager);

        _networkingManager.StateReceivers += OnStateReceived;
        _networkingManager.ErrorHandlers += OnConnectionError;
        _networkingManager.ConnectedHandler += OnConnected;
    }

    void OnConnectionError()
    {
        _isAtStartup = true;
        _isConnected = false;
    }

    void OnStateReceived(GameState gameState)
    {
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
                _networkingManager.StartHosting();
                _isAtStartup = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _networkingManager.ConnectToHost(ServerHostname);
                _isAtStartup = false;
            }
        }
        else if (_isConnected)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                switch (GameState.CurrentState)
                {
                    case GameEngineState.ServerWaiting:
                        GameState.CurrentState = GameEngineState.P1Moving;
                        break;
                    case GameEngineState.P1Moving:
                        GameState.CurrentState = GameEngineState.P2Moving;
                        break;
                    case GameEngineState.P2Moving:
                        GameState.CurrentState = GameEngineState.P1Revising;
                        break;
                    case GameEngineState.P1Revising:
                        GameState.CurrentState = GameEngineState.P2Revising;
                        break;
                    case GameEngineState.P2Revising:
                        GameState.CurrentState = GameEngineState.P1Moving;
                        break;
                }
                _networkingManager.SendState(GameState);
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
        else if (_isConnected)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press P to advance game state");
        }
        else
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Connecting...");
        }
    }
}
