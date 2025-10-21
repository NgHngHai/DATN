using System.Linq;
using System;
using UnityEngine;

[Serializable]
public class SkillDefinition
{
    [Tooltip("Unique id used for persistence/lookup")]
    public int skillId;

    public string skillName;
    public GameObject skillPrefab; 
    public float cooldown;
    public int cost;
    public bool isUnlocked = false;

    public void Activate()
    {
        // Skill activation logic goes here

        if (skillPrefab == null)
        {
            Debug.LogWarning($"Skill '{skillName}' has no GameObject assigned.");
            return;
        }

        var skillImpl = skillPrefab.GetComponentsInChildren<MonoBehaviour>().OfType<ISkill>().FirstOrDefault();

        if (skillImpl != null)
        {
            skillImpl.Activate();
        }
        else
        {
            Debug.LogWarning($"GameObject '{skillName}' does not contain a component implementing ISkill.");
        }
    }
}