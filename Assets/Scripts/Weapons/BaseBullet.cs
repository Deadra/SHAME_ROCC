using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Базовый класс для пули. Это просто летящий объект, 
/// при столкновении с чем-то существенным вызывающий OnHit()
/// </summary>
/// <remarks>
/// В BaseBullet нет зависимости от DontGoThroughThings, но 
/// если не добавить этот скрипт к пуле, на высоких скоростях 
/// она будет проходить сквозь стены. Также нет зависимостей и от
/// DestroyAtferTime, однако пуля должна исчезать из игровой сцены, 
/// если даже она никуда не попала
/// </remarks>
[RequireComponent(typeof(DontGoThroughThings), typeof(DestroyAtferTime))]
public class BaseBullet : NetworkBehaviour
{
    private new Rigidbody rigidbody;
    [SerializeField] private float launchForce;
    [SerializeField] private float accelerationForce = 0.0f;
    [SerializeField] private float accelerationTime = 2.0f;
    [SerializeField] private bool  ricochet = false;
    [SerializeField] private float ricochetAngle = 10;
    [SerializeField] private float minimumRayCastLength = 1.0f;
    public TeamList Team { get; set; }
    public BaseEntity Holder { get; set; }

    private GameObject hitted;
    private bool destroy = false;
    private float timeFromLaunch = 0.0f;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward * launchForce);
    }

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
        Vector3 velocity = rigidbody.velocity;
        Vector3 raycastDirection = velocity.normalized;
        float raycastLength = velocity.magnitude * Time.fixedDeltaTime;
        timeFromLaunch += Time.fixedDeltaTime;

        if (velocity == Vector3.zero) //At the first FixedUpdate velocity is still a zero vector 
        {
            raycastLength = launchForce / rigidbody.mass * Time.fixedDeltaTime * (2.0f / 100.0f); //magic number (⌒_⌒;)
            raycastDirection = transform.forward.normalized;
        }

        if (raycastLength < minimumRayCastLength)
            raycastLength = minimumRayCastLength;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, raycastDirection, out hitInfo, raycastLength))
        {
            if (hitted != null || !hitInfo.collider || hitInfo.collider.isTrigger)
                return;
            
            if (hitInfo.collider.gameObject.GetComponent<BaseBullet>() &&
                hitInfo.collider.gameObject.GetComponent<BaseBullet>().Team == Team)
                return;
   
            OnHit(hitInfo);

            if (!ricochet || Mathf.Abs(Vector3.Angle(hitInfo.normal, raycastDirection) - 90) > ricochetAngle)
            {
                Destroy(this.gameObject, 0.5f);
                destroy = true;
                hitted = hitInfo.collider.gameObject;
            }
        }

        if (accelerationTime > 0.0f && timeFromLaunch < accelerationTime)
            rigidbody.AddForce(raycastDirection * accelerationForce, ForceMode.Acceleration);
    }

    /// <summary>
    /// Обработчик столкновения
    /// </summary>
    protected virtual void OnHit(RaycastHit hitInfo)
    {

    }
}