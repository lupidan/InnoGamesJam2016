using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void GameStateReceivedHandler(GameState gameState);

public delegate void ClientConnectionErrorHandler();

public delegate void ConnectedToServerHandler();

public class ClientNetworkingManager : MonoBehaviour
{
    public event GameStateReceivedHandler StateReceivers;
    public event ClientConnectionErrorHandler ErrorHandlers;
    public event ConnectedToServerHandler ConnectedHandler;

    private NetworkClient _client;
    private int _playerId;

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
        if (ConnectedHandler != null)
        {
            ConnectedHandler();
        }
    }

    public void SendActions(List<GameAction> actions)
    {
        var gameActions = new GameActions();
        gameActions.actions = actions;
        gameActions.playerID = _playerId;

        var message = new MessageGameActions(gameActions);
        _client.Send(NetworkingConstants.MsgTypeGameActionsC2S, message);
    }

    private void OnServerToClientStateMessage(NetworkMessage networkMessage)
    {
        var message = networkMessage.ReadMessage<MessageGameState>();
        if (StateReceivers != null)
        {
            StateReceivers(message.ToGameState());
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