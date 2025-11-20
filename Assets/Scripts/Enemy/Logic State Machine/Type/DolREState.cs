using UnityEngine;

public class DolREState : EnemyOffensiveState
{
    protected EnemyDolRE dolre;
    public DolREState(Enemy enemy) : base(enemy)
    {
        dolre = enemy as EnemyDolRE;
    }
}

public class DolREIdleObservationState : DolREState
{
    public DolREIdleObservationState(Enemy enemy) : base(enemy)
    {
    }
}

public class DolREExplosionProjectileState : DolREState
{
    public DolREExplosionProjectileState(Enemy enemy) : base(enemy)
    {
    }
}

public class DolREHomingProjectileState : DolREState
{
    public DolREHomingProjectileState(Enemy enemy) : base(enemy)
    {
    }
}
