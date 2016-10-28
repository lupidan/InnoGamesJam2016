using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Assertions;
using UnityEngine.Networking;

// TODO: unify with MessageGameState

public class MessageGameActions : MessageBase
{
    public byte[] SerializedGameState;

    private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

    public MessageGameActions()
    {
    }

    public MessageGameActions(GameActions gameActions)
    {
        var memoryStream = new MemoryStream();
        BinaryFormatter.Serialize(memoryStream, gameActions);
        SerializedGameState = memoryStream.ToArray();
    }

    public GameActions ToGameActions()
    {
        var memoryStream = new MemoryStream(SerializedGameState);
        var gameActions = BinaryFormatter.Deserialize(memoryStream) as GameActions;
        Assert.IsNotNull(gameActions, "GameActions couldn't be deserialized");

        return gameActions;
    }
}