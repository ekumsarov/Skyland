using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;
using SimpleJSON;
using System.Linq;
using Lodkod;
using GameEvents;

public class Activity : ObjectID
{
    string ObjectID;

    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    protected Dictionary<string, List<GameEvent>> packs;
    protected Dictionary<string, GameEvent> events;

    SkyObject _object;
    public SkyObject Object
    {
        get { return this._object; }
        set { this._object = value; }
    }

    
    public virtual void initActivity()
    {
        this.packs = new Dictionary<string, List<GameEvent>>();
        this.events = new Dictionary<string, GameEvent>();
    }

    #region EventWork
    public void CallEvent(JSONNode node)
    {
        string evID = node["Event"].Value;
        if (!this.events.ContainsKey(evID))
        {
            GameEvent temp = GameEvent.loadEvent(node);
            if (Object != null)
                temp.Object = Object;


            events.Add(evID, temp);
        }
        else
            this.events[node["Event"]].PrepareEvent(node);

        GEM.AddEventInQueue(this.events[node["Event"]]);
    }

    public void CallEvent(string ID)
    {
        string parse = " { 'Event':'" + ID + "' } ";
        JSONNode node = JSON.Parse(parse.Replace("'", "\""));

        if (!this.events.ContainsKey(ID))
        {
            GameEvent temp = GameEvent.loadEvent(node);
            if (Object != null)
                temp.Object = Object;
            
            events.Add(ID, temp);
        }
        else
            this.events[ID].PrepareEvent(node);

        GEM.AddEventInQueue(this.events[node["Event"]]);
    }

    public void AddEvent(JSONNode node)
    {
        string evID = node["Event"].Value;
        if (!this.events.ContainsKey(evID))
        {
            GameEvent temp = GameEvent.loadEvent(node);
            if (Object != null)
                temp.Object = Object;


            events.Add(evID, temp);
        }
    }

    public void AddEvent(string ID)
    {
        string evID = ID;
        if (!this.events.ContainsKey(evID))
        {
            string parse = " { 'Event':'" + ID + "' } ";
            GameEvent temp = GameEvent.loadEvent(JSON.Parse(parse.Replace("'", "\"")));
            if (Object != null)
                temp.Object = Object;


            events.Add(evID, temp);
        }
    }

    public bool CanCallEvent(string id)
    {
        return this.events.ContainsKey(id);
    }

    public GameEvent GetEvent(JSONNode node)
    {
        string evID = node["Event"].Value;
        if (!this.events.ContainsKey(evID))
        {
            GameEvent temp = GameEvent.loadEvent(node);
            if (Object != null)
                temp.Object = Object;


            events.Add(evID, temp);
        }
        else
            this.events[node["Event"]].PrepareEvent(node);

        return this.events[node["Event"]];
    }

    public GameEvent GetEvent(string ev)
    {
        if (!this.events.ContainsKey(ev))
        {
            string parse = " { 'Event':'" + ID + "' } ";
            GameEvent temp = GameEvent.loadEvent(JSON.Parse(parse.Replace("'", "\"")));
            if (Object != null)
                temp.Object = Object;


            events.Add(ev, temp);
        }

        this.events[ev].PrepareEvent();

        return this.events[ev];
    }
    #endregion

    #region PackWork
    public void pushPack(EventPack pack)
    {
        if (packs.ContainsKey(pack.ID))
            return;

        if (!packs.ContainsKey(pack.ID))
        packs.Add(pack.ID, new List<GameEvent>());

        foreach (JSONNode nod in pack.events)
        {
            if(nod["Event"] == null)
            {
                GameEvent temp = GameEvent.loadEvent(nod);
                if (Object != null)
                    temp.Object = Object;

                packs[pack.ID].Add(temp);
            }
            else
            {
                string evID = nod["Event"].Value;
                if(!this.events.ContainsKey(evID))
                {
                    GameEvent temp = GameEvent.loadEvent(nod);
                    if (Object != null)
                        temp.Object = Object;


                    events.Add(evID, temp);
                }
                else
                    events[evID].PrepareEvent(nod);

                packs[pack.ID].Add(events[evID]);
            }
            
        }
    }

    public void PushPack(string ID, List<GameEvent> nEvents)
    {
        if (packs.ContainsKey(ID))
            return;

        if (!packs.ContainsKey(ID))
            packs.Add(ID, nEvents);

        foreach(var ev in nEvents)
        {
            ev.Object = Object;
        }
    }

    public void pushEventsForEmptyPack(List<GameEvent> nEvents)
    {
        if (!packs.ContainsKey("void"))
            packs.Add("void", new List<GameEvent>());

        foreach (GameEvent ev in nEvents)
        {
            if (Object != null)
                ev.Object = Object;

            packs["void"].Add(ev);
        }
    }

    public void restoreAllPackEvents(List<GameEvent> nEvents)
    {
        packs.Clear();

        packs.Add("void", new List<GameEvent>());
        foreach (GameEvent ev in nEvents)
        {
            if (Object != null)
                ev.Object = Object;

            packs["void"].Add(ev);
        }
    }

    public void clearAllActivityPack()
    {
        packs.Clear();
    }

    public void removePackEvent(string eve)
    {
        if (!packs.ContainsKey(eve))
            return;

        packs.Remove(eve);
    }

    public List<GameEvent> getActivity(string nEvent)
    {
        if (!packs.ContainsKey(nEvent))
        {
            Debug.LogError("Where are no event: " + nEvent);
            return null;
        }
        return packs[nEvent];
    }

    public bool CanCallPack(string nEvent)
    {
        bool cancall = packs.ContainsKey(nEvent);
        if (!cancall)
        {
            EventPack pack = GM.DelayedPack(Object.ID, nEvent);
            if(pack != null)
            {
                if (!packs.ContainsKey(pack.ID))
                    packs.Add(pack.ID, new List<GameEvent>());

                foreach (JSONNode nod in pack.events)
                {
                    if (nod["Event"] == null)
                    {
                        GameEvent temp = GameEvent.loadEvent(nod);
                        if (Object != null)
                            temp.Object = Object;

                        packs[pack.ID].Add(temp);
                    }
                    else
                    {
                        string evID = nod["Event"].Value;
                        if (!this.events.ContainsKey(evID))
                        {
                            GameEvent temp = GameEvent.loadEvent(nod);
                            if (Object != null)
                                temp.Object = Object;


                            events.Add(evID, temp);
                        }
                        else
                            events[evID].PrepareEvent(nod);

                        packs[pack.ID].Add(events[evID]);
                    }

                }
                cancall = true;
            }
        }

        return cancall;
    }

    public void callActivityPack(string nEvent, bool force = false)
    {
        if (!packs.ContainsKey(nEvent))
        {
            EventPack pack = GM.DelayedPack(Object.ID, nEvent);
            if (pack != null)
            {
                if (!packs.ContainsKey(pack.ID))
                    packs.Add(pack.ID, new List<GameEvent>());

                foreach (JSONNode nod in pack.events)
                {
                    if (nod["Event"] == null)
                    {
                        GameEvent temp = GameEvent.loadEvent(nod);
                        if (Object != null)
                            temp.Object = Object;

                        packs[pack.ID].Add(temp);
                    }
                    else
                    {
                        string evID = nod["Event"].Value;
                        if (!this.events.ContainsKey(evID))
                        {
                            GameEvent temp = GameEvent.loadEvent(nod);
                            if (Object != null)
                                temp.Object = Object;


                            events.Add(evID, temp);
                        }
                        else
                            events[evID].PrepareEvent(nod);

                        packs[pack.ID].Add(events[evID]);
                    }

                }
            }

            if(!packs.ContainsKey(nEvent))
            {
                Debug.LogError("Where are no event: " + nEvent);
                return;
            }
        }
        GEM.AddEventsInQueue(packs[nEvent], force: force);
    }

    public void callAllActivityPack()
    {
        if (this.packs.Count == 0)
        {
            Debug.LogError("Where are no events in this Activity");
            return;
        }
        GEM.AddEventsInQueue(getListOfEvents());
    }

    public void callActivityPack()
    {
        if(this.packs.Count == 0)
        {
            Debug.LogError("Where are no events in this Activity");
            return;
        }

        GEM.AddEventsInQueue(packs[packs.Keys.ElementAt(0)]);
    }
	
    public bool ContainPack(string ID)
    {
        return packs.ContainsKey(ID);
    }

    public List<GameEvent> getListOfEvents()
    {
        List<GameEvent> nevents = new List<GameEvent>();

        foreach(string key in this.packs.Keys)
        {
            foreach(var ev in packs[key])
            {
                nevents.Add(ev);
            }
            
        }
        return nevents;
    }
    #endregion
}
