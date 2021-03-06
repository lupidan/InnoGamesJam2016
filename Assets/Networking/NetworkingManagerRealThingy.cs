﻿using UnityEngine;

public class NetworkingManagerRealThingy : MonoBehaviour {

    private bool _isAtStartup = true;
    private bool _isConnected;
    private bool _isWaiting;

    private bool _amIHost;
    private bool _isPlayingSolo;

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
        if (hostAddressKeeper == null)
        {
            _isPlayingSolo = true;
        }
        else
        {
            _amIHost = hostAddressKeeper.AmIHost();
            if (!_amIHost)
            {
                ServerHostname = hostAddressKeeper.HostAddress;
            }
            Destroy(hostAddressKeeper);
        }

        Debug.Log("Awaked!");
    }

    void OnConnectionError()
    {
        _isAtStartup = true;
        _isConnected = false;
        Debug.Log("Connection error");
    }

    void OnStateChanged()
    {
        _isWaiting = false;
        Debug.Log("State changed");
    }

    void OnConnected()
    {
        _isConnected = true;
        Debug.Log("Connected!");
    }

    void Update()
    {
        if (_isAtStartup)
        {
            if (_isPlayingSolo)
            {
                Server.StartSinglePlayerServer();
                Client.ConnectLocally();
                _isAtStartup = false;
            }
            else if (_amIHost)
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
