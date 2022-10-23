using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarStats : MonoBehaviour
{

    Dictionary<string, Stat> _dic;

    bool active = false;

    public void Activate()
    {
        if (active)
            return;

        _dic = new Dictionary<string, Stat>();
        active = true;
    }

    public Dictionary<string, Stat> Stats
    {
        get
        {
            if (active == false)
                this.Activate();

            return this._dic;
        }
    }

    public void Add(string id, int count)
    {
        if(!Stats.ContainsKey(id))
        {
            if(!IOM.BattleUnitInfoDic.ContainsKey(id))
            {
                Debug.LogError("Not Found Battle Unit in config: " + id);
                return;
            }

            AddNew(id, Represent.Type.Simple, neg: false);
        }

        _dic[id].Count += count;
    }

    public void Remove(string id, int count)
    {
        if (!Stats.ContainsKey(id))
            return;

        Stats[id].Count -= count;
    }

    void AddNew(string ID, Represent.Type type, string icon = null, bool mainStat = false, float curVal = 0, float maxVal = 0, bool neg = false,
         bool intF = true)
    {
        Stat temp = Stat.CreateOwn(ID, type, icon, mainStat, curVal, maxVal, neg, intF);
        _dic.Add(ID, temp);
    }

    public Dictionary<string, int> GetAllUnits()
    {
        Dictionary<string, int> temp = new Dictionary<string, int>();

        foreach(var key in _dic.Keys)
        {
            temp.Add(key, (int)_dic[key].Count);
        }

        return temp;
    }
}