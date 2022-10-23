using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroGroup 
{
    List<HeroUnit> _units;

    public HeroGroup()
    {
        _units = new List<HeroUnit>();
    }

    public void AddHeroes(Dictionary<string, HeroInfo> data)
    {
        foreach(var unit in data)
        {
            this._units.Add(HeroUnit.Make(unit.Value));
        }
    }

    public List<HeroUnit> GetUnits()
    {
        return this._units;
    }

    #region Hero manage
    public virtual void AddNewHero(string hero)
    {
        if(!IOM.HeroList.ContainsKey(hero))
        {
            Debug.LogError("No such hero info: " + hero);
            return;
        }

        this._units.Add(HeroUnit.Make(IOM.HeroList[hero]));
    }

    public void RemoveHero(string hero)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit != null)
            this._units.Remove(unit);

        unit = null;
    }

    public void RemoveAllHeroes()
    {
        this._units.Clear();
    }

    #endregion

    #region Unit manage
    public void AddUnit(string hero, string unitType, int amount = 1)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.AddUnit(unitType, amount);
    }

    public void RemoveUnit(string hero, string unitType, int amount = 1)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.RemoveUnit(unitType, amount);
    }
    #endregion

    #region Skill manage
    public void AddSkills(string hero, string skill, int min, int max)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.AddSkills(skill, min, max);
    }

    public void AddSkills(string hero, SkillObject skill)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.AddSkills(skill);
    }

    public void AddSkills(string hero, List<SkillObject> skills)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.AddSkills(skills);
    }

    public void RemoveSkills(string hero, string skill, int min, int max)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.RemoveSkills(skill, min, max);
    }

    public void RemoveSkills(string hero, List<SkillObject> skills)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.RemoveSkills(skills);
    }
    #endregion

    #region Action manage
    public void AddAction(string hero, string actID)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.AddAction(actID);
    }

    public void RemoveAction(string hero, string actID)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.RemoveAction(actID);
    }

    public void EquipAction(string hero, string actID)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.EquipAction(actID);
    }

    public void UnequipAction(string hero, string actID)
    {
        HeroUnit unit = this._units.FirstOrDefault(uni => uni.Name.Equals(hero));
        if (unit == null)
        {
            Debug.LogError("No such hero in group: " + hero);
            return;
        }

        unit.UnequipAction(actID);
    }
    #endregion
    // TODO :
    /// Need to complete deal damage
    public void Damage(string hero, int amount)
    {
        if(hero.Equals("Group"))
        {
            for(int i = 0; i < this._units.Count; i++)
            {
                this._units[i].CurrentHP -= amount;
            }
        }
        else
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                if(this._units[i].Name.Equals(hero))
                {
                    this._units[i].CurrentHP -= amount;
                    break;
                }
                    
            }
        }
    }

    public void SetCurHP(string hero, int val)
    {
        if (hero.Equals("Group"))
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                this._units[i].CurrentHP = val;
            }
        }
        else
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                if (this._units[i].Name.Equals(hero))
                {
                    this._units[i].CurrentHP = val;
                    break;
                }

            }
        }
    }

    public void HealHero(string hero, int val)
    {
        if (hero.Equals("Group"))
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                this._units[i].CurrentHP += val;
            }
        }
        else
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                if (this._units[i].Name.Equals(hero))
                {
                    this._units[i].CurrentHP += val;
                    break;
                }

            }
        }
    }

    public void RestoreCurHP(string hero)
    {
        if (hero.Equals("Group"))
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                this._units[i].CurrentHP = this._units[i].HP;
            }
        }
        else
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                if (this._units[i].Name.Equals(hero))
                {
                    this._units[i].CurrentHP = this._units[i].HP;
                    break;
                }

            }
        }
    }

    public void SetHP(string hero, int val)
    {
        if (hero.Equals("Group"))
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                this._units[i].HP = val;
            }
        }
        else
        {
            for (int i = 0; i < this._units.Count; i++)
            {
                if (this._units[i].Name.Equals(hero))
                {
                    this._units[i].HP = val;
                    break;
                }

            }
        }
    }
}
