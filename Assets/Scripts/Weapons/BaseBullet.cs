using UnityEngine;
using UnityEngine.Networking;

public class BaseBullet : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float launchForce;

    public TeamList Team { get; set; }

    public BaseEntity Holder { get; set; }
    [SerializeField] private bool ricochet = false;
    [SerializeField] private float ricochetAngle = 10;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * launchForce);
    }

    void OnCollisionEnter(Collision col)
    {
        SetOff(col);

        if (!ricochet || Mathf.Abs(180 - Vector3.Angle(col.relativeVelocity, rb.velocity)) > ricochetAngle)
            Destroy(this.gameObject);        
    }

    protected virtual void SetOff(Collision col)
    {
    }
}