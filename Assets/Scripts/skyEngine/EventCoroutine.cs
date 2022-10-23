using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventCoroutine : MonoBehaviour
{
    void Start()
    {
        EventCoroutine.instance = this;
    }

    private static EventCoroutine instance = null;
    public delegate void Callback();
    Callback _callback;

    Callback Delegate
    {
        get { return EventCoroutine.instance._callback; }
        set
        {
            bool check = false;

            if (EventCoroutine.instance._callback == null)
                check = true;

            EventCoroutine.instance._callback = value;

            if (check && EventCoroutine.instance._callback != null)
                EventCoroutine.instance.StartCoroutine("GameEventCoroutine");

            if (EventCoroutine.instance._callback == null)
                EventCoroutine.instance.StopAllCoroutines();


        }
    }

    public static void AddCoroutine(Callback del)
    {
        bool startCor = EventCoroutine.instance._callback == null;
        EventCoroutine.instance._callback += del;

        if(startCor)
            EventCoroutine.instance.StartCoroutine("GameEventCoroutine"); 

    }

    public static void RemoveCoroutine(Callback del)
    {
        EventCoroutine.instance._callback -= del;

    }

    IEnumerator GameEventCoroutine()
    {
        yield return null;

        while(EventCoroutine.instance._callback != null)
        {
            EventCoroutine.instance._callback();
            yield return null;
        }
            
    }
}