using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Этот класс отвечает за перемещение лягушки
/// </summary>
public class FrogMover : BaseMover
{
    private Rigidbody rb;
    [SerializeField] float jumpForce;
    [SerializeField] Transform head;
    public Transform CurrentTarget { get; set; }
    float moveTime = 4;
    float moveStarted;
    float nextMove;
    float jumpCooldown = 3f;
    float nextJump;
    const float jumpForceFactor = 0.9f;

    [SerializeField] Animator headAnimator;
    [SerializeField] Animator bodyAnimator;

    public bool Aggressive { get; set; }

    bool canBite = false;

    public override void Start()
    {
        base.Start();
        Aggressive = true;
        rb = this.GetComponent<Rigidbody>();
        nextMove = Time.time;
        nextJump = Time.time;
        Physics.IgnoreCollision(this.GetComponent<Collider>(), transform.Find("FrogHead").GetComponent<Collider>());
    }

    public void JumpTowards()
    {
        if (Time.time > nextMove)
        {
            nextMove = Time.time + moveTime;
            moveStarted = Time.time;
        }
        if (Time.time > moveStarted + 1 && Time.time < moveStarted + 2)
        {
            BodyMatchHead();
        }
        if (Time.time > moveStarted + 2) //&& LooksAtTarget(currentTarget.position))
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (Time.time > nextJump)
        {
            nextJump = Time.time + jumpCooldown;
            Vector3 forward = head.forward;
            forward.y = 0;
            forward = forward.normalized;
            rb.AddForce((forward + Vector3.up * jumpForceFactor).normalized * jumpForce);
            RpcAnimateBody("Jump");
            if (Aggressive)
                Bite();
        }
    }

    public bool LookAtTarget()
    {
        RpcLookAtTarget(CurrentTarget.position);
        return LooksAtTarget(head.transform.position);
    }

    public bool LookAtTarget(Transform lookTargetTransform)
    {
        RpcLookAtTarget(lookTargetTransform.position);
        return LooksAtTarget(head.transform.position);
    }

    [ClientRpc]
    public void RpcLookAtTarget(Vector3 lookTarget)
    {
        float rotSpeed = 360f;
        Vector3 D = lookTarget - head.transform.position;
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(D), rotSpeed * Time.deltaTime);
        head.rotation = rot;

        float minRotation = -80;
        float maxRotation = 80;
        Vector3 currentRotation = head.localRotation.eulerAngles;
        if (currentRotation.y < 180)
        {
            currentRotation.y = Mathf.Clamp(currentRotation.y, minRotation, maxRotation);
        }
        else
        {
            currentRotation.y = Mathf.Clamp(currentRotation.y, 360 + minRotation, 360);
        }
        head.localRotation = Quaternion.Euler(currentRotation);
    }

    public void BodyMatchHead()
    {
        float rotSpeed = 4f;
        Quaternion rot = Quaternion.Slerp(transform.rotation, head.rotation, rotSpeed * Time.deltaTime);
        //rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);
        transform.eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);
    }

    public bool LooksAtTarget(Vector3 lookTarget)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - head.position);
        Quaternion currentRotation = head.rotation;
        float rotatingError = Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y);
        if (rotatingError < 5)
            return true;
        else
            return false;
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log(string.Format("Frog collided with {0}", col.collider.gameObject));
        if (isServer)
            RpcAnimateHead("Close");
        if (canBite)
        {
            //Debug.Log(string.Format("Can bite! Collided with {0}", col.gameObject));
            BaseEntity enemy = col.collider.gameObject.GetComponent<BaseEntity>();
            //Debug.Log("pam");
            if (enemy != null && enemy.Team != this.GetComponent<BaseEntity>().Team)
            {
                //Debug.Log("It's an enemy! Taking damage.");
                enemy.TakeDamage(15, this.GetComponent<BaseEntity>());
                //Debug.Log("poom");
            }
        }
        canBite = false;
    }

    void Bite()
    {
        canBite = true;
        if (Physics.Raycast(head.position, head.forward, 8.0f))
        {
            int c = Random.Range(0, 2);
            switch (c)
            {
                case 0:
                    RpcAnimateHead("Open1");
                    break;

                case 1:
                    RpcAnimateHead("Open2");
                    break;
            }
        }
    }

    [ClientRpc]
    void RpcAnimateHead(string trigger)
    {
        headAnimator.SetTrigger(trigger);
    }

    [ClientRpc]
    void RpcAnimateBody(string trigger)
    {
        bodyAnimator.SetTrigger(trigger);
    }
}