using System.Collections;
using UnityEngine;

public class Skill_Stomp : MonoBehaviour, ISkill
{
    [SerializeField] private int stompGravityScale= 25;
    [SerializeField] private bool canStomp = false;

    private float originalGravityScale;
    private bool isStomping = false;
    private bool isLockedByStomp = false; // To unlock movement locked by stomp

    // References
    [SerializeField] private GameObject player;
    private PlayerController playerController;
    private CapsuleCollider2D heightDetection;
    private Rigidbody2D rb;
    [SerializeField] private GameObject _hurtBox;

    // Ground detection
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;

    // Animation State
    public AnimationState stompState;


    private void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
        playerController = player.GetComponent<PlayerController>();
        originalGravityScale = playerController.originalGravityMultiplier;
        heightDetection = gameObject.GetComponent<CapsuleCollider2D>();
        stompState = new AnimationState(playerController, "stomp", true);
    }

    public void Activate()
    {
        if (!canStomp)
            return;

        isStomping = true;
        isLockedByStomp = true;

        // Lock movement, change gravity multiplier to avoid overwrite
        playerController.movementLocked = true;
        playerController.animStateLocked = true; 
        playerController.fallGravityMultiplier = stompGravityScale;

        // Change anims
        playerController.animStateMachine.ChangeState(stompState);

        // Increase gravity scale for stomp effect, set horizontal velocity to 0
        rb.gravityScale = stompGravityScale;
        rb.linearVelocityX = 0;
    }

    private void FixedUpdate()
    {
        canStomp = !heightDetection.IsTouchingLayers(groundLayer);

        if (!isStomping)
            return;

        // Detect landing and end stomp once
        if (playerController.isGrounded && isLockedByStomp)
        {
            EndStomp();
        }
    }

    private void EndStomp()
    {
        isStomping = false;
        isLockedByStomp = false;

        // Restore movement and gravity settings
        playerController.movementLocked = false;
        playerController.animStateLocked = false; 
        playerController.fallGravityMultiplier = originalGravityScale;
    }
}
