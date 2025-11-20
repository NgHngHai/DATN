using UnityEngine;

public class EliteState : EnemyOffensiveState
{
    protected BossElite elite;
    public EliteState(Enemy enemy) : base(enemy)
    {
        elite = enemy as BossElite;
    }
}