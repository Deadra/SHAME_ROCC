using UnityEngine;
using UnityEngine.Networking;

public class DestroyAtferTime : NetworkBehaviour
{
    [SerializeField] private float lifetime;

    void Start()
    {
        Destroy(this.gameObject, lifetime);
    }
}