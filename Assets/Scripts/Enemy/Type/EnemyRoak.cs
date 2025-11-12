using UnityEngine;

/// <summary>
/// Ground enemy that only patrols and releases poisonous smoke after a delay.
/// Poison smoke deals damage over time.
/// </summary>
public class EnemyRoak : GroundEnemy
{
    [Header("Enemy: Roak")]
    [Tooltip("Time interval between each poison smoke release.")]
    public float createPoisonDelay;

    [Tooltip("Prefab of the poison smoke created periodically.")]
    [SerializeField] private GameObject poisonSmoke;

    public RoakState mainState;

    protected override void Awake()
    {
        base.Awake();
        mainState = new RoakState(this);
    }

    protected override void Start()
    {
        base.Start();
        logicStateMachine.Initialize(mainState);
    }

    public void CreatePoison()
    {
        Instantiate(poisonSmoke, transform.position, Quaternion.identity);
    }

}
