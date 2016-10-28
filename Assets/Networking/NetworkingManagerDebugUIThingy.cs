using UnityEngine;
using UnityEngine.Assertions;

public class NetworkingManagerDebugUIThingy : MonoBehaviour
{
    private bool _isAtStartup = true;
    private NetworkingManager _networkingManager;

    void Awake()
    {
        _networkingManager = GetComponent<NetworkingManager>();
        Assert.IsNotNull(_networkingManager);

        _networkingManager.StateReceivers += OnStateReceived;
    }

    void OnStateReceived(GameState gameState)
    {
        Debug.LogError("Received game state");
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
                _networkingManager.ConnectToHost("127.0.0.1");
                _isAtStartup = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _networkingManager.SendState(new GameState());
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
            GUI.Label(new Rect(2, 10, 150, 100), "Press P to pend");
        }
    }

    void OnGameState(GameState newGameState)
    {
        Debug.LogError("Such state, much wow!");

    }
}
