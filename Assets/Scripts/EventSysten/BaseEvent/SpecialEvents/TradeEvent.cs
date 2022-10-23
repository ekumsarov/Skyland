using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;

public class TradeEvent : GameEvent
{

    public override void PrepareEvent(JSONNode node)
    {
        this.ID = "TradeEvent";
    }

    public override bool CanActive()
    {
        return true;
    }

    public override void Start()
    {
        List<ActionButtonInfo> list = new List<ActionButtonInfo>();

        Debug.LogError("TRADE WORKS ONLY WITH ATTACHED GAMEOBJECT OR TRADE NOT WORK!!!!");
    }
}
