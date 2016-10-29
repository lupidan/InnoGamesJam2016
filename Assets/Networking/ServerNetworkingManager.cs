using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkingManager : MonoBehaviour
{
    private ServerGameLogicManager _gameLogic;

    public void Awake()
    {
        _gameLogic = GetComponent<ServerGameLogicManager>();
        _gameLogic.UpdateHandlers += OnGameStateUpdated;
    }

    public void StartServer()
    {
        _gameLogic.InitializeNewGame();
        InitializeServer();
    }

    public void StartSinglePlayerServer()
    {
        _gameLogic.InitializeNewSinglePlayerGame();
        InitializeServer();
    }

    private void InitializeServer()
    {
        NetworkServer.Listen(NetworkingConstants.GamePort);
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
        NetworkServer.RegisterHandler(NetworkingConstants.MsgTypeGameActionsC2S, OnGameActionsFromClient);
        NetworkServer.RegisterHandler(NetworkingConstants.MsgTypeGameJoin, OnPlayerJoin);
    }

    public void OnPlayerJoin(NetworkMessage message)
    {
        if (_gameLogic.HasGameStarted)
        {
            message.conn.Disconnect();
            return;
        }

        var joinMessage = message.ReadMessage<MessagePlayerJoin>();
        _gameLogic.PlayerHasJoined(joinMessage.PlayerId);
    }

    public void OnClientConnected(NetworkMessage message)
    {
        message.conn.Send(NetworkingConstants.MsgTypeGameStateS2C, new MessageGameState(_gameLogic.CurrentGameState));
    }

    private void OnGameActionsFromClient(NetworkMessage networkMessage)
    {
        var gameActionsMessage = networkMessage.ReadMessage<MessageGameActions>();
        var gameActions = gameActionsMessage.ToGameActions();
        _gameLogic.ReceivedActionsFromPlayer(gameActions.playerID, gameActions.actions);
    }

    private void OnGameStateUpdated()
    {
        var gameStateMessage = new MessageGameState(_gameLogic.CurrentGameState);
        NetworkServer.SendToAll(NetworkingConstants.MsgTypeGameStateS2C, gameStateMessage);
    }
}
