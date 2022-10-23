using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using SimpleJSON;

public class iStat {

    public string type;
    public float amount;

    
    /*
     * 
     * Resources work
     * 
     */
    public static iStat Create()
    {
        iStat obj = new iStat
        {
            type = "Wood",
            amount = 0
        };

        return obj;
    }

    public static iStat Create(StatType t)
    {
        iStat obj = new iStat
        {
            type = t.ToString(),
            amount = 0
        };

        return obj;
    }

    public static iStat Create(StatType t, float a)
    {
        iStat obj = new iStat
        {
            type = t.ToString(),
            amount = a
        };

        return obj;
    }

    public static iStat Create(string t, float a)
    {
        iStat obj = new iStat
        {
            type = t,
            amount = a
        };

        return obj;
    }


    public static List<iStat> createResList(JSONNode data)
    {
        List<iStat> temp = new List<iStat>();
        foreach(var key in data.Keys)
        {
            temp.Add(iStat.Create(key, data[key].AsFloat));
        }

        return temp;
    }


    public static string GetCostListInfo(string BuildType)
    {
        return LocalizationManager.Get("BuildCostTemplate", LocalizationManager.Get(BuildType), IOM.BuildInfoDic[BuildType][0].Cost[0].amount, IOM.BuildInfoDic[BuildType][0].Cost[1].amount, IOM.BuildInfoDic[BuildType][0].Cost[2].amount, IOM.BuildInfoDic[BuildType][0].Cost[3].amount);
    }

    public static bool CompareList(List<iStat> cost, List<iStat> list)
    {
        foreach(var res in cost)
        {
            foreach(var lis in list)
            {
                if (lis.type != res.type)
                    continue;

                if (lis.amount != res.amount)
                    return false;
            }
        }

        return true;
    }


    // start with set
    public void setType(StatType type)
    {
        this.type = type.ToString();
    }

    public void setType(string type)
    {
        this.type = type;
    }

    public string getType()
    {
        return this.type.ToString();
    }





    public void setAmount(int amount)
    {
        this.amount = amount;
    }

    public float getAmount()
    {
        return this.amount;
    }

    public static bool CheckList(List<iStat> res)
    {
        for (int i = 0; i < res.Count; i++)
        {
            if (SM.Stats.ContainsKey(res[i].type) && SM.Stats[res[i].type].Count < res[i].amount)
                return false;
        }


        return true;
    }

    public static bool CheckTakeList(List<iStat> res)
    {
        for (int i = 0; i < res.Count; i++)
        {
            if (SM.Stats.ContainsKey(res[i].type) && SM.Stats[res[i].type].Count < res[i].amount)
                return false;
        }

        for (int i = 0; i < res.Count; i++)
        {
            if(SM.Stats.ContainsKey(res[i].type))
                SM.Stats[res[i].getType()].Count -= res[i].amount;
        }

        return true;
    }

    public static bool CheckAdd(string type, int amount)
    {
        string res = type;
        if (SM.Stats.ContainsKey(res) && SM.Stats[res].Count + amount > SM.Stats[res].Max)
            return false;
        return true;
    }

    public static bool CheckResource(string type, int amount)
    {
        if(!SM.Stats.ContainsKey(type))
        {
            Debug.LogError("No Stat type: " + type);
            return false;
        }

        return SM.Stats[type].Count > amount;
    }

    public string GetStatString()
    {
        return LocalizationManager.Get(type + "Icon") + " " + this.amount;
    }
}
