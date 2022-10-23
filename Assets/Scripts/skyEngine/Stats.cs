using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using System;

public class Stat : ObjectID
{
    string ObjectID;
    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    StatItem guiItem = null;
    Represent represent;

    float _curVal;
    float _maxVal;
    float _deltaVal;

    bool intFlag = false;
    bool negative = false;
    bool active = true;
    bool ChangeVisibilityOnEmpty = false;

    string NotifyFunction;

    string _icon;

    #region Base Function
    public static void Create(string ID, Represent.Type type = Represent.Type.Simple, string icon = null, bool mainStat = false, float curVal = 0, float maxVal = 0, bool neg = false,
         bool intF = true, bool isProd = false, bool changeOnEmpty = false)
    {
        Stat temp = new Stat();

        temp.ID = ID;
        temp.NotifyFunction = ID + "Changed";

        temp.negative = neg;
        temp.intFlag = intF;

        temp._curVal = curVal;
        temp._maxVal = maxVal;

        temp.productPattern = LocalizationManager.Get("ProductionInDayPattern");
        temp.consumptionPattern = LocalizationManager.Get("ConsumptionInDayLabel");

        if (isProd)
            temp.initProduction();

        if (type == Represent.Type.Slash && maxVal == 0)
            type = Represent.Type.Simple;
        
        temp.represent = new Represent
        {
            Format = type
        };

        if (!icon.IsNullOrEmpty())
        {
            temp.guiItem = StatItem.Create(ID, icon, temp.represent.Formatter(temp._curVal, temp._maxVal), mainStat);
            temp.guiItem.SetTextDelegate(temp.GetText);
            temp._icon = icon;
        }

        temp.ChangeVisibilityOnEmpty = changeOnEmpty;
        if (temp._curVal < 1 && temp.ChangeVisibilityOnEmpty)
            temp.Active = false;
            

        SM.Stats.Add(ID, temp);
    }

    public static Stat CreateOwn(string ID, Represent.Type type = Represent.Type.Simple, string icon = null, bool mainStat = false, float curVal = 0, float maxVal = 0, bool neg = false,
         bool intF = true, bool isProd = false, bool changeOnEmpty = false)
    {
        Stat temp = new Stat();

        temp.ID = ID;
        temp.NotifyFunction = ID + "Changed";

        temp.negative = neg;
        temp.intFlag = intF;

        temp._curVal = curVal;
        temp._maxVal = maxVal;

        if (isProd)
            temp.initProduction();

        temp.represent = new Represent
        {
            Format = type
        };

        if (!icon.IsNullOrEmpty())
        {
            temp.guiItem = StatItem.Create(ID, icon, temp.represent.Formatter(temp._curVal, temp._maxVal), mainStat);
            temp.guiItem.SetTextDelegate(temp.GetText);
        }

        temp.ChangeVisibilityOnEmpty = changeOnEmpty;
        if (temp._curVal < 1 && temp.ChangeVisibilityOnEmpty)
            temp.Active = false;

        return temp;
    }

    public string Icon
    {
        get { return this._icon; }
    }

    public string CallFunction
    {
        get
        { return this.NotifyFunction; }
    }

    public float Count
    {
        get
        {
            return _curVal;
        }
        set
        {
            if (!negative)
                _curVal = Mathf.Max(0, value);
            else
                _curVal = value;

            if (_maxVal > 0)
                _curVal = Mathf.Min(_maxVal, _curVal);

            ES.NotifySubscribers(TriggerType.StatChanged.ToString(), "");
            ES.NotifySubscribers(NotifyFunction, "");

            if (_curVal == 0 && this.active && ChangeVisibilityOnEmpty)
                this.Active = false;

            if (_curVal > 1 && this.active == false)
                this.Active = true;

            if (guiItem != null)
                guiItem.Text = represent.Formatter(_curVal, _maxVal);
        }
    }

    public int Max
    {
        get
        { return (int)_maxVal; }
        set
        {
            ES.NotifySubscribers(TriggerType.StatChanged.ToString(), "");
            ES.NotifySubscribers(NotifyFunction, "");

            this._maxVal = value;
            if (guiItem != null)
                guiItem.Text = represent.Formatter(_curVal, _maxVal);
        }
    }

    public bool Active
    {
        get { return this.active; }
        set
        {
            this.active = value;
            this.guiItem.Visible = value;
        }
    }
    #endregion

    #region Production
    Dictionary<string, float> _statProduct;
    Dictionary<string, float> _statСonsumption;
    float _productivity;
    float _lastProductivity;

    bool _isProduct;

    void initProduction()
    {
        _statProduct = new Dictionary<string, float>();
        _statСonsumption = new Dictionary<string, float>();
        _statProduct.Add("baseProduct", 0);
        _isProduct = true;
        _productivity = 1.0f;
        _lastProductivity = 1.0f;
    }

    public bool IsProduct
    {
        get { return _isProduct; }
    }

    public Dictionary<string, float> Сonsumption
    {
        get
        {
            if (_statСonsumption == null)
                _statСonsumption = new Dictionary<string, float>();

            return _statСonsumption;
        }
    }

    public void AddProduct(float amount, string source = "baseProduct")
    {
        if (_statProduct == null)
            _statProduct = new Dictionary<string, float>(); 

        if (!_statProduct.ContainsKey(source))
            _statProduct.Add(source, 0);

        _statProduct[source] += amount;
    }

    public void SetupProduct(float amount, string source = "baseProduct")
    {
        if (_statProduct == null)
            _statProduct = new Dictionary<string, float>();

        if (!_statProduct.ContainsKey(source))
            _statProduct.Add(source, 0);

        _statProduct[source] = amount;
    }

    public float Productivity
    {
        set
        {
            _lastProductivity = _productivity;
            _productivity = Mathf.Max(0, value);
        }
    }

    public void ReturnLastProductivity()
    {
        _productivity = _lastProductivity;
    }

    /// <summary>
    /// Production for the day. Calls every tick to simulate realtime. Production for percentage
    /// </summary>
    /// <param name="percent"></param>
    public void Production(float percent)
    {
        if (!_isProduct)
            return;

        if (!active)
            return;

        float baseProd = _statProduct["baseProduct"] * _productivity;

        foreach(var pd in _statProduct)
        {
            if (pd.Key.Equals("baseProduct"))
                continue;

            baseProd += pd.Value;
        }

        foreach (var cp in _statСonsumption)
        {
            baseProd -= cp.Value;
        }

        Count += baseProd*percent;
    }

    public float RatioProductionConsumption
    {
        get
        {
            float cons = 0;
            float baseProd = _statProduct["baseProduct"] * _productivity;

            foreach (var pd in _statProduct)
            {
                if (pd.Key.Equals("baseProduct"))
                    continue;

                baseProd += pd.Value;
            }

            foreach (var cp in _statСonsumption)
            {
                cons += cp.Value;
            }

            return cons / baseProd;
        }
    }
    #endregion

    #region TooltipText

    string productPattern;
    string consumptionPattern;

    public string GetText()
    {
        float baseProd = _statProduct["baseProduct"] * _productivity;

        foreach (var pd in _statProduct)
        {
            if (pd.Key.Equals("baseProduct"))
                continue;

            baseProd += pd.Value;
        }

        float baseCons = 0f;
        foreach (var cp in _statСonsumption)
        {
            baseCons += cp.Value;
        }
        string label = productPattern;
        label += " " + baseProd + "\n\n" + consumptionPattern + " " + baseCons;

        return label;
    }

    #endregion
}

public class StatTemplate
{
    public string ObjectID;
    public Represent.Type represent;
    public float _curVal;
    public float _maxVal;

    public bool mainStat = false;
    public bool intFlag = false;
    public bool negative = false;
    public bool _isProd;
    public string _icon;
    
    public static StatTemplate Create(string ID, Represent.Type type, string icon = null, bool mainStat = false, float curVal = 0, float maxVal = 0, bool neg = false,
         bool intF = true, bool isProd = false)
    {
        StatTemplate temp = new StatTemplate();

        temp.ObjectID = ID;
        temp.negative = neg;
        temp.intFlag = intF;
        temp._curVal = curVal;
        temp._maxVal = maxVal;
        temp._isProd = isProd;
        temp._icon = icon;
        temp.mainStat = mainStat;

        temp.represent = type;

        return temp;
    }

    public static StatTemplate Create(SimpleJSON.JSONNode data)
    {
        StatTemplate temp = new StatTemplate();

        if (data["ID"] != null)
            temp.ObjectID = data["ID"].Value;

        if (data["Negative"] != null)
            temp.negative = data["Negative"].AsBool;

        if (data["IsProduct"] != null)
            temp._isProd = data["IsProduct"].AsBool;

        if (data["IconID"] != null)
            temp._icon = data["IconID"].Value;

        if (data["MainStat"] != null)
            temp.mainStat = data["MainStat"].AsBool;

        temp.represent = Represent.Type.Simple;
        if (data["Representer"] != null)
            temp.represent = (Represent.Type)Enum.Parse(typeof(Represent.Type), data["Representer"].Value);

        temp.intFlag = false;

        temp._curVal = 0;
        if (data["CurValue"] != null)
            temp._curVal = data["CurValue"].AsFloat;

        temp._maxVal = 0;
        if (data["MaxValue"] != null)
            temp._curVal = data["MaxValue"].AsFloat;

        return temp;
    }
}

public class SM
{
    private static SM instance = null;
    private Dictionary<string, Stat> _stats;
    private Dictionary<string, bool> _flags;
    private Subscriber sub;
    public SM()
    {
        _stats = new Dictionary<string, Stat>();
        _flags = new Dictionary<string, bool>();
    }

    public static void NewGame()
    {
        if (SM.instance != null)
        {
            SM.instance = null;
        }

        SM.instance = new SM();
        SM.instance.sub = Subscriber.Create(SM.instance);
        SM.instance.sub.AddEvent(TriggerType.ProductTick.ToString(), "");

        Stat.Create(StatType.Wood.ToString(), Represent.Type.Simple, icon: "log", mainStat : true, curVal:0, isProd:true);
        Stat.Create(StatType.Stone.ToString(), Represent.Type.Simple, icon: "stone", mainStat: true, curVal: 0, isProd: true);
        Stat.Create("Metal", Represent.Type.Simple, icon: "metal", mainStat: true, curVal: 0, isProd: true);
        Stat.Create(StatType.Food.ToString(), Represent.Type.Simple, icon: "apple", mainStat: true, curVal: 0, isProd: true);
        Stat.Create(StatType.Skystone.ToString(), Represent.Type.Simple, icon: "gravore", mainStat: true, curVal: 0, isProd: true);
        Stat.Create(StatType.Unit.ToString(), Represent.Type.Simple, icon: "persIcon", mainStat: true, curVal: 12, isProd: true);
    }

    public static Dictionary<string, Stat> Stats
    {
        get
        {
            return SM.instance._stats;
        }
    }

    public static void AddStat(float amount, string id)
    {
        if (SM.instance._stats.ContainsKey(id))
            SM.instance._stats[id].Count += amount;
    }

    public static void SetupStat(float amount, string id)
    {
        if (SM.instance._stats.ContainsKey(id))
            SM.instance._stats[id].Count = amount;
    }

    public static void SetupMax(float amount, string id)
    {
        if (SM.instance._stats.ContainsKey(id))
            SM.instance._stats[id].Max = (int)amount;
    }

    public static void AddMax(float amount, string id)
    {
        if (SM.instance._stats.ContainsKey(id))
            SM.instance._stats[id].Max += (int)amount;
    }

    public static void RemoveStat(string ID)
    {
        if (SM.instance._stats.ContainsKey(ID))
            SM.instance._stats.Remove(ID);
    }

    public static void ActiveStat(string ID, bool active = true)
    {
        if (SM.instance._stats.ContainsKey(ID))
            SM.instance._stats[ID].Active = active;
    }

    public static void SetupProduct(string ID, float amount, string source = "baseProduct")
    {
        if (SM.instance._stats.ContainsKey(ID))
            SM.instance._stats[ID].SetupProduct(amount, source);
    }

    public static void AddProduct(string ID, float amount, string source = "baseProduct")
    {
        if(!SM.instance._stats.ContainsKey(ID))
        {
            if(ID.Equals("Wood"))
                Stat.Create(StatType.Wood.ToString(), Represent.Type.Slash, "log", true, 0, 0, isProd: true);
            else if(ID.Equals(StatType.Food.ToString()))
                Stat.Create(StatType.Food.ToString(), Represent.Type.Slash, "apple", true, 0, 0, isProd: true);
            else if (ID.Equals(StatType.Stone.ToString()))
                Stat.Create(StatType.Stone.ToString(), Represent.Type.Slash, "stone", true, 0, 0, isProd: true);
            else if (ID.Equals(StatType.Skystone.ToString()))
                Stat.Create(StatType.Skystone.ToString(), Represent.Type.Slash, "gravore", true, 0, 0, isProd: true);
            else if (ID.Equals(StatType.Unit.ToString()))
                Stat.Create(StatType.Unit.ToString(), Represent.Type.Slash, "persIcon", true, 0, 0, isProd: true);

            if(!SM.instance._stats.ContainsKey(ID))
            {
                Debug.LogError("Where no Stat: " + ID);
                return;
            }
        }

        SM.instance._stats[ID].AddProduct(amount, source);
    }

    public static void SetupСonsumption(string ID, float amount, string source = "baseConsumption")
    {
        if (SM.instance._stats.ContainsKey(ID))
        {
            if (SM.instance._stats[ID].Сonsumption.ContainsKey(source))
                SM.instance._stats[ID].Сonsumption[source] = amount;
            else
            {
                SM.instance._stats[ID].Сonsumption.Add(source, amount);
            }
        }
    }

    public static void AddСonsumption(string ID, float amount, string source = "baseConsumption")
    {
        if (SM.instance._stats.ContainsKey(ID))
        {
            if (SM.instance._stats[ID].Сonsumption.ContainsKey(source))
                SM.instance._stats[ID].Сonsumption[source] += amount;
            else
            {
                SM.instance._stats[ID].Сonsumption.Add(source, amount);
            }
        }
    }

    public void ProductTick()
    {
        foreach(var prod in Stats)
        {
            if (prod.Value.IsProduct)
                prod.Value.Production(TM.PercentTick);
        }
    }

    #region Flags

    public static void AddFlag(string fl)
    {
        if (SM.instance._flags == null)
            SM.instance._flags = new Dictionary<string, bool>();

        if (SM.instance._flags.ContainsKey(fl))
            Debug.LogError("We have flag ID: " + fl);
        else
            SM.instance._flags.Add(fl, false);
    }

    public static bool CheckFlag(string fl)
    {
        if (!SM.instance._flags.ContainsKey(fl))
        {
            Debug.LogError("No such flag ID: " + fl);
            return false;
        }

        return SM.instance._flags[fl];
    }

    public static void SetFlag(string fl, bool isT = true)
    {
        if (!SM.instance._flags.ContainsKey(fl))
        {
            Debug.LogError("No such flag ID: " + fl);
            SM.instance._flags.Add(fl, isT);
            return;
        }

        SM.instance._flags[fl] = isT;
        ES.NotifySubscribers(TriggerType.FlagChanged.ToString(), fl);
    }

    public static void RemoveFlag(string fl)
    {
        SM.instance._flags.Remove(fl);
    }

    #endregion

    #region

    // Template solution
    public static string GetStatIcon(string statID)
    {
        if (SM.Stats.ContainsKey(statID))
            return SM.Stats[statID].Icon;

        return "BaseIcon";
    }

    #endregion
}

public class Represent
{
    public enum Type
    {
        Non,
        Simple,
        Slash,
        Percent
    }

    Type _format;
    public Type Format
    {
        get
        { return _format; }
        set
        {
            _format = value;
        }
    }

    public string Formatter(float main = 0.0f, float max = 0.0f)
    {
        int man = Mathf.FloorToInt(main);
        int maks = Mathf.FloorToInt(max);

        if (Format == Type.Simple)
            return man.ToString();
        else if (Format == Type.Slash)
            return string.Format("{0}/{1}", man, maks);
        else if (Format == Type.Percent)
            return string.Format("{0}%", (Mathf.Round(man / maks)) * 100);

        return "";
    }

    public string Formatter(int main = 0, int max = 0)
    {
        if (Format == Type.Simple)
            return main.ToString();
        else if (Format == Type.Slash)
            return string.Format("{0}/{1}", main, max);
        else if (Format == Type.Percent)
            return string.Format("{0}%", (Mathf.Round(main / max)) * 100);

        return "";
    }
}
