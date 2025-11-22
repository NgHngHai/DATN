using UnityEngine;

public interface ISkillPassive
{
    // active == true -> apply; false -> remove
    void SetPassiveActive(bool active, GameObject owner);
}
