using UnityEngine;

public class NetworkingManagerRealThingy : MonoBehaviour {

    private bool _isAtStartup = true;
    private bool _isConnected;
    private bool _isWaiting;

    private bool _amIHost;

    public ServerNetworkingManager Server;
    public ClientNetworkingManager Client;
    public ClientGameLogicManager GameLogic;

    public string ServerHostname = "127.0.0.1";

    void Awake()
    {
        GameLogic.StateUpdatedHandler += OnStateChanged;
        Client.ErrorHandlers += OnConnectionError;
        Client.ConnectedHandler += OnConnected;

        // Find connector on scene
        var hostAddressKeeper = FindObjectOfType<HostAddressKeeper>();
        _amIHost = hostAddressKeeper.AmIHost();
        if (_amIHost)
        {
            ServerHostname = hostAddressKeeper.HostAddress;
        }
        Destroy(hostAddressKeeper);
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
            if (_amIHost)
            {
                Server.StartServer();
                Client.ConnectLocally();
                _isAtStartup = false;
            }
            else
            {
                Client.ConnectToHost(ServerHostname);
                _isAtStartup = false;
            }
        }
    }

    void OnGUI()
    {
        if (_isConnected && _isWaiting)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Waiting for other player...");
        }
        else
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Connecting...");
        }
    }
}
