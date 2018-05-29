using UnityEngine;

/// <summary>
/// Рука, способная удерживать предметы
/// </summary>
public class Hand : BaseWeapon {

    bool grabbed = false;
    GameObject touchedObject;
    [SerializeField] FixedJoint joint;
    [SerializeField] Transform fingers;
    [SerializeField] Transform originalFingerPlaceholder;
    [SerializeField] Transform closedFingerPlaceholder;
    bool fistClosed = false;
    Vector3 lastPos;
    Vector3 currentVelocity;

    void OnCollisionEnter(Collision col)
    {
        GameObject other = col.gameObject;
        if (other.GetComponent<Rigidbody>() != null)
        {
            touchedObject = other;
        }
    }

    void OnCollisionExit(Collision col)
    {
        GameObject other = col.gameObject;
        if (other == touchedObject)
        {
            touchedObject = null;
        }
    }

    protected override void OnPrimaryFire()
    {
        ammo++;
        if (grabbed && joint.connectedBody!=null)
        {
                Rigidbody other = joint.connectedBody;
                joint.connectedBody = null;
                other.velocity = currentVelocity;
            Debug.Log(string.Format("Other object is {0}, its velocity is {1}, controller velocity is {2}", other.gameObject, other.velocity, currentVelocity));
                grabbed = false;
        }
        else if (touchedObject != null && !grabbed)
        {
            joint.connectedBody = touchedObject.GetComponent<Rigidbody>();
            grabbed = true;
        }
        fistClosed = !fistClosed;
        if (grabbed)
            fistClosed = true;
        MoveFingers();
        
    }

    void MoveFingers()
    {
        if (fistClosed)
        {
            fingers.rotation = closedFingerPlaceholder.rotation;
        }
        else
        {
            fingers.rotation = originalFingerPlaceholder.rotation;
        }
    }

    void Update()
    {
        currentVelocity = (this.transform.position - lastPos) / Time.deltaTime;
        lastPos = this.transform.position;
    }

    public override void Start()
    {
        Collider[] playerColliders = GetComponentsInParent<Collider>();
        foreach (var current in playerColliders)
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), current);
        }
    }

}
