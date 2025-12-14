using UnityEngine;

public class Skill_Blast: MonoBehaviour, ISkill
{
    // Dash, damaging everything hit
    // Blast params
    [SerializeField] private float blastSpeed = 20f;
    [SerializeField] private float blastDuration = 0.5f;
    private float blastTimeLeft = 0f;
    private bool isBlasting = false;

    // Animation State
    public AnimationState blastState;

    // References
    private PlayerController _playerController;
    private Rigidbody2D _rb;
    private Health _playerHealth;
    [SerializeField] private GameObject _hurtBox;

    private void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _rb = GetComponentInParent<Rigidbody2D>();
        _playerHealth = GetComponentInParent<Health>();
        blastState = new AnimationState(_playerController, "blast", true);
    }

    private void FixedUpdate()
    {
        if (isBlasting)
        {
            blastTimeLeft -= Time.fixedDeltaTime;
            float dir = _playerController.isFacingRight ? 1f : -1f;

            _rb.linearVelocityX = dir * blastSpeed;
            _rb.linearVelocityY = 0f; // Maintain zero vertical velocity during dash

            if (blastTimeLeft <= 0f)
            {
                EndBlast();
            }
        }   
    }

    public void Activate()
    {
        StartBlast();
    }

    private void StartBlast()
    {
        //_playerController.isDashing = true;
        _playerController.movementLocked = true;
        _playerController.animStateLocked = true;

        _playerController.animStateMachine.ChangeState(blastState);

        isBlasting = true;

        blastTimeLeft = blastDuration;
        
        _rb.gravityScale = 0f; // Disable gravity during blast
        _rb.linearVelocityY = 0f; // Cancel any vertical velocity

        _playerHealth.isInvincible = true; // Invincible during dash

        _hurtBox.SetActive(true); // Enable hurtbox to deal damage
    }
    
    private void EndBlast()
    {
        //_playerController.isDashing = false;
        _playerController.movementLocked = false; // Unlock movement
        _playerController.animStateLocked = false; // Unlock anim state
        isBlasting = false;

        _rb.gravityScale = _playerController.originalGravityMultiplier; // Restore gravity

        _playerHealth.isInvincible = false; // Disable invincibility

        _hurtBox.SetActive(false); // Disable hurtbox
    }
}
