using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Базовый класс для пули. Это просто летящий объект, 
/// при столкновении с чем-то существенным вызывающий OnHit()
/// </summary>
public class BaseBullet : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float launchForce;
    [SerializeField] private bool  ricochet = false;
    [SerializeField] private float ricochetAngle = 10;
    [SerializeField] private float minimumRayCastLength = 1.0f;
    public TeamList Team { get; set; }
    public BaseEntity Holder { get; set; }

    private GameObject hitted;
    private bool destroy = false;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * launchForce);
    }

    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        DestroyBullet(col);
    }

    void OnCollisionStay(Collision col)
    {
        DestroyBullet(col);
    }

    /// <summary>
    /// Уничтожает пулю, если надо. Это нужно делать только после того, как RigidBody пули
    /// вступил в коллизию с каким-то объектом
    /// </summary>
    private void DestroyBullet(Collision col)
    {
        if (col.gameObject == hitted && destroy)
            Destroy(this.gameObject);
    }

    /// <summary>
    /// Полное управление полётом пули: определяет столкновение с объектом, решает, произойдёт ли рикошет пули,
    /// вызывает обработчик и т.д.
    /// </summary>
    void FixedUpdate()
    {
        RaycastHit hitInfo;
        Vector3 velocity = rb.velocity;
        float raycastLength;
        Vector3 raycastDirection;

        if (velocity == Vector3.zero) //At the first FixedUpdate velocity is still a zero vector 
        {
            raycastLength = launchForce / rb.mass * Time.fixedDeltaTime * (2.0f / 100.0f); //magic number (⌒_⌒;)
            raycastDirection = transform.forward;
        }
        else
        {
            raycastLength = velocity.magnitude * Time.fixedDeltaTime;
            raycastDirection = velocity.normalized;
        }

        if (raycastLength < minimumRayCastLength)
            raycastLength = minimumRayCastLength;

        if (Physics.Raycast(transform.position, raycastDirection, out hitInfo, raycastLength))
        {
            //Debug.Log("Hit");
            if (hitted != null || !hitInfo.collider || hitInfo.collider.isTrigger)
                return;
            
            if (hitInfo.collider.gameObject.GetComponent<BaseBullet>() &&
                hitInfo.collider.gameObject.GetComponent<BaseBullet>().Team == Team)
                return;
            OnHit(hitInfo);

            if (!ricochet || Mathf.Abs(Vector3.Angle(hitInfo.normal, raycastDirection) - 90) > ricochetAngle)
            {
                destroy = true;
                hitted = hitInfo.collider.gameObject;
            }
        }
    }

    /// <summary>
    /// Обработчик столкновения
    /// </summary>
    protected virtual void OnHit(RaycastHit hitInfo)
    {

    }
}