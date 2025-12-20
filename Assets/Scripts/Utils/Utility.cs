using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Provides common helper methods for performing 2D physics operations and layer checks.
/// </summary>
public static class Utility
{
    public static bool IsRaycastHit(Vector2 origin, Vector2 direction, float distance, LayerMask mask)
    {
        return Physics2D.Raycast(origin, direction, distance, mask).collider != null;
    }

    public static bool IsLayerInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    public static bool IsGameObjectInLayer(GameObject gameObject, LayerMask mask)
    {
        return IsLayerInMask(gameObject.layer, mask);
    }

    public static T ConvertState<T>(object raw)
    {
        if (raw is JObject jObj)
            return jObj.ToObject<T>();

        return (T)raw;
    }
}
