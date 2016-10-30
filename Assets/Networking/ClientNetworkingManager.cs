using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public delegate void ClientConnectionErrorHandler();

public delegate void ConnectedToServerHandler();

public class ClientNetworkingManager : MonoBehaviour
{
    private const string CurrentPlayerText = "CurrentPlayerText";

    public event ClientConnectionErrorHandler ErrorHandlers;
    public event ConnectedToServerHandler ConnectedHandler;

    private NetworkClient _client;

    private int _playerId;
    public int PlayerId
    {
        get { return _playerId; }

        set
        {
            _playerId = value;
            var currentPlayerText = GameObject.Find(CurrentPlayerText);
            if (currentPlayerText)
            {
                currentPlayerText.GetComponent<Text>().text = "Player " + _playerId;
            }
        }
    }

    private ClientGameLogicManager _gameLogic;

    public void Awake()
    {
        _gameLogic = GetComponent<ClientGameLogicManager>();
    }

    public void ConnectLocally()
    {
        PlayerId = 0;
        _client = ClientScene.ConnectLocalServer();
        RegisterClientHandlers();
    }

    public void ConnectToHost(string hostname)
    {
        Debug.Log("Connecting to " + hostname);
        PlayerId = 1;
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
        Debug.Log("Client connected to server");
        _client.Send(NetworkingConstants.MsgTypeGameJoin, new MessagePlayerJoin(PlayerId));

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
            playerID = PlayerId
        };

        var message = new MessageGameActions(gameActions);
        _client.Send(NetworkingConstants.MsgTypeGameActionsC2S, message);
    }

    private void OnServerToClientStateMessage(NetworkMessage networkMessage)
    {
        Debug.Log("Client received state from server");
        var message = networkMessage.ReadMessage<MessageGameState>();
        _gameLogic.ReceivedNewGameState(message.ToGameState());
    }

    private void OnClientNetworkError(NetworkMessage networkMessage)
    {
        Debug.Log("Client connection error");
        if (ErrorHandlers != null)
        {
            ErrorHandlers();
        }
    }
}