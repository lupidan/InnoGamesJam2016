using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Assertions;
using UnityEngine.Networking;

internal class MessageGameState : MessageBase
{
    public byte[] SerializedGameState;

    private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

    public MessageGameState()
    {
    }

    public MessageGameState(GameState gameState)
    {
        var memoryStream = new MemoryStream();
        BinaryFormatter.Serialize(memoryStream, gameState);
        SerializedGameState = memoryStream.ToArray();
    }

    public GameState ToGameState()
    {
        var memoryStream = new MemoryStream(SerializedGameState);
        var gameState = BinaryFormatter.Deserialize(memoryStream) as GameState;
        Assert.IsNotNull(gameState, "GameState couldn't be deserialized");

        return gameState;
    }
}