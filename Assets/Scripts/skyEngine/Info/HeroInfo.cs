using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;

public class HeroInfo
{
    public float HP;

    public string Name;
    public string Icon;
    public string Description;

    public int Level;
    public int Experience;

    public List<string> actions;
    public List<string> equipedActions;
    public List<string> equipedLoot;
    public Dictionary<string, SkillObject> skills;
    public Dictionary<string, int> units;
    public List<string> effectImmune;

    public static HeroInfo Make(string name, JSONNode data)
    {
        HeroInfo temp = new HeroInfo();

        temp.Name = name;
        temp.HP = data["HPAm"].AsFloat;
        temp.Icon = data["Icon"].Value;
        temp.Description = data["Description"].Value;


        temp.actions = new List<string>();
        JSONNode array = data["Actions"];
        for (int i = 0; i < array.Count; i++)
        {
            temp.actions.Add(array[i].Value);
        }

        temp.equipedActions = new List<string>();
        JSONNode earray = data["EquippedActions"];
        for (int i = 0; i < earray.Count; i++)
        {
            temp.equipedActions.Add(earray[i].Value);
        }

        temp.equipedLoot = new List<string>();
        JSONNode larray = data["Loot"];
        for (int i = 0; i < larray.Count; i++)
        {
            temp.equipedLoot.Add(larray[i].Value);
        }

        temp.effectImmune = new List<string>();
        if(data["EffectImmune"] != null)
        {
            JSONNode effects = data["EffectImmune"];
            for (int i = 0; i < effects.Count; i++)
            {
                temp.effectImmune.Add(effects[i].Value);
            }
        }
        

        temp.skills = new Dictionary<string, SkillObject>();
        JSONNode skill = data["Skills"];
        for (int i = 0; i < skill.Count; i++)
        {
            if (skill[i]["id"] == null)
            {
                Debug.LogError("Not set id for skill");
                continue;
            }

            temp.skills.Add(skill[i]["id"].Value, SkillObject.Make(skill[i]));
        }

        temp.units = new Dictionary<string, int>();
        JSONNode uni = data["Units"];
        for (int i = 0; i < uni.Count; i++)
        {
            temp.units.Add(uni[i]["unit"].Value, uni[i]["amount"].AsInt);
        }

        return temp;
    }

    public static HeroInfo CopyInfo(string data)
    {
        if(!IOM.HeroList.ContainsKey(data))
        {
            Debug.LogError("No such hero: " + data);
            return null;
        }

        HeroInfo hero = new HeroInfo();
        HeroInfo copyData = IOM.HeroList[data];

        hero.HP = copyData.HP;
        hero.Name = copyData.Name;
        hero.Icon = copyData.Icon;
        hero.Level = copyData.Level;
        hero.Experience = copyData.Experience;
        hero.actions = copyData.actions;
        hero.skills = copyData.skills;
        hero.units = copyData.units;
        hero.Description = copyData.Description;

        return hero;
    }

    public HeroInfo Copy()
    {
        HeroInfo hero = new HeroInfo();

        hero.HP = this.HP;
        hero.Name = this.Name;
        hero.Icon = this.Icon;
        hero.Level = this.Level;
        hero.Experience = this.Experience;
        hero.actions = this.actions;
        hero.skills = this.skills;
        hero.units = this.units;
        hero.Description = this.Description;

        return hero;
    }
}