using UnityEngine;

public class SnatcherJumpOnTargetState : SnatcherState
{
    private Rigidbody2D rb;
    private Collider2D col;

    private bool hasJumped;
    private bool hasLanded;

    public SnatcherJumpOnTargetState(Enemy enemy) : base(enemy)
    {
        rb = snatcher.GetComponent<Rigidbody2D>();
        col = snatcher.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        base.Enter();

        snatcher.OnTriggerEntered += HandleTriggerEnter;
        snatcher.OnTriggerExited += HandleTriggerExit;

        JumpAtPlayer();
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0 && hasLanded )
        {
            logicStateMachine.ChangeState(snatcher.chaseState);
        }

    }

    public override void Exit()
    {
        base.Exit();

        snatcher.OnTriggerEntered -= HandleTriggerEnter;
        snatcher.OnTriggerExited -= HandleTriggerExit;
    }

    private void HandleTriggerEnter(Collider2D collision)
    {
        bool hitTarget = PhysicsUtils.IsGameObjectInLayer(collision.gameObject, snatcher.whatIsTarget);

        bool hitLand = PhysicsUtils.IsGameObjectInLayer(collision.gameObject, snatcher.whatIsGround);

        if (hitTarget || hitLand)
        {
            if(hitTarget)
            {
                Debug.Log($"Deal {snatcher.jumpDamage} Jump Damage to {collision.name}");
            }

            CrossJumpWhenLanding();
        }
    }

    private void HandleTriggerExit(Collider2D collision)
    {
        if (hasJumped) return;

        if (PhysicsUtils.IsGameObjectInLayer(collision.gameObject, snatcher.whatIsGround))
        {
            hasJumped = true;
        }
    }

    private void CrossJumpWhenLanding()
    {
        snatcher.StopVelocity();

        float angle = snatcher.landJumpAngle;
        Vector2 baseDir = Quaternion.Euler(0, 0, angle) * Vector2.right;

        if (Random.Range(0, 2) == 0)
            baseDir.x = -baseDir.x;

        rb.AddForce(baseDir.normalized * snatcher.landJumpForce, ForceMode2D.Impulse);

        stateTimer = 1f;
        hasLanded = true;
        col.isTrigger = false;
    }

    private void JumpAtPlayer()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        Vector2 startPos = snatcher.transform.position;
        Vector2 targetPos = targetHandler.CurrentTarget.position;

        float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float t = snatcher.jumpTime;

        Vector2 displacement = targetPos - startPos;

        float vx = displacement.x / t;
        float vy = displacement.y / t + 0.5f * g * t;

        Vector2 velocity = new Vector2(vx, vy);
        rb.AddForce(velocity * rb.mass, ForceMode2D.Impulse);

        snatcher.FlipOnVelocityX();
    }
}
