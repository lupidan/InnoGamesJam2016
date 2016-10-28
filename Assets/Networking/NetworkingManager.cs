using UnityEngine;
using UnityEngine.Networking;

public delegate void GameStateReceivedHandler(GameState gameState);

public delegate void ClientConnectionErrorHandler();

public delegate void ConnectedToServerHandler();

public class NetworkingManager : MonoBehaviour
{
    private const int GamePort = 9874;
    private const short MsgTypeGameStateC2S = 10000;
    private const short MsgTypeGameStateS2C = 10001;

    public event GameStateReceivedHandler StateReceivers;
    public event ClientConnectionErrorHandler ErrorHandlers;
    public event ConnectedToServerHandler ConnectedHandler;

    private NetworkClient _client;
    private GameState _lastSeenGameState;

    public void StartHosting()
    {
        InitializeGameState();
        InitializeServer();
        InitializeLocalClient();
    }

    private void InitializeLocalClient()
    {
        _client = ClientScene.ConnectLocalServer();
        RegisterClientHandlers();
    }

    private void InitializeServer()
    {
        NetworkServer.Listen(GamePort);
        NetworkServer.RegisterHandler(MsgTypeGameStateC2S, OnClientToServerStateMessage);
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
    }

    private void InitializeGameState()
    {
        _lastSeenGameState = new GameState();
    }

    public void ConnectToHost(string hostname)
    {
        _client = new NetworkClient();
        RegisterClientHandlers();
        _client.Connect(hostname, GamePort);
    }

    private void RegisterClientHandlers()
    {
        _client.RegisterHandler(MsgType.Connect, OnServerConnected);
        _client.RegisterHandler(MsgType.Error, OnClientNetworkError);
        _client.RegisterHandler(MsgType.Disconnect, OnClientNetworkError);
        _client.RegisterHandler(MsgTypeGameStateS2C, OnServerToClientStateMessage);
    }

    public void OnServerConnected(NetworkMessage message)
    {
        if (ConnectedHandler != null)
        {
            ConnectedHandler();
        }
    }

    public void OnClientConnected(NetworkMessage message)
    {
        if (_lastSeenGameState != null)
        {
            message.conn.Send(MsgTypeGameStateS2C, new MessageGameState(_lastSeenGameState));
        }
    }

    public void SendState(GameState gameState)
    {
        var message = new MessageGameState(gameState);
        _client.Send(MsgTypeGameStateC2S, message);
    }

    private void OnServerToClientStateMessage(NetworkMessage networkMessage)
    {
        var message = networkMessage.ReadMessage<MessageGameState>();
        if (StateReceivers != null)
        {
            StateReceivers(message.ToGameState());
        }
    }

    private void OnClientToServerStateMessage(NetworkMessage networkMessage)
    {
        var gameStateMessage = networkMessage.ReadMessage<MessageGameState>();
        var sendingClientId = networkMessage.conn.connectionId;

        _lastSeenGameState = gameStateMessage.ToGameState();
        foreach (var connection in NetworkServer.connections)
        {
            var connectionId = connection.connectionId;
            if (connectionId != sendingClientId)
            {
                NetworkServer.SendToClient(connectionId, MsgTypeGameStateS2C, gameStateMessage);
            }
        }
    }

    private void OnClientNetworkError(NetworkMessage networkMessage)
    {
        if (ErrorHandlers != null)
        {
            ErrorHandlers();
        }
    }
}
