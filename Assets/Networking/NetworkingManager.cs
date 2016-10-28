using UnityEngine;
using UnityEngine.Networking;

public delegate void GameStateReceiver(GameState gameState);

public class NetworkingManager : MonoBehaviour
{
    private const int GamePort = 9874;
    private const short MsgTypeGameStateC2S = 10000;
    private const short MsgTypeGameStateS2C = 10001;

    public event GameStateReceiver StateReceivers;

    private NetworkClient _client;

    public void StartHosting()
    {
        NetworkServer.Listen(GamePort);
        NetworkServer.RegisterHandler(MsgTypeGameStateC2S, OnClientToServerStateMessage);
        _client = ClientScene.ConnectLocalServer();
        RegisterClientHandlers();
    }

    public void ConnectToHost(string hostname)
    {
        _client = new NetworkClient();
        RegisterClientHandlers();
        _client.Connect(hostname, GamePort);
    }

    private void RegisterClientHandlers()
    {
        _client.RegisterHandler(MsgType.Connect, OnConnected);
        _client.RegisterHandler(MsgTypeGameStateS2C, OnServerToClientStateMessage);
    }

    public void OnConnected(NetworkMessage message)
    {
        Debug.Log("connected to server");
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
        var message = networkMessage.ReadMessage<MessageGameState>();
        var sendingClientId = networkMessage.conn.connectionId;

        foreach (var connection in NetworkServer.connections)
        {
            var connectionId = connection.connectionId;
            if (connectionId != sendingClientId)
            {
                NetworkServer.SendToClient(connectionId, MsgTypeGameStateS2C, message);
            }
        }
    }
}
