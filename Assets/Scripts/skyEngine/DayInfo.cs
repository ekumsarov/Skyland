using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lodkod;

public class DayInfo {

    #region Base
    int _day;
    DayPart _part;
    int _ticks;
    float _dayTimer;

    public Subscriber _sub;

    int _dayPartCount = Enum.GetValues(typeof(DayPart)).Length;
	
    public int Day
    {
        get { return _day; }
        set { _day = value; }
    }

    public DayPart DayPart
    {
        get { return _part; }
        set {  _part = value; }
    }

    public int DayTick
    {
        get { return _ticks; }
        set { _ticks = value; }
    }

    public int DayPartCount
    {
        get { return _dayPartCount; }
        set { _dayPartCount = value; }
    }

    public float DayTimer
    {
        get { return _dayTimer; }
        set { _dayTimer = value; }
    }

    public static DayInfo Create(int day = 0, DayPart part = DayPart.Morning)
    {
        DayInfo temp = new DayInfo
        {
            Day = day,
            DayPart = part,
            DayTick = 0
        };

        return temp;
    }

    public static DayInfo Create(int ticks)
    {
        return TM.GetExpieredDay(ticks);
    }

    public static DayInfo Create(DayInfo day)
    {
        DayInfo temp = new DayInfo
        {
            Day = day.Day,
            DayPart = day.DayPart,
            DayTick = 0
        };

        return temp;
    }
    #endregion

    #region Logic
    

    

    #endregion
}
