using UnityEngine;

public class BossAnimationEvent : EnemyAnimationEvent
{
    private void DashTowardFacingDir(float dashForce)
    {
        enemy.rb.AddForce(new Vector2(enemy.FacingDir * dashForce, 0), ForceMode2D.Impulse);
    }
}
