using UnityEngine;

public class SnatcherState : EnemyState
{
    protected Snatcher snatcher;
    protected EnemyTargetHandler targetHandler;

    public SnatcherState(Enemy enemy) : base(enemy)
    {
        snatcher = enemy as Snatcher;
        targetHandler = snatcher.GetComponent<EnemyTargetHandler>();
    }
}
