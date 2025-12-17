using UnityEngine;

public class BossAnimationEvent : EnemyAnimationEvent
{
    private Boss boss;

    protected override void Start()
    {
        base.Start();
        boss = enemy as Boss;
    }

    private void DashTowardFacingDir(float dashForce)
    {
        boss.rb.AddForce(new Vector2(boss.FacingDir * dashForce, 0), ForceMode2D.Impulse);
    }

    private void CreateDeathExplosionInRadius()
    {
        boss.CreateDeathExplosionInRadius();
    }

    private void DestroyBoss() => Destroy(boss.gameObject);
}
