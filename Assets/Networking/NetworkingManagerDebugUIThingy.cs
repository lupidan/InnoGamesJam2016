using UnityEngine;
using UnityEngine.Assertions;

public class NetworkingManagerDebugUIThingy : MonoBehaviour
{
    private bool _isAtStartup = true;
    private NetworkingManager _networkingManager;
    public GameState GameState;
    public string serverHostname = "127.0.0.1";

    void Awake()
    {
        _networkingManager = GetComponent<NetworkingManager>();
        Assert.IsNotNull(_networkingManager);

        _networkingManager.StateReceivers += OnStateReceived;
    }

    void OnStateReceived(GameState gameState)
    {
        GameState = gameState;
        Debug.LogError("Received game state " + GameState.CurrentState);
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
                _networkingManager.ConnectToHost(serverHostname);
                _isAtStartup = false;
            }
        }
        else
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
            GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
        }
        else
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press P to advance game state");
        }
    }
}
