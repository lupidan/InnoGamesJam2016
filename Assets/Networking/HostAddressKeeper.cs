using UnityEngine;

public class HostAddressKeeper : MonoBehaviour
{

    public string HostAddress;

    public bool AmIHost()
    {
        return HostAddress == null;
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
