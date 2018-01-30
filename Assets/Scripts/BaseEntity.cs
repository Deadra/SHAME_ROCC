using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Vehicles.Car;
using UnityStandardAssets.Vehicles.Aeroplane;

public enum TeamList
{
    Friendly, Hostile, Neutral
}

/// <summary>
/// Основная сущность для персонажей игры, как игровых, так и неигровых
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
            gameObject.EnableAllComponentsInRoot<Camera>(false);
            gameObject.EnableAllComponentsInRoot<AudioListener>(false);
            gameObject.EnableAllComponentsInRoot<PlatformDataSender>(false);
            gameObject.EnableAllComponentsInRoot<SteamVR_RenderModel>(false);
            gameObject.EnableAllComponentsInRoot<SteamVR_Camera>(false);
            gameObject.EnableAllComponentsInRoot<CarController>(false);
            gameObject.EnableAllComponentsInRoot<CarUserControl>(false);
            gameObject.EnableAllComponentsInRoot<AeroplaneController>(false);
            gameObject.EnableAllComponentsInRoot<AeroplaneUserControl4Axis>(false);
            gameObject.EnableAllComponentsInRoot<AeroplaneControlSurfaceAnimator>(false);
        }

        if (!isServer)
        {
            gameObject.EnableAllComponentsInRoot<BaseAI>(false);
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