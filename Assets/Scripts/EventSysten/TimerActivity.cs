using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;
using EventPackSystem;
 

public class TimerActivity {

    DayInfo timeToActive;
    public string Event;

    public static TimerActivity Create(string eve, DayInfo time)
    {
        return new TimerActivity().createTimerActivity(eve, time);
    }

    public TimerActivity createTimerActivity(string pack, DayInfo time)
    {
        this.Event = pack;
        this.timeToActive = new DayInfo { Day = time.Day + TM.Day, DayPart = time.DayPart };

        return this;
    }

    public bool Activate
    {
        get
        {
            return TM.Expired(timeToActive);
        }
    }
}
