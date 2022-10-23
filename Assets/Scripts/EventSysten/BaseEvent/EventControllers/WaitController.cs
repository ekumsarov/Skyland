using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class WaitController : MonoBehaviour {

    public GameEvent gEvent;
    float seconds;

    public void Wait(float second, GameEvent ev)
    {
        gEvent = ev;
        seconds = second;

        if (seconds > 0f)
            StartCoroutine("Waiting");
        else
            gEvent.End();
    }

    public IEnumerator Waiting()
    {
        while (seconds > 0)
        {
            seconds -= Time.deltaTime;
            yield return null;
        }

        gEvent.End();
    }
}
