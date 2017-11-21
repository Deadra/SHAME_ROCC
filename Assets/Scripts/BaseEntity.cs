using UnityEngine;
using UnityEngine.Networking;

public enum TeamList
{
    Friendly, Hostile, Neutral
}

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
        //Debug.LogWarningFormat("{0} is the Server - {1}, Client - {2}, Local Player - {3}",
        //        gameObject.name, isServer.ToString(), isClient.ToString(), isLocalPlayer.ToString());

        if (!isLocalPlayer)
        {
            foreach (var component in GetComponentsInChildren<Camera>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<AudioListener>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<PlatformDataSender>())
                component.enabled = false;
            /*foreach (var component in GetComponentsInChildren<VRManager>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<VRTeleport>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<SteamVR_ControllerManager>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<SteamVR_PlayArea>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<SteamVR_TrackedObject>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<SteamVR_RenderModel>())
                component.enabled = false;*/
            foreach (var component in GetComponentsInChildren<SteamVR_Camera>())
                component.enabled = false;
            foreach (var component in GetComponentsInChildren<VRTeleport>())
                component.enabled = false;
            //foreach (var component in GetComponentsInChildren<SteamVR_Ears>())
            //    component.enabled = false;
        }

        if (!isServer)
        {
            if (GetComponentInChildren<BaseAI>())
                GetComponentInChildren<BaseAI>().enabled = false;
        }
    }

    protected virtual void Update()
    {

    }

    public void TakeDamage(float value)
    {
        if (!isServer)
            return;

        currentHealth -= value;
        OnDamageTaken(value);

        //Debug.LogFormat("{0} took {1} damage. Remaining health = {2}", gameObject, value, currentHealth);

        if (currentHealth <= 0)
            RpcOnDeath();
    }

    public void TakeDamage(float value, BaseEntity attacker)
    {
        TakeDamage(value);
        BaseAI brain = this.GetComponent<BaseAI>();
        if (brain != null)
        {
            //Debug.Log("got brain");
            brain.AttackedBy(attacker);
        }
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

    public static void SetLayerRecursively(GameObject go, Layer layer)
    {
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = (int)layer;
        }
    }
}