using System.Collections;
using UnityEngine.Networking;

public class BaseAI : NetworkBehaviour
{
    public virtual void AttackedBy(BaseEntity attacker)
    {
    }
}