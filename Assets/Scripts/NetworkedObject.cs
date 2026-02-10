using UnityEngine;

public abstract class NetworkedObject : MonoBehaviour
{
    // Called when the object is initialized over the network
    public abstract void Initialize();

    // Returns a unique ID used to identify this object over the network
    public abstract int GetNetworkId();
}
