using UnityEngine;

public class FrogAI : BaseAI
{
    public enum FrogAiStates
    {
        Patroling, AttackNear, AttackFar, Seeking
    }

    FrogAiStates currentState = FrogAiStates.Patroling;
    private FrogAiStates CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;
            stateText.text = string.Format("{0}", currentState);
        }
    }

    [SerializeField] TextMesh stateText;

    [SerializeField] public Transform patrolingCenter;
    private Vector3 currentTarget;

    [SerializeField] private float patrolingRadius;
    [SerializeField] private Transform head;
    [SerializeField] private float visionDistance = 30.0f;

    private BaseEntity currentTargetEntity;
    private FrogMover mover;
    private Vector3 lastKnownLocation;

    void Start()
    {
        mover = this.GetComponent<FrogMover>();
        currentTarget = patrolingCenter.position;
        FindObjectOfType<EventHub>().EntityDeathEvent += new EntityDeathHandler(EntityDeathDetected);
    }

    void Update()
    {
        switch (CurrentState)
        {
            case (FrogAiStates.Patroling):
                PatrolingState();
                break;

            case (FrogAiStates.AttackNear):
                AttackNearState();
                break;

            case (FrogAiStates.AttackFar):
                AttackFarState();
                break;

            case (FrogAiStates.Seeking):
                SeekingState();
                break;
        }

        if (currentTargetEntity != null && Vector3.Distance(currentTargetEntity.transform.position, transform.position) > visionDistance)
        {
            CancelInvoke();
            CurrentState = FrogAiStates.Patroling;
            currentTargetEntity = null;
        }
    }

    void EntityDeathDetected(BaseEntity died)
    {
        if (currentTargetEntity == died)
        {
            CurrentState = FrogAiStates.Patroling;
            currentTargetEntity = null;
        }
    }

    void PatrolingState()
    {
        Vector3 frogPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPosition = new Vector3(currentTarget.x, 0, currentTarget.z);
        float distanceToTarget = Vector3.Distance(frogPosition, targetPosition);

        if (distanceToTarget > 3)
        {
            mover.Aggressive = false;
            mover.LookAtTarget(currentTarget);
            mover.JumpTowards();
        }
        else
        {
            currentTarget = GetPointInCircle(patrolingCenter.position, patrolingRadius);
        }
    }

    void AttackNearState()
    {
        mover.Aggressive = true;
        mover.LookAtTarget(currentTargetEntity.transform.position);
        mover.JumpTowards();
        if (!IsInvoking())
            InvokeRepeating("CheckTargetVisible", 0.0f, 0.5f);
    }

    void AttackFarState()
    {
    }

    void SeekingState()
    {
        mover.Aggressive = false;
        mover.LookAtTarget(currentTarget);
        mover.JumpTowards();
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);
        if (distanceToTarget < 3)
        {
            CurrentState = FrogAiStates.Patroling;
            //currentTargetEntity = null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(string.Format("{0} entered the trigger", col.gameObject));
        CheckIfEnemy(col);
    }

    void OnTriggerStay(Collider col)
    {
    }

    void CheckIfEnemy(Collider col)
    {
        BaseEntity other = col.gameObject.GetComponentInParent<BaseEntity>();
        Debug.Log(string.Format("Other is {0}", other));
        if (other == null)
            return;
        if (other == this.GetComponent<BaseEntity>())
            return;
        else
        {
            if (other.Team != this.GetComponent<BaseEntity>().Team)
            {
                CurrentState = FrogAiStates.AttackNear;
                mover.CurrentTarget = other.transform;
                currentTargetEntity = other;
            }
        }
    }

    void CheckTargetVisible()
    {
        if (currentTargetEntity == null)
        {
            CancelInvoke();
            CurrentState = FrogAiStates.Patroling;
            return;
        }

        RaycastHit hit;
        if (Physics.Linecast(head.position, currentTargetEntity.transform.position, out hit))
        {
            //Debug.Log(string.Format("Linecast hit {0}", hit.collider.gameObject));
            BaseEntity other = hit.collider.gameObject.GetComponentInParent<BaseEntity>();
            if (other != currentTargetEntity && other != this.GetComponent<BaseEntity>())
            {
                if (CurrentState == FrogAiStates.AttackNear)
                {
                    CurrentState = FrogAiStates.Seeking;
                    currentTarget = hit.point;
                }
            }
            else if (other == currentTargetEntity)
            {
                CurrentState = FrogAiStates.AttackNear;
            }
        }
    }

    public override void AttackedBy(BaseEntity attacker)
    {
        //Debug.Log(string.Format("attacked by {0}",attacker));
        if (attacker != null)
        {
            currentTargetEntity = attacker;
            CurrentState = FrogAiStates.AttackNear;
        }
    }

    public Vector3 GetPointInCircle(Vector3 center, float radius)
    {
        float x, z;
        do
        {
            x = Random.Range(center.x - radius, center.x + radius);
            z = Random.Range(center.z - radius, center.z + radius);
        }
        while ((x - center.x) * (x - center.x) + (z - center.z) * (z - center.z) >= radius * radius);
        //Debug.Log(string.Format("{0} units need a circle with radius {1}. Giving coordinates {2}.", numberOfUnits, circleRadius, new Vector3(x, center.y, z)));
        return new Vector3(x, center.y, z);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolingCenter.position, patrolingRadius);
        Gizmos.DrawLine(transform.position, currentTarget);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionDistance);
    }
}