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
    public bool isPassive = false;

    [SerializeField] private bool _isUnlocked = false;
    public bool isUnlocked
    {
        get => _isUnlocked;
        set
        {
            if (_isUnlocked == value) return;
            _isUnlocked = value;

            if (isPassive)
            {
                var player = _player != null ? _player : GameObject.FindWithTag("Player");
                EnablePassive(player, _isUnlocked);
            }
        }
    }

    // Reference
    [NonSerialized] private GameObject _player;

    public void Initialize(GameObject player)
    {
        _player = player;
        if (isPassive && isUnlocked)
        {
            EnablePassive(_player, true);
        }
    }

    public void Activate()
    {
        if (skillPrefab == null)
        {
            Debug.LogWarning($"Skill '{skillName}' has no GameObject assigned.");
            return;
        }

        var skillImpl = skillPrefab
            .GetComponentsInChildren<MonoBehaviour>()
            .OfType<ISkill>()
            .FirstOrDefault();

        if (skillImpl != null && !isPassive)
        {
            skillImpl.Activate();
        }
        else if (isPassive)
        {
            Debug.Log("this is a passive skill and cannot be activated directly.");
        }
        else
        {
            Debug.LogWarning($"GameObject '{skillName}' does not contain a component implementing ISkill.");
        }
    }

    // Toggle passive effect on/off for the player
    public void EnablePassive(GameObject owner, bool active)
    {
        if (!isPassive || skillPrefab == null || owner == null) return;

        var effect = skillPrefab
            .GetComponentsInChildren<MonoBehaviour>(true)
            .OfType<ISkillPassive>()
            .FirstOrDefault();

        effect?.SetPassiveActive(active, owner);
    }
}