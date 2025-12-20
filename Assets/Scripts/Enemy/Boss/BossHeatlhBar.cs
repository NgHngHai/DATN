using System;
using UnityEngine;
using UnityEngine.UI;

public class BossHeatlhBar : MonoBehaviour
{
    [SerializeField] private Image currentHp;
    [SerializeField] private Image recentHp;
    [SerializeField] float recentDelay = 0.5f;
    [SerializeField] float recentDecreaseSpeed = 1.5f;

    private Animator animator;
    private Boss boss;
    float targetFill = 1f;
    float delayTimer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (boss == null) return;

        float current = boss.GetCurrentHealth();
        if (current < 0) Destroy(gameObject);

        float max = boss.GetMaxHealth();
        targetFill = Mathf.Clamp01(current / max);

        currentHp.fillAmount = targetFill;

        if (recentHp.fillAmount > targetFill)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= recentDelay)
            {
                recentHp.fillAmount = Mathf.MoveTowards(
                  recentHp.fillAmount, targetFill, Time.deltaTime * recentDecreaseSpeed
                );
            }
        }
        else
        {
            delayTimer = 0f;
            recentHp.fillAmount = targetFill;
        }
    }

    public void Initialize(Boss boss)
    {
        animator.SetBool("appear", true);
        this.boss = boss;
    }
}
