using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Lodkod;
using SimpleJSON;

// LOOT SYSTEM
public class LS
{
    public enum LootQuantity { Simple = 0, Reusable = 1 }

    Dictionary<string, LootItem> bag;
    Dictionary<string, Dictionary<LootType ,LootItem>> equip;

    private static LS instance = null;
    public static void NewGame()
    {
        if (LS.instance != null)
            LS.instance = null;

        LS.instance = new LS
        {
            bag = new Dictionary<string, LootItem>(),
            equip = new Dictionary<string, Dictionary<LootType, LootItem>>()
        };
    }

    public static void NewGame(SimpleJSON.JSONNode node)
    {
        if (LS.instance != null)
            LS.instance = null;

        LS.instance = new LS
        {
            bag = new Dictionary<string, LootItem>(),
            equip = new Dictionary<string, Dictionary<LootType, LootItem>>()
        };

        if(node["LootBag"] != null)
        {
            JSONArray lootArray = node["LootBag"].AsArray;
            if(lootArray != null && lootArray.Count > 0)
            {
                for(int i = 0; i < lootArray.Count; i++)
                {
                    LS.instance.bag.Add(lootArray[i], IOM.GetLootItem(lootArray[i]));
                }
            }
        }
    }

    public static void AddItem(string id, LS.LootType type, string to = "Player")
    {
        LootItem item = IOM.GetLootItem(id);

        if (to.Equals("bag"))
        {
            LS.instance.bag.Add(id, item);
            return;
        }

        if(LS.instance.equip[to][type] != null)
            LS.instance.bag.Add(id, item);
        else
        {
            LS.instance.equip[to][type] = item;
            foreach (var act in item.actions)
                GM.Player.Group.AddAction(to, act);

            foreach (var flag in item.flagsOn)
                SM.SetFlag(flag);

            foreach (var flag in item.flagsOff)
                SM.SetFlag(flag, false);

            GM.Player.Group.AddSkills(to, item.passive);
        }
            
    }

    public static bool EquipItem(string id, LS.LootType type, string to = "Player")
    {
        if(!LS.instance.bag.ContainsKey(id))
            return false;

        LootItem item = LS.instance.bag[id];
        LS.instance.bag.Remove(id);

        if(LS.instance.equip[to][type] != null)
        {
            UnequipItem(type, to);
        }

        LS.instance.equip[to][type] = item;

        foreach (var act in item.actions)
            GM.Player.Group.AddAction(to, act);

        foreach (var flag in item.flagsOn)
            SM.SetFlag(flag);

        foreach (var flag in item.flagsOff)
            SM.SetFlag(flag, false);

        GM.Player.Group.AddSkills(to, item.passive);

        return true;
    }

    public static void UnequipItem(LootType type, string to = "Player")
    {
        if (LS.instance.equip[to][type] != null)
        {
            foreach (var act in LS.instance.equip[to][type].actions)
                GM.Player.Group.RemoveAction(act, to);

            foreach (var flag in LS.instance.equip[to][type].flagsOn)
                SM.SetFlag(flag, false);

            foreach (var flag in LS.instance.equip[to][type].flagsOff)
                SM.SetFlag(flag, true);

            GM.Player.Group.RemoveSkills(to, LS.instance.equip[to][type].passive);

            LS.instance.bag.Add(LS.instance.equip[to][type].ID, LS.instance.equip[to][type]);
            LS.instance.equip[to][type] = null;
        }
    }

    public static void RemoveItem(string id)
    {
        if (LS.instance.bag.ContainsKey(id))
        {
            LS.instance.bag.Remove(id);
            return;
        }

        foreach (var hero in LS.instance.equip)
        {
            if (hero.Value[LootType.Weapon].ID.Equals(id))
            {
                foreach (var act in hero.Value[LootType.Weapon].actions)
                    GM.Player.Group.RemoveAction(act, hero.Key);

                foreach (var flag in hero.Value[LootType.Weapon].flagsOn)
                    SM.SetFlag(flag, false);

                foreach (var flag in hero.Value[LootType.Weapon].flagsOff)
                    SM.SetFlag(flag, true);

                GM.Player.Group.RemoveSkills(hero.Key, hero.Value[LootType.Weapon].passive);

                hero.Value[LootType.Weapon] = null;
                return;
            }

            if (hero.Value[LootType.SecondWeapon].ID.Equals(id))
            {
                foreach (var act in hero.Value[LootType.SecondWeapon].actions)
                    GM.Player.Group.RemoveAction(act, hero.Key);

                foreach (var flag in hero.Value[LootType.SecondWeapon].flagsOn)
                    SM.SetFlag(flag, false);

                foreach (var flag in hero.Value[LootType.SecondWeapon].flagsOff)
                    SM.SetFlag(flag, true);

                GM.Player.Group.RemoveSkills(hero.Key, hero.Value[LootType.SecondWeapon].passive);

                hero.Value[LootType.SecondWeapon] = null;
                return;
            }

            if (hero.Value[LootType.Amulet].ID.Equals(id))
            {
                foreach (var act in hero.Value[LootType.Amulet].actions)
                    GM.Player.Group.RemoveAction(act, hero.Key);

                foreach (var flag in hero.Value[LootType.Amulet].flagsOn)
                    SM.SetFlag(flag, false);

                foreach (var flag in hero.Value[LootType.Amulet].flagsOff)
                    SM.SetFlag(flag, true);

                GM.Player.Group.RemoveSkills(hero.Key, hero.Value[LootType.Amulet].passive);

                hero.Value[LootType.Amulet] = null;
                return;
            }

            if (hero.Value[LootType.Armor].ID.Equals(id))
            {
                foreach (var act in hero.Value[LootType.Amulet].actions)
                    GM.Player.Group.RemoveAction(act, hero.Key);

                foreach (var flag in hero.Value[LootType.Amulet].flagsOn)
                    SM.SetFlag(flag, false);

                foreach (var flag in hero.Value[LootType.Amulet].flagsOff)
                    SM.SetFlag(flag, true);

                GM.Player.Group.RemoveSkills(hero.Key, hero.Value[LootType.Amulet].passive);

                hero.Value[LootType.Amulet] = null;
                return;
            }

            if (hero.Value[LootType.Extra].ID.Equals(id))
            {
                foreach (var act in hero.Value[LootType.Amulet].actions)
                    GM.Player.Group.RemoveAction(act, hero.Key);

                foreach (var flag in hero.Value[LootType.Amulet].flagsOn)
                    SM.SetFlag(flag, false);

                foreach (var flag in hero.Value[LootType.Amulet].flagsOff)
                    SM.SetFlag(flag, true);

                GM.Player.Group.RemoveSkills(hero.Key, hero.Value[LootType.Amulet].passive);

                hero.Value[LootType.Amulet] = null;
                return;
            }
        }

    }

    public static void PartyUpdate()
    {
        foreach(HeroUnit hero in GM.Player.Group.GetUnits())
        {
            if(!LS.instance.equip.ContainsKey(hero.Name))
            {
                LS.instance.equip.Add(hero.Name, new Dictionary<LootType, LootItem>());

                
                LS.instance.equip[hero.Name].Add(LootType.Weapon, null);
                LS.instance.equip[hero.Name].Add(LootType.SecondWeapon, null);
                LS.instance.equip[hero.Name].Add(LootType.Amulet, null);
                LS.instance.equip[hero.Name].Add(LootType.Armor, null);
                LS.instance.equip[hero.Name].Add(LootType.Extra, null);

                if (hero.equipedLoot != null && hero.equipedLoot.Count > 0)
                {
                    for(int i = 0; i < hero.equipedLoot.Count; i++)
                    {
                        LootItem item = IOM.GetLootItem(hero.equipedLoot[i]);
                        if(item == null)
                        {
                            Debug.LogError("No item ID: " + hero.equipedLoot[i]);
                            continue;
                        }

                        LS.instance.equip[hero.Name][item.type] = item;

                        foreach (var act in item.actions)
                            GM.Player.Group.AddAction(hero.Name, act);

                        foreach (var flag in item.flagsOn)
                            SM.SetFlag(flag);

                        foreach (var flag in item.flagsOff)
                            SM.SetFlag(flag, false);

                        GM.Player.Group.AddSkills(hero.Name, item.passive);
                    }
                }
            }
        }
    }

    public enum LootType
    {
        Weapon,
        Amulet,
        SecondWeapon,
        Armor,
        Extra
    }

    public static bool HasItem(string item)
    {
        if (LS.instance.bag.ContainsKey(item))
            return true;

        foreach(var hero in LS.instance.equip.Values)
        {
            if (hero[LootType.Weapon] != null && hero[LootType.Weapon].ID.Equals(item))
                return true;

            if (hero[LootType.SecondWeapon] != null && hero[LootType.SecondWeapon].ID.Equals(item))
                return true;

            if (hero[LootType.Amulet] != null && hero[LootType.Amulet].ID.Equals(item))
                return true;

            if (hero[LootType.Armor] != null && hero[LootType.Armor].ID.Equals(item))
                return true;

            if (hero[LootType.Extra] != null && hero[LootType.Extra].ID.Equals(item))
                return true;
        }

        return false;
    }

    public static LootItem GetItem(string item)
    {
        if (LS.instance.bag.ContainsKey(item))
            return LS.instance.bag[item];

        foreach (var hero in LS.instance.equip.Values)
        {
            if (hero[LootType.Weapon] != null && hero[LootType.Weapon].ID.Equals(item))
                return hero[LootType.Weapon];

            if (hero[LootType.SecondWeapon] != null && hero[LootType.SecondWeapon].ID.Equals(item))
                return hero[LootType.SecondWeapon];

            if (hero[LootType.Amulet] != null && hero[LootType.Amulet].ID.Equals(item))
                return hero[LootType.Amulet];

            if (hero[LootType.Armor] != null && hero[LootType.Armor].ID.Equals(item))
                return hero[LootType.Armor];

            if (hero[LootType.Extra]!= null && hero[LootType.Extra].ID.Equals(item))
                return hero[LootType.Extra];
        }

        return null;
    }

    public static bool HasHeroItem(LS.LootType where, string hero)
    {
        if (LS.instance.equip[hero][where] != null)
            return true;

        return false;
    }

    public static LootItem GetHeroItem(LS.LootType where, string hero)
    {
        return LS.instance.equip[hero][where];
    }

    public static Dictionary<string, LootItem> Bag
    {
        get { return LS.instance.bag; }
    }
}
