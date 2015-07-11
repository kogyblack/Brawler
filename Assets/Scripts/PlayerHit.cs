using UnityEngine;
using UnityEngine.Networking;

public class PlayerHit : NetworkBehaviour {
    [SyncVar]
    public float hitpoints = 0f;
}
