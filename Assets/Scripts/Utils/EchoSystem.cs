using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

public class EchoSystem : MonoBehaviour {

    private Dictionary<string, Echos> eventDictionary;

    public void Init()
    {
        eventDictionary = new Dictionary<string, Echos>();
    }

    public void AddListiner(string eventName, UnityAction<string, string> listener)
    {
        Echos thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new Echos();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void RemoveListiner(string eventName, UnityAction<string, string> listener)
    {
        Echos thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void NotifySubscribers(string eventName, string objID)
    {
        Echos thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(eventName, objID);
        }
    }
}

public class ES
{
    private static EchoSystem instance = null;
    public static void NewGame()
    {
        ES.instance = GameObject.Find("Managers").GetComponentInChildren<EchoSystem>();
        ES.instance.Init();
    }

    public static void AddListiner(string eventName, UnityAction<string, string> listener)
    {
        ES.instance.AddListiner(eventName, listener);
    }

    public static void RemoveListiner(string eventName, UnityAction<string, string> listener)
    {
        ES.instance.RemoveListiner(eventName, listener);
    }

    public static void NotifySubscribers(string eventName, string objID)
    {
        ES.instance.NotifySubscribers(eventName, objID);
    }
}

public class Subscriber
{
    Dictionary<string, List<string>> dictionary;
    System.Object sub;

    public static Subscriber Create(System.Object obj)
    {
        Subscriber temp = new Subscriber
        {
            sub = obj,
            dictionary = new Dictionary<string, List<string>>()
        };

        return temp;
    }

    public void AddEvent(string ev, string objID = "")
    {
        if (dictionary.ContainsKey(ev) && objID.Equals(""))
            return;
        else if(dictionary.ContainsKey(ev))
        {
            dictionary[ev].Add(objID);
            return;
        }
        
        dictionary.Add(ev, new List<string>());

        if (!objID.Equals(""))
            dictionary[ev].Add(objID);

        ES.AddListiner(ev, Notify);
    }

    public void AddEvent(string ev, List<string> objID)
    {
        if (dictionary.ContainsKey(ev))
            return;

        dictionary.Add(ev, new List<string>());
        dictionary[ev].AddRange(objID);

        ES.AddListiner(ev, Notify);
    }

    public void AddListeningObject(string ev, string objID)
    {
        if (!dictionary.ContainsKey(ev))
        {
            this.AddEvent(ev, objID);
            return;
        }

        dictionary[ev].Add(objID);
    }

    public void AddListeningObject(string ev, List<string> objID)
    {
        if(!dictionary.ContainsKey(ev))
        {
            this.AddEvent(ev, objID);
            return;
        }

        dictionary[ev].AddRange(objID);
    }

    public void RemoveEvent(string ev)
    {
        if (dictionary.ContainsKey(ev))
            dictionary.Remove(ev);

        ES.RemoveListiner(ev, Notify);
    }
    
    public void RemoveListeningObject(string ev, string objID)
    {
        if (!dictionary.ContainsKey(ev))
            return;

        dictionary[ev].Remove(objID);
    }

    public void ClearListeningObject(string ev)
    {
        if (!dictionary.ContainsKey(ev))
            return;

        dictionary[ev].Clear();
    }

    void Notify(string ev, string objID)
    {
        if (sub == null)
        {
            Debug.LogError("No object set to subscriber");
            return;
        }

        if(!objID.Equals("") && !dictionary[ev].Contains(objID))
            return;
        else if(objID.Equals("") && dictionary[ev].Count > 0)
            return;
        
        MethodInfo mi = sub.GetType().GetMethod(ev);
        if (mi != null)
            mi.Invoke(sub, null);
        else
            Debug.LogError("Not implemented method for subscriber: " + ev);
    }
}

public class Echos : UnityEvent<string, string>
{
}