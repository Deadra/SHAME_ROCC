using UnityEngine;

public class DestroyBaseEntity : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponentInParentAndChildren<BaseEntity>())
            col.gameObject.GetComponentInParentAndChildren<BaseEntity>().TakeDeadlyDamage();
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.GetComponentInParentAndChildren<BaseEntity>())
            col.gameObject.GetComponentInParentAndChildren<BaseEntity>().TakeDeadlyDamage();
    }
}