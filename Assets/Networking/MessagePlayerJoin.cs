using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Assertions;
using UnityEngine.Networking;

internal class MessagePlayerJoin : MessageBase
{
    public int PlayerId;

    public MessagePlayerJoin()
    {
    }

    public MessagePlayerJoin(int playerId)
    {
        PlayerId = playerId;
    }
}