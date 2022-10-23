using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Lodkod;
using SimpleJSON;

// LOOT SYSTEM
public class BS
{
    Dictionary<BuffType, Buff> buffs;

    private static BS instance = null;
    public static void NewGame()
    {
        if (BS.instance != null)
            BS.instance = null;

        BS.instance = new BS
        {
            buffs = new Dictionary<BuffType, Buff>()
        };
    }

    public enum BuffType
    {
        Stat,
        Hero,
        Build
    }

    public static void NewBuff(BuffType type, string id, JSONNode data)
    {

    }
}

public class Buff : ObjectID
{
    string ObjectID = "nil";

    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    BS.BuffType type = BS.BuffType.Stat;
    BS.BuffType Type
    {
        get { return type; }
        set { this.type = value; }
    }

    public virtual void Make(string id, JSONNode data)
    {

    }
}