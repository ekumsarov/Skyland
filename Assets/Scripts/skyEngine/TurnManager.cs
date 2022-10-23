using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Lodkod;

public class TurnManager : MonoBehaviour, ObjectID  {

    #region Time Work

    string ObjectID = "nil";

    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    public bool Active = false;

    GlobalDay Day;

    /*
    float DailyTimer;
    int Days;
    

    int DaysPart;
    float DaysPartTimer;
    float DaysPartOffsetTimer;
    float CurrentDayTimer;
    int currentDaysPart;


    int ProductParts;
    float ProductTimer;
    float CurrentProductTimer;
    int CurrentTics;
    float PercentOfTicks;*/

    float DailyTimer;
    float _productTimer;
    float PercentOfTicks;

    float LevelPlayTime;

    List<DayInfo> _expierdList;
    List<DayInfo> _threatList;

    public void Init(SimpleJSON.JSONNode node)
    {
        ID = "TurnManager";
        Active = false;

        /*
        Days = 0;
        DailyTimer = node["RoundTime"].AsFloat;

        DaysPart = node["DaysPart"].AsInt;
        DaysPartOffsetTimer = DailyTimer / (2 * DaysPart);
        DaysPartTimer = DailyTimer / DaysPart;
        CurrentDayTimer = 0;
        currentDaysPart = 0;

        ProductParts = node["DayTics"].AsInt;
        ProductTimer = DailyTimer / ProductParts;
        CurrentProductTimer = 0;
        CurrentTics = 0;
        PercentOfTicks = 1 / ProductParts;*/

        DailyTimer = node["RoundTime"].AsFloat;
        _productTimer = DailyTimer / node["DayTics"].AsInt;
        PercentOfTicks = Mathf.Round((1.0f / node["DayTics"].AsInt) * 1000.0f) / 1000.0f;

        _threatList = new List<DayInfo>();
        _expierdList = new List<DayInfo>();

        LevelPlayTime = 0f;

        Day = new GlobalDay(node["DayTics"].AsInt, _productTimer);
        Day.SetAllTicks(0);
        ES.NotifySubscribers(TriggerType.ProductTick.ToString(), "");
    }

    void Update()
    {
        if (!Active)
            return;

        if (GM.GameState == GameState.EventWorking || GM.GameState == GameState.War)
            return;

        float delta = Time.deltaTime;

        LevelPlayTime += delta;

        // Product count
        Day.ImproveTick(delta);
    }

    #endregion
    
    /// <summary>
    /// Percent of one tick
    /// </summary>
    public float PercentTick
    {
        get
        {
            return PercentOfTicks;
        }
    }

    public GlobalDay Info
    {
        get
        {
            return Day;
        }
    }

    public int Ticks
    {
        get
        {
            return Day.DayTick;
        }
    }

    public float ProductTimer
    {
        get
        {   return _productTimer; }
    }

    public bool CurrentDayNight
    {
        get
        {
            if (Day.DayTick >= Day.ProductParts / 2)
                return true;

            return false;
        }
    }

    public DayInfo GetExpieredDay(int ticks)
    {
        return Day.GetExpieredDay(ticks);
    }

    public DayInfo GetExpieredDay(DayInfo ticks)
    {
        return Day.GetExpieredDay(ticks);
    }

    public List<DayInfo> Threats
    {
        get { return _threatList; }
    }

    public void AddThreat(DayInfo day, bool visual = false)
    {
        if(!visual)
        {
            _expierdList.Add(day);
            return;
        }

        _threatList.Add(day);
        ES.NotifySubscribers(TriggerType.NewThreat.ToString(), "");
    }

    public void RemoveThreat(DayInfo day)
    {
        if(_expierdList.Contains(day))
        {
            _expierdList.Remove(day);
            return;
        }

        if(_threatList.Contains(day))
            _threatList.Remove(day);

        ES.NotifySubscribers(TriggerType.NewThreat.ToString(), "");
    }
}

public class TM
{
    private static TurnManager instance = null;
    public static void NewGame(SimpleJSON.JSONNode node)
    {
        TM.instance = GameObject.Find("Managers").GetComponentInChildren<TurnManager>();
        TM.instance.Init(node);
//        TM.instance._thre
    }

    public static void Start()
    {
        TM.instance.Active = true;
    }

    public static DayPart DayPart
    {
        get
        {
            return TM.instance.Info.DayPart;
        }
    }
    
    public static int Day
    {
        get
        {
            return TM.instance.Info.Day;
        }
    }

    public static int Ticks
    {
        get
        {
            return TM.instance.Ticks;
        }
    }

    public static float ProductTimer
    {
        get { return TM.instance.ProductTimer; }
    }

    public static bool Expired(DayInfo day)
    {
        if (day.Day != TM.instance.Info.Day)
            return false;

        if (day.DayTick > TM.instance.Info.DayTick)
            return false;

        return true;
    }

    public static DayInfo GetExpieredDay(int ticks)
    {
        return TM.instance.GetExpieredDay(ticks);
    }

    public static DayInfo GetExpieredDay(DayInfo day)
    {
        return TM.instance.GetExpieredDay(day);
    }

    public static void AddThreat(DayInfo day, bool visual = false)
    {
        TM.instance.AddThreat(day, visual);
    }

    public static void RemoveThreat(DayInfo day)
    {
        TM.instance.RemoveThreat(day);
    }

    public static List<DayInfo> Threats
    {
        get { return TM.instance.Threats; }
    }

    public static float PercentTick
    {
        get { return TM.instance.PercentTick; }
    }

    public static float ConvertTicks(int ticks)
    {
        return TM.instance.Info.ConvertTicks(ticks);
    }
}
