using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class EffectEvents : MonoBehaviour
{
    [Header("Generic gameplay events for VFX/SFX")]
    public UnityEvent OnJump;
    public UnityEvent OnDashStart;
    public UnityEvent OnLand;
    public UnityEvent<Vector2> OnDamagedWithReaction;
    public UnityEvent OnDeath;

    // Helper invokers to be called by gameplay scripts
    public void InvokeJump() => OnJump?.Invoke();
    public void InvokeDashStart() => OnDashStart?.Invoke();
    public void InvokeLand() => OnLand?.Invoke();
    public void InvokeDamagedWithReaction(Vector2 dir) => OnDamagedWithReaction?.Invoke(dir);
    public void InvokeDeath() => OnDeath?.Invoke();
}
