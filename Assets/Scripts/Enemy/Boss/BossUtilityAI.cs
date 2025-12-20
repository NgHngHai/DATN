using System.Collections.Generic;
using UnityEngine;

public class BossUtilityAI : MonoBehaviour
{
    [SerializeField] private Transform handSlamPoint;
    [SerializeField] private float thinkInterval = 1f;

    private Boss boss;
    private List<BossBehavior> behaviors = new List<BossBehavior>();
    private bool isAIStart;
    private float thinkTimer;

    public void Initialize(Boss boss)
    {
        this.boss = boss;
        BuildBehaviors();
    }

    private void Update()
    {
        if (!IsBossRest() || !isAIStart) return;

        thinkTimer -= Time.deltaTime;

        if (thinkTimer < 0)
            ThinkAndDecide();
    }

    private bool IsBossRest()
    {
        return boss.logicStateMachine.currentState == boss.restState;
    }

    private void ThinkAndDecide()
    {
        float bestScore = 0f;
        BossBehavior bestBehavior = null;

        foreach (var behavior in behaviors)
        {
            float score = behavior.BaseEvaluate();
            if (score > bestScore)
            {
                bestScore = score;
                bestBehavior = behavior;
            }
        }

        if (bestBehavior != null)
        {
            bestBehavior.BaseExecute();
        }

        thinkTimer = thinkInterval;
    }

    private void BuildBehaviors()
    {
        behaviors.Clear();

        behaviors.Add(new BossHandAttackBehavior(boss, 2f, handSlamPoint, 3f));
        behaviors.Add(new BossDashAttackBehavior(boss, 3f, 4f));
        behaviors.Add(new BossMoveBackBehavior(boss, 2f, 6f));
        behaviors.Add(new BossNukeAttackBehavior(boss, 15f));
    }

    public void AcknowledgePhaseTwo()
    {
        behaviors.Add(new BossAdjustBowstringAggressiveBehavior(boss, 1f, 3f, 8f));
    }

    public void StartAI() => isAIStart = true;
    public void StopAI() => isAIStart = false;


}
