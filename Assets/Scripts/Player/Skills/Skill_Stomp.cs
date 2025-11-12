using UnityEngine;

public class Skill_Stomp : MonoBehaviour, ISkill
{
    [SerializeField] private int stompGravityScale= 25;
    [SerializeField] private bool canStomp = false;
    private float originalGravityScale;
    private bool isStomping = false;

    // References
    [SerializeField] private GameObject player;
    private PlayerController playerController;
    private CapsuleCollider2D heightDetection;
    private Rigidbody2D rb;

    // Ground detection
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;


    private void Awake()
    {
        rb = player.GetComponent<Rigidbody2D>();
        playerController = player.GetComponent<PlayerController>();
        originalGravityScale = playerController.originalGravityMultiplier;
        heightDetection = player.GetComponent<CapsuleCollider2D>();
    }

    public void Activate()
    {
        isStomping = true;

        // Lock movement, change gravity multiplier to avoid overwrite
        playerController.movementLocked = true;
        playerController.fallGravityMultiplier = stompGravityScale;

        // Increase gravity scale for stomp effect, set horizontal velocity to 0
        rb.gravityScale = stompGravityScale;
        rb.linearVelocityX = 0;
    }

    private void FixedUpdate()
    {
        if (!isStomping)
            return;

        // Detect landing and end stomp once
        if (playerController.isGrounded)
        {
            EndStomp();
        }
    }

    private void EndStomp()
    {
        isStomping = false;

        // Restore movement and gravity settings
        playerController.movementLocked = false;
        playerController.fallGravityMultiplier = originalGravityScale;
    }
}
