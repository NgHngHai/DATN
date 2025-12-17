using UnityEngine;

public class AutoDestroyOnAnimationEnd : MonoBehaviour
{
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
