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

    public void ConnectLocally()
    {
        _client = ClientScene.ConnectLocalServer();
        RegisterClientHandlers();
    }

    public void ConnectToHost(string hostname)
    {
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

    public void SendState(GameState gameState)
    {
        var message = new MessageGameState(gameState);
        _client.Send(NetworkingConstants.MsgTypeGameStateC2S, message);
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
