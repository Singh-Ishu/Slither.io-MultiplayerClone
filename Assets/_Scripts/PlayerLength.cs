using Unity.Netcode;
using UnityEngine;

public class PlayerLength : NetworkBehaviour
{
    public NetworkVariable<ushort> length = new(1);

}
