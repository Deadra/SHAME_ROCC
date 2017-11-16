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
    public TeamList Team { get; set; }
    public BaseEntity Holder { get; set; }

    private GameObject hitted;
    private bool destroy = false;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * launchForce);
    }

    /// <summary>
    /// Уничтожает пулю, если надо. Это нужно делать только после того, как RigidBody пули
    /// вступил в коллизию с каким-то объектом
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
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

        if (Physics.Raycast(gameObject.transform.position, velocity, out hitInfo, velocity.magnitude * Time.deltaTime))
        {
            if (hitted != null || !hitInfo.collider || hitInfo.collider.isTrigger)
                return;

            if (hitInfo.collider.gameObject.GetComponent<BaseBullet>() &&
                hitInfo.collider.gameObject.GetComponent<BaseBullet>().Team == Team)
                return;

            OnHit(hitInfo);

            if (!ricochet || Mathf.Abs(Vector3.Angle(hitInfo.normal, velocity) - 90) > ricochetAngle)
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