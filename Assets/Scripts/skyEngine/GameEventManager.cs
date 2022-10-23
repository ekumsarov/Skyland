using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using GameEvents;

public enum GameEventState { Sleep, Closed, Play };

public class GameEventManager : SkyObject {
    
    List<TimerActivity> TimeEvents;
    int counbt = 1;


    public void Init()
    {
        InitBased = true;
        this.ID = "GameEventManager";

        this.Activity = new Activity();
        this.Activity.ID = "Iteration " + counbt;
        counbt += 1;
        this.Activity.Object = this;
        this.Activity.initActivity();

        this.MainEvent = "Greeting";

        this.Action = new ActionAddition();
        this.Action.parent = this;
        

        Subscibe();

        this.eventsInQueue = new List<GameEvent>();
        this.forces = new List<GameEvent>();
        this.state = GameEventState.Sleep;

        TimeEvents = new List<TimerActivity>();
        Subscribe = TriggerType.ChangedDaysPart;
        Subscribe = TriggerType.ProductTick;
        Subscribe = TriggerType.GameStateChanged;
    }

    public void ChangedDaysPart()
    {
        
    }

    public void ProductTick()
    {

    }


    GameEventState state;
    public GameEventState State
    {
        get { return this.state; }
        set { this.state = value; }
    }

    public void ExecuteForce()
    {
        if (this.forces.Count == 0)
        {
            this.Execute();
            return;
        }

        this.state = GameEventState.Play;
        GM.GameState = Lodkod.GameState.EventWorking;

        if (forces.Count > 0)
        {
            foreach (GameEvent ev in forces)
            {
                if (ev.CanActive())
                {
                    Debug.LogError("Event force played: " + ev.ID);
                    ev.Start();
                    return;
                }
                else
                {
                    this.SafeRemoveForceEvent(ev);
                    return;
                }
            }
        }
    }


    public void Execute()
    {
        if (eventsInQueue.Count == 0 && this.forces.Count == 0)
        {
            GM.GameState = Lodkod.GameState.Game;
            this.state = GameEventState.Sleep;
            return;
        }

        this.state = GameEventState.Play;
        GM.GameState = Lodkod.GameState.EventWorking;



        foreach (GameEvent ev in eventsInQueue)
        {
            if (ev.CanActive())
            {
                Debug.Log("Event played: " + ev.ID);
                ev.Start();
                return;
            }
            else
            {
                Debug.LogError("Event can't active: " + ev.ID);
                this.SafeRemoveEvent(ev);
                return;
            }
        }
    }

    public override void Actioned(string ev = "")
    {
        if (ev.Equals(""))
            Debug.LogError("Cannot be empty event id on GameEventManager");

        if (this.Activity.ContainPack(ev))
        {
            this.Activity.callActivityPack(ev);
            return;
        }
            

        if (this.Activity.CanCallEvent(ev))
        {
            this.Activity.CallEvent(ev);
            return;
        }

        this.Action.CallAction(ev);
        
//        Debug.LogError("No such event in GEM: " + ev);
    }

    private void SafeRemoveEvent(GameEvent ev)
    {
        if (eventsInQueue.Contains(ev))
            eventsInQueue.Remove(ev);

        this.Execute();
    }

    private void SafeRemoveForceEvent(GameEvent ev)
    {
        if (forces.Contains(ev))
            forces.Remove(ev);

        this.ExecuteForce();
    }

    #region EventStack

    List<GameEvent> eventsInQueue;
    List<GameEvent> forces;

    public void ContinueEvent(GameEvent ev)
    {
        if (eventsInQueue.Contains(ev))
        {
            eventsInQueue.Remove(ev);
            if(forces.Count > 0)
            {
                this.ExecuteForce();
                return;
            }
        }
        else if (forces.Contains(ev))
        {
            forces.Remove(ev);
            this.ExecuteForce();
            return;
        }
            

        Execute();
    }

    public void addEventsInQueue(List<GameEvent> events, bool force = false)
    {
        if(force)
        {
            foreach(GameEvent ev in events)
            {
                forces.Add(ev);
            }
            if(this.state == GameEventState.Sleep)
                this.ExecuteForce();
            return;
        }
        
        foreach (GameEvent ev in events)
        {
            eventsInQueue.Add(ev);
        }

        if (this.state == GameEventState.Play || this.state == GameEventState.Closed)
            return;

        this.Execute();
    }

    public void addEventInQueue(GameEvent events, bool force = false)
    {

        if (force)
        {
            forces.Add(events);
            if (this.state == GameEventState.Sleep)
                this.ExecuteForce();
            return;
        }
        else
            eventsInQueue.Add(events);

        if (this.state == GameEventState.Play || this.state == GameEventState.Closed)
            return;

        this.Execute();
    }

    public void insertEventsInQueue(List<GameEvent> events, GameEvent after = null)
    {
        int insertIterator = 0;
        bool isAfter = false;
        if (after != null)
        {
            isAfter = true;
            insertIterator = eventsInQueue.IndexOf(after) + 1;
        }

        foreach (GameEvent ev in events)
        {
            if (isAfter)
            {
                eventsInQueue.Insert(insertIterator, ev);
                insertIterator++;
            }
            else
            {
                eventsInQueue.Add(ev);
            }
        }
    }

    public void PlayForceEvents(List<GameEvent> events)
    {
        foreach (GameEvent ev in events)
        {
            forces.Add(ev);
        }
        if (this.state == GameEventState.Sleep)
            this.ExecuteForce();
    }

    public void PushEventInQueue(GameEvent insertEvent, GameEvent after, bool play = false)
    {
        eventsInQueue.Insert(eventsInQueue.IndexOf(after), insertEvent);
        if (play)
        {
            eventsInQueue.Remove(after);
            this.Execute();
        }

    }

    public void AddEvent(string ID)
    {
        this.Activity.AddEvent(ID);
    }

    public void AddPack(EventPackSystem.EventPack ID)
    {
        this.Activity.pushPack(ID);
    }

    public void Play(string ev)
    {
        if (this.Activity.CanCallPack(ev))
            eventsInQueue.AddRange(this.Activity.getActivity(ev));
        else if (this.Activity.CanCallEvent(ev))
            eventsInQueue.Add(this.Activity.GetEvent(ev));
        else
        {
            EventPackSystem.EventPack pack = GM.Pack(ev);
            if (pack!=null)
            {
                this.Activity.pushPack(pack);
                eventsInQueue.AddRange(this.Activity.getActivity(ev));

                if (this.state == GameEventState.Play || this.state == GameEventState.Closed)
                    return;

                this.Execute();
                return;
            }

            eventsInQueue.Add(this.Activity.GetEvent(ev));

            if (this.state == GameEventState.Play || this.state == GameEventState.Closed)
                return;

            this.Execute();
        }
    }

    public void PlayEvent(string ev)
    {
        eventsInQueue.Add(this.Activity.GetEvent(ev));

        if (this.state == GameEventState.Play || this.state == GameEventState.Closed)
            return;

        this.Execute();
    }

    public void PlayPack(string ev)
    {
        eventsInQueue.AddRange(this.Activity.getActivity(ev));

        if (this.state == GameEventState.Play)
            return;

        this.Execute();
    }

    public void GameStateChanged()
    {
        if(GM.GameState == GameState.Game)
        {
            this.state = GameEventState.Sleep;
            if (this.eventsInQueue.Count > 0)
                this.Execute();
            else if (this.forces.Count > 0)
                this.ExecuteForce();

            return;
        }

        if (GM.GameState == GameState.EventWorking)
        {
            this.state = GameEventState.Play;
            return;
        }

        this.state = GameEventState.Closed;
    }
    #endregion
}

public class GEM
{
    public static GameEventManager instance = null;
    public static void NewGame()
    {
        GEM.instance = GameObject.Find("Managers").GetComponentInChildren<GameEventManager>();
        GEM.instance.HardSet();
        GEM.instance.Init();
    }

    public static GameEventState State
    {
        get { return GEM.instance.State; }
        set
        {
            GEM.instance.State = value;
        }
        
    }

    public static void AddEventsInQueue(List<GameEvent> events, bool force = false)
    {
        GEM.instance.addEventsInQueue(events, force);
    }

    public static void AddEventInQueue(GameEvent events, bool force = false)
    {
        GEM.instance.addEventInQueue(events, force);
    }

    public static void InsertEventsInQueue(List<GameEvent> events, GameEvent after = null)
    {
        GEM.instance.insertEventsInQueue(events, after);
    }

    public static void ContinueEvent(GameEvent ev)
    {
        GEM.instance.ContinueEvent(ev);
    }

    public static void ContinueEvent(GameEvent insertEvent, GameEvent after, bool play = false)
    {
        GEM.instance.PushEventInQueue(insertEvent, after, play);
    }

    public static void AddEvent(string ev)
    {
        GEM.instance.AddEvent(ev);
    }

    public static void Execute(string ev, DayInfo day = null, int ticks = -1)
    {
        GEM.instance.AddEvent(ev);

        if (day != null)
        {
            ExpiredDay.ExpiredAfterDay(day, ev: ev);
 //           TimerActivity.Create(ev, day);
            return;
        }

        if(ticks != -1)
        {
            ExpiredDay.ExpiredAfterTicks(ticks, ev: ev);
            //           TimerActivity.Create(ev, day);
            return;
        }

        GEM.instance.Play(ev);
    }

    public static void Execute(EventPackSystem.EventPack ev, DayInfo day = null, int ticks = -1)
    {
        if (ev == null)
        {
            Debug.LogError("No such pack ID: " + ev);
            return;
        }

        GEM.instance.AddPack(ev);

        if (day != null)
        {
            ExpiredDay.ExpiredAfterDay(day, pack: ev);
            return;
        }

        if(ticks != -1)
        {
            ExpiredDay.ExpiredAfterTicks(ticks, pack: ev);
            return;
        }

        GEM.instance.PlayPack(ev.ID);
    }
}