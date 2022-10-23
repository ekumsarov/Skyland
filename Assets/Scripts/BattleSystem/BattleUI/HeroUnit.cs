using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;
using BattleEffects;
using Lodkod;

public class HeroUnit
{
    public float CurrentHP;
    public float HP;

    public string Name;
    public string Icon;
    public string Description;

    public int Level;
    public int Experience;

    public List<string> EquipedActions;
    public List<string> actions;
    public List<string> equipedLoot;
    public Dictionary<string, SkillObject> heroSkills;
    public Dictionary<string, SkillObject> skills;
    public Dictionary<string, SkillObject> effectSkill;
    public Dictionary<BattleUnitInfo, int> units;
    public List<string> effectsImmune;

    public static HeroUnit Make(HeroInfo info)
    {
        HeroUnit temp = new HeroUnit()
        {
            CurrentHP = info.HP,
            HP = info.HP,
            Name = info.Name,
            Icon = info.Icon,
            Description = info.Description,
            Level = info.Level,
            Experience = info.Experience,
            actions = info.actions,
            EquipedActions = info.equipedActions,
            equipedLoot = info.equipedLoot,
            heroSkills = info.skills,
            effectsImmune = info.effectImmune
        };

        if (temp.actions == null)
            temp.actions = new List<string>();

        if (temp.EquipedActions == null)
            temp.EquipedActions = new List<string>();
        
        temp._endTurn = false;
        
        temp._removeEffectList = new List<BattleEffect>();
        temp._effects = new List<BattleEffect>();
        temp.effectSkill = new Dictionary<string, SkillObject>();
        temp.skills = new Dictionary<string, SkillObject>();
        temp.units = new Dictionary<BattleUnitInfo, int>();
        foreach (var unit in info.units)
        {
            if (!IOM.BattleUnitInfoDic.ContainsKey(unit.Key))
                continue;

            temp.units.Add(IOM.BattleUnitInfoDic[unit.Key], unit.Value);
        }

        foreach (var skill in temp.heroSkills)
            temp.skills.Add(skill.Key, SkillObject.Make(skill.Value));

        temp.RebuildAllSkills();
        return temp;

    }

    void RebuildAllSkills()
    {
        foreach (var skill in this.skills)
        {
            if (this.heroSkills.ContainsKey(skill.Key))
                this.skills[skill.Key].SetupSkill(this.heroSkills[skill.Key]);

            foreach (var unit in units)
            {
                if (unit.Key.skills.ContainsKey(skill.Key))
                    this.skills[skill.Key].SetSkill(unit.Key.skills[skill.Key].Min * unit.Value, unit.Key.skills[skill.Key].Max * unit.Value);
            }
        }

        foreach (var effect in this.effectSkill)
        {
            if (this.skills.ContainsKey(effect.Value.Skill))
                this.skills[effect.Value.Skill].RemoveSkills(effect.Value.Min, effect.Value.Max);
        }
    }

    public void AddSkills(string skill, int min, int max)
    {
        if (!this.heroSkills.ContainsKey(skill))
        {
            Debug.LogError("No skill to set: " + skill);
            return;
        }

        this.heroSkills[skill].SetSkill(min, max);
        this.RebuildAllSkills();
    }

    public void AddSkills(SkillObject skill)
    {
        if (!this.heroSkills.ContainsKey(skill.ID))
        {
            this.heroSkills.Add(skill.ID, skill);
            return;
        }

        this.heroSkills[skill.ID].SetSkill(skill);
        this.RebuildAllSkills();
    }

    public void AddSkills(List<SkillObject> skills)
    {
        foreach(var skill in skills)
        {
            if (!this.heroSkills.ContainsKey(skill.ID))
                continue;

            this.heroSkills[skill.ID].SetSkill(skill);
        }
        
        this.RebuildAllSkills();
    }

    public void RemoveSkills(string skill, int min, int max)
    {
        if (skill.Equals("") || !this.heroSkills.ContainsKey(skill))
        {
            Debug.LogError("No skill to set: " + skill);
            return;
        }

        this.heroSkills[skill].RemoveSkills(min, max);
        this.RebuildAllSkills();
    }

    public void RemoveSkills(List<SkillObject> skills)
    {
        foreach (var skill in skills)
        {
            if (!this.heroSkills.ContainsKey(skill.ID))
                continue;

            this.heroSkills[skill.ID].RemoveSkills(skill);
        }

        this.RebuildAllSkills();
    }

    public void AddUnit(string type, int amount = 1)
    {
        BattleUnitInfo temp = this.units.FirstOrDefault(uni => uni.Key.Name.Equals("type")).Key;
        if(temp != null)
        {
            this.units[temp] += amount;
            this.RebuildAllSkills();
            return;
        }
    }

    public void RemoveUnit(string type, int amount = 1)
    {
        BattleUnitInfo temp = this.units.FirstOrDefault(uni => uni.Key.Name.Equals("type")).Key;
        if (temp != null)
        {
            this.units[temp] -= amount;
            this.RebuildAllSkills();
            return;
        }
    }

    #region Action
    public void AddAction(string actID)
    {
        if (!IOM.BattleActionInfoDic.ContainsKey(actID))
        {
            Debug.LogError("Where is no Battle Action ID: " + actID);
            return;
        }

        if (this.actions.Any(act => act.Equals(actID)) || this.EquipedActions.Any(act => act.Equals(actID)))
        {
            Debug.LogError("Unit already have Action ID: " + actID);
            return;
        }

        if (this.EquipedActions.Count < 8)
        {
            this.EquipedActions.Add(actID);
            return;
        }

        this.actions.Add(actID);
    }

    public void RemoveAction(string actID)
    {
        if(this.EquipedActions.Any(act => act.Equals(actID)))
        {
            this.EquipedActions.Remove(actID);
        }

        if (this.actions.Any(act => act.Equals(actID)))
        {
            this.actions.Remove(actID);
        }
    }

    public void EquipAction(string ID)
    {
        if(!this.actions.Any(act => act.Equals(ID)))
        {
            Debug.LogError("Where is no Battle Action ID: " + ID + " at hero: " + this.Name);
            return;
        }

        this.actions.Remove(ID);
        this.EquipedActions.Add(ID);
    }

    public void UnequipAction(string ID)
    {
        if(!this.EquipedActions.Any(act => act.Equals(ID)))
        {
            Debug.LogError("Where is no Equiped Battle Action ID: " + ID + " at hero: " + this.Name);
            return;
        }

        this.actions.Add(ID);
        this.EquipedActions.Remove(ID);
    }

    public void RemoveFromEquipAction(string ID)
    {

    }

    public void RemoveFromInventoryAction(string ID)
    {

    }

    #endregion

    public void AddSkillEffect(string effectID, string skill, int min, int max)
    {
        if (this.effectSkill == null)
            this.effectSkill = new Dictionary<string, SkillObject>();

        if(!this.effectSkill.ContainsKey(effectID))
        {
            this.effectSkill.Add(effectID, SkillObject.Make(skill, min, max));
            this.RebuildAllSkills();
        }
        else
            Debug.LogError("Effect skill already exist");
    }

    public void RemoveSkillEffect(string effectID)
    {
        if(this.effectSkill.ContainsKey(effectID))
        {
            this.effectSkill.Remove(effectID);
            this.RebuildAllSkills();
        }
    }

    #region BattleStats

    bool _endTurn;
    public bool EndTurn
    {
        get { return this._endTurn; }
        set
        {
            this._endTurn = value;
        }
    }

    int _dexResult;
    public int DexResult
    {
        get { return this._dexResult; }
    }
    public void MakeDexResult()
    {
        this._dexResult = this.skills["dexterity"].GetValue;
    }

    int _actionPoint = 3;
    int _currentActionPoint = 3;
    public void PrepareBattle()
    {
        this._currentActionPoint = this._actionPoint;
    }
    public int ActionPoints
    {
        get { return this._currentActionPoint; }
        set
        {
            if (value == 0)
                this._currentActionPoint = 0;

            this._currentActionPoint = value;
        }
    }

    public void Damage(int amount)
    {
        if (this.units.Count > 0)
        {
            if(amount > this.units.Count)
            {
                int index = 0;
                while(amount < 0)
                {
                    BattleUnitInfo uni = this.units.ElementAt(index).Key;
                    if (this.units[uni] > 0)
                        this.units[uni] -= 1;
                    else
                    {
                        this.units.Remove(uni);
                        uni = null;
                    }

                    index++;
                    if (index >= this.units.Count)
                        index = 0;

                    amount -= 1;
                }
            }
        }
        else
            this.CurrentHP -= amount;

        if(this.CurrentHP < 0)
        {
            this.CurrentHP = 0;
            ES.NotifySubscribers("UnitDead", null);
        }
    }

    public void HealHero(int amount)
    {
        if (this.CurrentHP + amount > this.HP)
        {
            this.CurrentHP = this.HP;
        }
        else
            this.CurrentHP += amount;
    }

    /// <summary>
    /// Effects on unit during battle
    /// </summary>
    public List<BattleEffect> _effects;
    List<BattleEffect> _removeEffectList;
    public void AddEffect(BattleEffect effect)
    {
        if (this._effects == null)
            this._effects = new List<BattleEffect>();

        this._effects.Add(effect);
        effect.Start();
    }

    public bool HasEffect(string effectID)
    {
        return this._effects.Any(eff => eff.ID.Equals(effectID));
    }

    public void EffectTurnStart()
    {
        for(int i = 0; i < this._effects.Count; i++)
        {
            this._effects[i].TurnStart();
        }
    }

    public void EffectTurnEnd()
    {
        for (int i = 0; i < this._effects.Count; i++)
        {
            this._effects[i].TurnEnd();

            if(this._effects[i].EffectDestroyed)
            {
                if (this._removeEffectList == null)
                    this._removeEffectList = new List<BattleEffect>();

                this._removeEffectList.Add(this._effects[i]);
            }
        }

        if (this._removeEffectList.Count > 0)
            this.EffectDestroy();
            
    }

    void EffectDestroy()
    {
        for(int i = 0; i < this._removeEffectList.Count; i++)
        {
            this._effects.Remove(this._removeEffectList[i]);
        }
        this._removeEffectList.Clear();
    }

    public bool EffectCancelAction(string action)
    {
        for(int i = 0; i < this._effects.Count; i++)
        {
            if (this._effects[i].CancelAction(action))
                return true;
        }

        return false;
    }

    #endregion
}
