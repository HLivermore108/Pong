using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SpriteRenderer))]
public class NetworkBallColor : NetworkBehaviour
{
    [SerializeField] private Color colorA = Color.blue;
    [SerializeField] private Color colorB = Color.red;

    private SpriteRenderer sr;

    private NetworkVariable<bool> useColorA = new NetworkVariable<bool>(
        true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        ApplyColor(useColorA.Value);
        useColorA.OnValueChanged += (_, newValue) => ApplyColor(newValue);
    }

    private void ApplyColor(bool a)
    {
        sr.color = a ? colorA : colorB;
    }

    // Called by server only
    public void Toggle()
    {
        if (!IsServer) return;
        useColorA.Value = !useColorA.Value;
    }
}
