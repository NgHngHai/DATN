using UnityEngine;

public class AnimationStateMachine
{
    public AnimationState currentState;

    public void Initialize(AnimationState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(AnimationState newState)
    {
        if (newState == null) return;

        currentState?.Exit();                      
        currentState = newState;                   
        currentState.Enter();                      
    }

    public void UpdateCurrentState()
    {
        currentState?.Update();
    }
}

public class AnimationState
{
    protected Entity entity;                       
    protected AnimationStateMachine animStateMachine; 
    protected Animator animator;                   

    protected float stateTimer;                    
    protected bool triggerCalled;                  
    protected string animParamName;                
    protected bool isBool;                         

    public AnimationState(Entity entity, string animParamName, bool isBool = true)
    {
        this.entity = entity;
        this.animParamName = animParamName;
        this.isBool = isBool;
        animator = entity.animator;                
        animStateMachine = entity.animStateMachine;
    }

    public virtual void Enter()
    {
        if (isBool)
            animator.SetBool(animParamName, true);      
        else
            animator.SetTrigger(animParamName);          // Kích hoạt parameter trigger tương ứng trong Animator

        triggerCalled = false;                     
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;              
    }

    public virtual void Exit()
    {
        if (isBool)
            animator.SetBool(animParamName, false);     
    }

    public void CallAnimationTrigger()
    {
        triggerCalled = true;                      
    }

    public bool IsTriggerCalled() => triggerCalled;
}