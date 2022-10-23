using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using System;


public class GlobalDay : DayInfo
{
    int _allTicks;
    public int ProductParts;

    int _partInTicks;
    int _currentPartInTicks;

    float ProductTimer;

    public GlobalDay(int productParts, float productTimer)
    {
        Day = 0;
        DayPart = 0;
        DayTick = 0;
        DayTimer = 0;
        ProductTimer = productTimer;
        _allTicks = 0;
        ProductParts = productParts;
        _partInTicks = ProductParts / Enum.GetValues(typeof(DayPart)).Length;
        _currentPartInTicks = _partInTicks;
    }

    public void ImproveTick(float delta)
    {
        DayTimer += delta;
        if (DayTimer >= ProductTimer)
        {
            DayTimer -= ProductTimer;

            _allTicks += 1;
            DayTick += 1;

            ES.NotifySubscribers(TriggerType.ProductTick.ToString(), "");

            _currentPartInTicks -= 1;
            if (_currentPartInTicks <= 0)
            {
                _currentPartInTicks = _partInTicks;
                ImproveDaysPart();
                ES.NotifySubscribers(TriggerType.ChangedDaysPart.ToString(), "");
            }

            if (DayTick >= ProductParts)
            {
                DayTick = 0;
                Day += 1;
                ES.NotifySubscribers(TriggerType.NewDay.ToString(), "");
            }
        }
    }

    public void ImproveDaysPart()
    {
        int cur = (int)DayPart;

        cur += 1;

        if (cur >= DayPartCount)
            cur = 0;

        DayPart = (DayPart)cur;
    }

    public void SetAllTicks(int ticks)
    {
        Day = (int)(ticks / ProductParts);
        DayTick = ticks & ProductParts;
        DayPart = (DayPart)((int)(DayTick / _partInTicks));
        _currentPartInTicks = DayTick & _partInTicks;

        if (_currentPartInTicks == 0)
            _currentPartInTicks = _partInTicks;
    }

    public int AllTicks
    {
        get
        {
            return _allTicks;
        }
    }

    public DayInfo GetExpieredDay(int ticks, bool includeTimer = false)
    {
        DayInfo day = DayInfo.Create();

        int _tic = ticks + _allTicks;

        day.Day = (int)(_tic / ProductParts) + Day;
        day.DayTick = _tic & ProductParts;
        day.DayPart = (DayPart)((int)(day.DayTick / _partInTicks));
        day.DayTimer = -1;

        if (includeTimer)
            day.DayTimer = DayTimer;

        return day;
    }

    public DayInfo GetExpieredDay(DayInfo day)
    {
        day.Day = TM.Day + day.Day;
        day.DayTick += TM.Ticks;
        if(day.DayTick > ProductParts)
        {
            day.Day += 1;
            day.DayTick -= ProductParts;
        }
        day.DayPart = (DayPart)((int)(day.DayTick / _partInTicks));
        day.DayTimer = -1;

        return day;
    }

    public float ConvertTicks(int ticks)
    {
        return ProductTimer * ticks;
    }
}
