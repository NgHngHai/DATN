using UnityEngine;

public class OneTimeAnimation : MonoBehaviour
{
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
