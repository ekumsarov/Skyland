using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using EventPackSystem;
using GameEvents;
using UnityEngine.UI;

public class LocationIcon : IconObject
{
    public LocationObject parent;

    public string AfterZoomPack;

    public override void HardSet()
    {
        base.HardSet();

        this.MainEvent = "MoveToLocation";
        this.AfterZoomPack = string.Empty;
    }

    protected override void ClickedIcon()
    {
        if (GM.GameState != GameState.Game || this._lock)
            return;

        if (this.MainEvent.IsNullOrEmpty())
            this.MainEvent = "MoveToLocation";

        this.Activity.callActivityPack(this.MainEvent);

        if (!this.AfterZoomPack.IsNullOrEmpty())
            this.Activity.callActivityPack(this.AfterZoomPack);
    }

    //86 10.6 -0.45
    //14.6 -23.7
}