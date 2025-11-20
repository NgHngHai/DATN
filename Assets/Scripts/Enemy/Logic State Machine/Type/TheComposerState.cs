using UnityEngine;

public class TheComposerState : EnemyOffensiveState
{
    protected BossTheComposer theComposer;
    public TheComposerState(Enemy enemy) : base(enemy)
    {
        theComposer = enemy as BossTheComposer;
    }
}