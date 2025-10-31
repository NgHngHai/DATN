using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    // Energy variables
    [Header("Energy Settings")]
    public int maxEnergy = 20;
    public int currentEnergy = 0;

    // Skill list
    [Header("Skills")]
    public List<SkillDefinition> skills = new();

    // runtime
    private Dictionary<int, SkillDefinition> skillMap;
    private Dictionary<int, float> lastUsedTime;
    public int activeSkillId = 1;

    // PlayerPrefs key
    private const string activeSkillPref = "PlayerActiveSkill";

    // Event fired when active skill changes (UI can subscribe)
    public event Action<int> OnActiveSkillChanged;

    private void Awake()
    {
        BuildLookup();
        LoadSavedActiveSkill();
    }

    // Load skill map
    private void BuildLookup()
    {
        skillMap = new Dictionary<int, SkillDefinition>(skills.Count);
        lastUsedTime = new Dictionary<int, float>(skills.Count);

        foreach (var s in skills)
        {
            if (s == null) continue;

            var id = s.skillId;
            // Check for duplicate ids
            if (skillMap.ContainsKey(id))
            {
                Debug.LogWarning($"Duplicate skill id: {id} on skill '{s.skillName}'", this);
                continue;
            }

            skillMap.Add(id, s);
            lastUsedTime[id] = -Mathf.Infinity;
        }
    }

    // UI calls this to change active skill by id (int)
    public bool ChangeActiveSkill(int skillId)
    {
        if (skillMap == null) BuildLookup();

        if (!skillMap.TryGetValue(skillId, out var s))
            return false;

        if (!s.isUnlocked)
        {
            Debug.LogWarning($"Skill id {skillId} ('{s.skillName}') is locked.", this);
            return false;
        }

        if (activeSkillId == s.skillId) return true;

        activeSkillId = s.skillId;
        PlayerPrefs.SetInt(activeSkillPref, skillId);
        PlayerPrefs.Save();

        OnActiveSkillChanged?.Invoke(activeSkillId);
        return true;
    }

    // Overload - change by index (useful for UI list)
    public bool ChangeActiveSkillByIndex(int index)
    {
        if (index < 1 || index >= skills.Count) return false;
        var s = skills[index];
        if (s == null) return false;
        return ChangeActiveSkill(s.skillId);
    }

    public int GetActiveSkillId() => activeSkillId;

    private void LoadSavedActiveSkill()
    {
        if (skillMap == null) BuildLookup();

        if (PlayerPrefs.HasKey(activeSkillPref))
        {
            var id = PlayerPrefs.GetInt(activeSkillPref, int.MinValue);
            if (skillMap.TryGetValue(id, out var s) && s.isUnlocked)
            {
                activeSkillId = s.skillId;
                OnActiveSkillChanged?.Invoke(activeSkillId);
                return;
            }
        }

        // fallback: first unlocked skill, otherwise first skill in list
        foreach (var s in skills)
        {
            if (s != null && s.isUnlocked && s.skillId != 0)
            {
                activeSkillId = s.skillId;
                OnActiveSkillChanged?.Invoke(activeSkillId);
                return;
            }
        }
        if (skills.Count > 1)
        {
            activeSkillId = 1;
            OnActiveSkillChanged?.Invoke(activeSkillId);
        }
    }

    // Check if a given skill can be used (unlocked, enough energy, off cooldown)
    public bool CanUseSkill(int skillId)
    {
        if (skills[skillId] == null) return false;

        if (!skills[skillId].isUnlocked)
        {
            Debug.LogWarning($"CanUseSkill: skill '{skills[skillId].skillName}' (id={skillId}) is locked.", this);
            return false;
        }

        if (currentEnergy < skills[skillId].cost)
        {
            Debug.LogWarning($"CanUseSkill: not enough energy for '{skills[skillId].skillName}' (cost={skills[skillId].cost}, current={currentEnergy}).", this);
            return false;
        }

        if (lastUsedTime == null) lastUsedTime = new Dictionary<int, float>();
        if (!lastUsedTime.ContainsKey(skillId)) lastUsedTime[skillId] = -Mathf.Infinity;

        var timeSince = Time.time - lastUsedTime[skillId];
        if (timeSince < skills[skillId].cooldown)
        {
            Debug.LogWarning($"CanUseSkill: skill '{skills[skillId].skillName}' is on cooldown. {skills[skillId].cooldown - timeSince:F2}s remaining.", this);
            return false;
        }

        return true;
    }

    // Use the active skill by delegating to the SkillDefinition.Activate() method
    public bool UseActiveSkill()
    {
        if (!CanUseSkill(activeSkillId)) return false;

        // Call the SkillDefinition's Activate method. SkillDefinition (or its prefab's component)
        // is responsible for handling spawnPosition/spawnRotation if needed.
        try
        {
            skills[activeSkillId].Activate();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error activating skill '{skills[activeSkillId]?.skillName}': {ex}", this);
            return false;
        }

        currentEnergy = Mathf.Max(0, currentEnergy - skills[activeSkillId].cost);
        lastUsedTime[activeSkillId] = Time.time;
        return true;
    }

    // Use a skill by its id. Performs the same validation as UseActiveSkill but targets the given id.
    public bool UseSkillById(int skillId)
    {
        if (skillMap == null) BuildLookup();

        if (!skillMap.TryGetValue(skillId, out var s))
        {
            Debug.LogWarning($"UseSkillById: skill id {skillId} not found.", this);
            return false;
        }

        if (!CanUseSkill(skillId)) return false;

        try
        {
            s.Activate();
        }
        catch (Exception ex)
        {
            Debug.LogError($"UseSkillById: error activating skill '{s.skillName}' (id={skillId}): {ex}", this);
            return false;
        }

        currentEnergy = Mathf.Max(0, currentEnergy - s.cost);
        lastUsedTime[skillId] = Time.time;
        return true;
    }
}
