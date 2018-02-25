using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Vehicles.Car;
using UnityStandardAssets.Vehicles.Aeroplane;

public enum TeamList
{
    Friendly, Hostile, Neutral
}

/// <summary>
/// Основная сущность для персонажей игры, как игровых, так и неигровых.
/// Имеет здоровье, может получать повреждения и умирать.
/// </summary>
public class BaseEntity : NetworkBehaviour
{
    [SerializeField] private TeamList team;
    public TeamList Team
    { 
        get { return team; }
        set { team = value; } 
    }
    [SerializeField] protected float maxHealth;
    [SyncVar] protected float currentHealth;

    private bool destroyInLateUpdate;

    public EventHub EHub { get; set; }

    void Awake()
    {
        currentHealth = maxHealth;
        EHub = FindObjectOfType<EventHub>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        EHub = FindObjectOfType<EventHub>();
    }

    public virtual void Start()
    {
        if (!isLocalPlayer)
        {
            gameObject.EnableComponentsInParentAndChildren<Camera>(false);
            gameObject.EnableComponentsInParentAndChildren<AudioListener>(false);
            gameObject.EnableComponentsInParentAndChildren<PlatformDataSender>(false);
            gameObject.EnableComponentsInParentAndChildren<SteamVR_RenderModel>(false);
            gameObject.EnableComponentsInParentAndChildren<SteamVR_Camera>(false);
            gameObject.EnableComponentsInParentAndChildren<CarController>(false);
            gameObject.EnableComponentsInParentAndChildren<CarUserControl>(false);
            gameObject.EnableComponentsInParentAndChildren<AeroplaneController>(false);
            gameObject.EnableComponentsInParentAndChildren<AeroplaneUserControl4Axis>(false);
            gameObject.EnableComponentsInParentAndChildren<AeroplaneControlSurfaceAnimator>(false);
        }

        if (!isServer)
        {
            gameObject.EnableComponentsInParentAndChildren<BaseAI>(false);
        }
    }

    protected virtual void Update()
    {

    }

    private void TakeDamage(float value)
    {
        if (!hasAuthority)
            return;

        currentHealth -= value;
        OnDamageTaken(value);

        if (currentHealth <= 0)
            RpcOnDeath();
    }

    public void TakeDamage(float value, BaseEntity attacker)
    {
        TakeDamage(value);
        BaseAI brain = this.GetComponent<BaseAI>();

        if (brain != null)
            brain.AttackedBy(attacker);
    }

    public void TakeDeadlyDamage()
    {
        TakeDamage(currentHealth + 1.0f);
    }

    protected virtual void OnDamageTaken(float value) { }

    protected virtual void RpcOnDeath()
    {
        EHub.SignalEntityDeath(this);
        destroyInLateUpdate = true;
    }

    void LateUpdate()
    {
        if (destroyInLateUpdate)
            Destroy(this.gameObject);
    }
}