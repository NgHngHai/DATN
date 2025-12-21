using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class Chest : Interactables
{
    [SerializeField] private int goldAmount = 50;
    [SerializeField] private float moneyDelaySeconds = 0.5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem moneyParticle;
    [SerializeField] private AudioSource chestOpenSFX;

    [Header("Post-open behavior")]
    [Tooltip("Extra delay after particles finish before disabling/destroying.")]
    [SerializeField] private float postOpenDelaySeconds = 0.5f;
    [Tooltip("Duration of the fade-out before disabling the chest.")]
    [SerializeField] private float fadeDuration = 0.35f;

    // References
    private SpriteRenderer spriteRenderer;
    private SFXEmitter sfxEmitter;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnInteract(GameObject player)
    {
        if (isOneTimeUse && playerInteracted)
            return;

        PlayerMoneyManager playerMoney = player.GetComponent<PlayerMoneyManager>();
        playerMoney.AddMoneyDelayed(goldAmount, moneyDelaySeconds);

        playerInteracted = true;

        moneyParticle.Play();
        //chestOpenSFX.Play();
        
        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        if (moneyParticle != null)
        {
            // Wait for particle system to finish
            while (moneyParticle.isPlaying)
            {
                yield return null;
            }
        }

        if (postOpenDelaySeconds > 0f)
            yield return new WaitForSeconds(postOpenDelaySeconds);

        if (fadeDuration > 0)
        {
            float t = 0;
            var color = spriteRenderer.color;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                color.a = alpha;
                spriteRenderer.color = color;
                yield return null;
            }

            color.a = 0f;
            spriteRenderer.color = color;
        }

        gameObject.SetActive(false);
    }    
}
