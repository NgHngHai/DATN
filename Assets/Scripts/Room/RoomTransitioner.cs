using UnityEngine;

public class RoomTransitioner : MonoBehaviour
{
    private RoomManager roomManager;
    private Animator animator;
    private bool isTransitioning;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(RoomManager roomManager) => this.roomManager = roomManager;

    public void PlayTransitionAnim(EnterDirection enterDirection)
    {
        switch (enterDirection)
        {
            case EnterDirection.Left:
                animator.SetTrigger("leftToRight");
                break;
            case EnterDirection.Right:
                animator.SetTrigger("rightToLeft");
                break;
            case EnterDirection.Top:
                animator.SetTrigger("topToBottom");
                break;
            default:
                animator.SetTrigger("bottomToTop");
                break;
        }
    }

    public void StartLoadRoomRoutine()
    {
        animator.speed = 0;
        roomManager.StartLoadRoomRoutine();
    }

    public void UnfreezeAnim()
    {
        animator.speed = 1;
    }

}
