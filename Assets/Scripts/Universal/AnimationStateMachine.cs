using UnityEngine;

public class AnimationStateMachine
{
    public AnimationState currentState;

    // Khởi tạo state đầu tiên
    public void Initialize(AnimationState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    // Đổi sang state mới
    public void ChangeState(AnimationState newState)
    {
        if (newState == null) return;

        currentState?.Exit();                      // Thoát khỏi state cũ
        currentState = newState;                   // Gán state mới
        currentState.Enter();                      // Gọi hàm vào state mới
    }

    // Gọi update của state hiện tại mỗi frame
    public void UpdateCurrentState()
    {
        currentState?.Update();
    }
}

public class AnimationState
{
    protected Entity entity;                       // Entity mà state này thuộc về (vd: Player, Enemy...)
    protected AnimationStateMachine animStateMachine; // Tham chiếu đến FSM để đổi state khi cần
    protected Animator animator;                   // Dùng để điều khiển animation qua Animator Parameter

    protected float stateTimer;                    // Thời gian state còn lại (nếu cần giới hạn thời gian)
    protected bool triggerCalled;                  // Cờ báo hiệu animation đã phát xong (set qua Animation Event)
    protected string animParamName;                // Tên parameter trong Animator
    protected bool isBool;                         // Kiểu parameter (bool hoặc trigger)

    public AnimationState(Entity entity, string animParamName, bool isBool)
    {
        this.entity = entity;
        this.animParamName = animParamName;
        this.isBool = isBool;
        animator = entity.animator;                // Lấy animator từ entity cha
        animStateMachine = entity.animStateMachine;// Lấy FSM từ entity cha
    }

    // Khi vào state
    public virtual void Enter()
    {
        if (isBool)
            animator.SetBool(animParamName, true);      // Bật parameter bool tương ứng trong Animator
        else
            animator.SetTrigger(animParamName);          // Kích hoạt parameter trigger tương ứng trong Animator

        triggerCalled = false;                     // Reset flag trigger
    }

    // Gọi mỗi frame khi đang ở state này
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;              // Giảm timer (nếu state có thời gian)
    }

    // Khi rời state
    public virtual void Exit()
    {
        if (isBool)
            animator.SetBool(animParamName, false);     // Tắt parameter bool
    }

    // Được gọi khi animation event báo hiệu anim kết thúc
    public void CallAnimationTrigger()
    {
        triggerCalled = true;                      // Đánh dấu rằng animation đã xong
    }
}