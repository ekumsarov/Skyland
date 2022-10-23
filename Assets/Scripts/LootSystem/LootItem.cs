using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;
using System;

public class LootItem 
{
    public string ID;
    public string Icon;
    public int Amount = 1;
    public LS.LootQuantity Quantity = LS.LootQuantity.Simple;
    public LS.LootType type;
    public List<string> actions;
    public List<SkillObject> passive;
    public List<SkillObject> skillCheckHelp;
    public List<string> flagsOn;
    public List<string> flagsOff;

    public static LootItem Make(string name, JSONNode data)
    {
        LootItem temp = new LootItem();

        temp.ID = data["ID"].Value;
        temp.Icon = data["Icon"].Value;


        temp.actions = new List<string>();
        if(data["Actions"] != null)
        {
            JSONNode array = data["Actions"];
            for (int i = 0; i < array.Count; i++)
            {
                temp.actions.Add(array[i].Value);
            }
        }

        temp.flagsOn = new List<string>();
        if (data["FlagsOn"] != null)
        {
            JSONNode flo = data["FlagsOn"];
            for (int i = 0; i < flo.Count; i++)
            {
                temp.flagsOn.Add(flo[i].Value);
            }
        }
        

        temp.flagsOff = new List<string>();
        if (data["FlagsOff"] != null)
        {
            JSONNode flof = data["FlagsOff"];
            for (int i = 0; i < flof.Count; i++)
            {
                temp.flagsOff.Add(flof[i].Value);
            }
        }

        temp.passive = new List<SkillObject>();
        if (data["PassiveSkills"] != null)
        {
            JSONNode flof = data["PassiveSkills"];
            for (int i = 0; i < flof.Count; i++)
            {
                temp.passive.Add(SkillObject.Make(flof[i]));
            }
        }

        temp.skillCheckHelp = new List<SkillObject>();
        if (data["ActiveSkills"] != null)
        {
            JSONNode flof = data["ActiveSkills"];
            for (int i = 0; i < flof.Count; i++)
            {
                temp.passive.Add(SkillObject.Make(flof[i]));
            }
        }

        temp.Quantity = LS.LootQuantity.Simple;
        if(data["Quantity"] != null)
            temp.Quantity = (LS.LootQuantity)Enum.Parse(typeof(LS.LootQuantity), data["Quantity"].Value);

        temp.type = LS.LootType.Weapon;
        if(data["type"] != null)
            temp.type = (LS.LootType)Enum.Parse(typeof(LS.LootType), data["type"].Value);

        return temp;
    }
}
