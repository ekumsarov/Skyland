using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStats : MonoBehaviour {

    Dictionary<string, Stat> _dic;

    bool active = false;

    public void Activate()
    {
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

    public void Add(string ID, Represent.Type type, string icon = null, bool mainStat = false, float curVal = 0, float maxVal = 0, bool neg = false,
         bool intF = true)
    {
        Stat temp = Stat.CreateOwn(ID, type, icon, mainStat, curVal, maxVal, neg, intF);
        _dic.Add(ID, temp);
    }
}
