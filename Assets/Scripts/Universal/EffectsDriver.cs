using UnityEngine;

[DisallowMultipleComponent]
public class EffectsDriver : MonoBehaviour
{
    [Header("Particle Systems to play on events")]
    public ParticleSystem jumpDust;
    public ParticleSystem dashTrail;
    public ParticleSystem landPoof;
    public ParticleSystem hitSpark;
    public ParticleSystem deathBurst;

    private EffectEvents events;

    private void Awake()
    {
        events = GetComponent<EffectEvents>();
    }

    private void OnEnable()
    {
        if (events == null) return;

        events.OnJump.AddListener(PlayJumpDust);
        events.OnDashStart.AddListener(PlayDashTrail);
        events.OnLand.AddListener(PlayLandPoof);
        events.OnDamagedWithReaction.AddListener(PlayHitSpark);
        events.OnDeath.AddListener(PlayDeathBurst);
    }

    private void OnDisable()
    {
        if (events == null) return;

        events.OnJump.RemoveListener(PlayJumpDust);
        events.OnDashStart.RemoveListener(PlayDashTrail);
        events.OnLand.RemoveListener(PlayLandPoof);
        events.OnDamagedWithReaction.RemoveListener(PlayHitSpark);
        events.OnDeath.RemoveListener(PlayDeathBurst);
    }

    private void PlayJumpDust() { if (jumpDust != null) jumpDust.Play(); }
    private void PlayDashTrail() { if (dashTrail != null) dashTrail.Play(); }
    private void PlayLandPoof() { if (landPoof != null) landPoof.Play(); }
    private void PlayDeathBurst() { if (deathBurst != null) deathBurst.Play(); }
    private void PlayHitSpark(Vector2 dir) 
    { 
        if (hitSpark == null) return;

        Vector3 dir3 = dir.sqrMagnitude < 1e-5f ? Vector3.up : new Vector3(dir.x, dir.y, 0f).normalized;

        // Rotate so local up matches the incoming direction (2D convention)
        hitSpark.transform.rotation = Quaternion.FromToRotation(Vector3.right, dir3);

        hitSpark.Play();
    }
}
