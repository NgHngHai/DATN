
public class Fly0State : EnemyState
{
    protected Fly0 fly0;
    protected EnemyTargetHandler targetHandler;
    protected EnemyAttackBehavior attackBehavior;

    public Fly0State(Enemy enemy) : base(enemy)
    {
        fly0 = enemy as Fly0;
        targetHandler = fly0.GetComponent<EnemyTargetHandler>();
        attackBehavior = fly0.GetComponent<EnemyAttackBehavior>();
    }
}
