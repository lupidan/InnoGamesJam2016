using UnityEngine;

public class HostAddressKeeper : MonoBehaviour
{

    public string HostAddress;

    public bool AmIHost()
    {
        return string.IsNullOrEmpty(HostAddress);
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
