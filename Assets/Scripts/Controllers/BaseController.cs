using UnityEngine;
using UnityEngine.Networking;

public class BaseController : NetworkBehaviour
{
    protected Rigidbody objectToMove;

    public virtual void Start()
    {
        objectToMove = GetComponent<Rigidbody>();
    }
}