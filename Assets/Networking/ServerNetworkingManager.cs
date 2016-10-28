using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkingManager : MonoBehaviour
{
    private GameState _currentGameState;

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
        NetworkServer.RegisterHandler(NetworkingConstants.MsgTypeGameActionsC2S, OnGameActionsFromClient);
    }

    private void InitializeGameState()
    {
        _currentGameState = new GameState();
    }

    public void OnClientConnected(NetworkMessage message)
    {
        if (_currentGameState != null)
        {
            message.conn.Send(NetworkingConstants.MsgTypeGameStateS2C, new MessageGameState(_currentGameState));
        }
    }

    private void OnClientToServerStateMessage(NetworkMessage networkMessage)
    {
        var gameStateMessage = networkMessage.ReadMessage<MessageGameState>();
        var sendingClientId = networkMessage.conn.connectionId;

        _currentGameState = gameStateMessage.ToGameState();
        foreach (var connection in NetworkServer.connections)
        {
            var connectionId = connection.connectionId;
            if (connectionId != sendingClientId)
            {
                NetworkServer.SendToClient(connectionId, NetworkingConstants.MsgTypeGameStateS2C, gameStateMessage);
            }
        }
    }

    private void InformClientsAboutGameState()
    {
        var gameStateMessage = new MessageGameState(_currentGameState);
        NetworkServer.SendToAll(NetworkingConstants.MsgTypeGameStateS2C, gameStateMessage);
    }

    private void OnGameActionsFromClient(NetworkMessage networkMessage)
    {
        var gameActionsMessage = networkMessage.ReadMessage<MessageGameActions>();
        var gameActions = gameActionsMessage.ToGameActions();

        // FIXME: this is no proper handling or place for this :-)
        _currentGameState.PendingPlayerIDs.Remove(gameActions.playerID);

        if (_currentGameState.MayAdvanceState())
        {
            switch (_currentGameState.CurrentState)
            {
                case GameEngineState.ServerWaiting:
                    _currentGameState.CurrentState = GameEngineState.Planning;
                    break;
                case GameEngineState.Planning:
                    _currentGameState.CurrentState = GameEngineState.Revision;
                    break;
                case GameEngineState.Revision:
                    if (new System.Random().Next(5) == 0)
                    {
                        _currentGameState.CurrentState = GameEngineState.End;
                    }
                    else
                    {
                        _currentGameState.CurrentState = GameEngineState.Planning;
                    }
                    break;
            }
            _currentGameState.RefreshAllPlayersPending();
            InformClientsAboutGameState();
        }
    }
}
