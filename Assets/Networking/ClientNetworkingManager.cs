using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void ClientConnectionErrorHandler();

public delegate void ConnectedToServerHandler();

public class ClientNetworkingManager : MonoBehaviour
{
    public event ClientConnectionErrorHandler ErrorHandlers;
    public event ConnectedToServerHandler ConnectedHandler;

    private NetworkClient _client;
    private int _playerId;
    private ClientGameLogicManager _gameLogic;

    public void Awake()
    {
        _gameLogic = GetComponent<ClientGameLogicManager>();
    }

    public void ConnectLocally()
    {
        _playerId = 0;
        _client = ClientScene.ConnectLocalServer();
        RegisterClientHandlers();
    }

    public void ConnectToHost(string hostname)
    {
        _playerId = 1;
        _client = new NetworkClient();
        RegisterClientHandlers();
        _client.Connect(hostname, NetworkingConstants.GamePort);
    }

    private void RegisterClientHandlers()
    {
        _client.RegisterHandler(MsgType.Connect, OnServerConnected);
        _client.RegisterHandler(MsgType.Error, OnClientNetworkError);
        _client.RegisterHandler(MsgType.Disconnect, OnClientNetworkError);
        _client.RegisterHandler(NetworkingConstants.MsgTypeGameStateS2C, OnServerToClientStateMessage);
    }

    public void OnServerConnected(NetworkMessage message)
    {
        _client.Send(NetworkingConstants.MsgTypeGameJoin, new MessagePlayerJoin(_playerId));

        if (ConnectedHandler != null)
        {
            ConnectedHandler();
        }
    }

    public void SendActions(List<GameAction> actions)
    {
        var gameActions = new GameActions
        {
            actions = actions,
            playerID = _playerId
        };

        var message = new MessageGameActions(gameActions);
        _client.Send(NetworkingConstants.MsgTypeGameActionsC2S, message);
    }

    private void OnServerToClientStateMessage(NetworkMessage networkMessage)
    {
        var message = networkMessage.ReadMessage<MessageGameState>();
        _gameLogic.ReceivedNewGameState(message.ToGameState());
    }

    private void OnClientNetworkError(NetworkMessage networkMessage)
    {
        if (ErrorHandlers != null)
        {
            ErrorHandlers();
        }
    }
}