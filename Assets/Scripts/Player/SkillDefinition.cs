using System.Linq;
using System;
using UnityEngine;
using Unity.VisualScripting;

[Serializable]
public class SkillDefinition
{
    // Skill properties
    [Tooltip("Unique id used for persistence/lookup")]
    public int skillId;

    public string skillName;
    public GameObject skillPrefab; 
    public float cooldown;
    public int cost;
    public bool isPassive = false;
    public int unlockCost = 50;

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
    private PlayerMoneyManager _moneyManager;

    private void Start()
    {
        _moneyManager = _player.GetComponent<PlayerMoneyManager>();
    }

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

    /// <summary>
    /// Spend unlock cost to unlock this skill.
    /// </summary>
    /// 
    public bool UnlockSkill(int cost)
    {
        if (isUnlocked) return true;
        if (_moneyManager == null)
        {
            Debug.LogWarning("PlayerMoneyManager reference is missing.");
            return false;
        }
        if (_moneyManager.TrySpend(cost))
        {
            isUnlocked = true;
            Debug.Log($"Skill '{skillName}' unlocked.");
            return true;
        }
        else
        {
            Debug.LogWarning($"Not enough money to unlock skill '{skillName}'.");
            return false;
        }
    }
}