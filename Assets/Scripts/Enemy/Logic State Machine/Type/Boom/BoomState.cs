using UnityEngine;

public class BoomState : EnemyState
{
    protected Boom boom;
    protected EnemyTargetHandler targetHandler;
    public BoomState(Enemy enemy) : base(enemy)
    {
        boom = enemy as Boom;
        targetHandler = boom.GetComponent<EnemyTargetHandler>();
    }
}
