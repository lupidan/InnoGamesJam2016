using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkingManager : MonoBehaviour
{
    private GameState _lastSeenGameState;

    public void StartServer()
    {
        InitializeGameState();
        InitializeServer();
    }

    private void InitializeServer()
    {
        NetworkServer.Listen(NetworkingConstants.GamePort);
        NetworkServer.RegisterHandler(NetworkingConstants.MsgTypeGameStateC2S, OnClientToServerStateMessage);
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
    }

    private void InitializeGameState()
    {
        _lastSeenGameState = new GameState();
    }

    public void OnClientConnected(NetworkMessage message)
    {
        if (_lastSeenGameState != null)
        {
            message.conn.Send(NetworkingConstants.MsgTypeGameStateS2C, new MessageGameState(_lastSeenGameState));
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
                NetworkServer.SendToClient(connectionId, NetworkingConstants.MsgTypeGameStateS2C, gameStateMessage);
            }
        }
    }
}
