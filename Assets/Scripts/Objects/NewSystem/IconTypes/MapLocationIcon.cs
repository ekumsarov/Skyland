using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using EventPackSystem;
using GameEvents;
using UnityEngine.UI;
using System;

public class MapLocationIcon : IconObject
{
    public MapLocationObject parent;

    public string AfterZoomPack;

    public override void HardSet()
    {
        base.HardSet();

        if(parent != null)
        {
            this.MainEvent = "StandartAction";

            Actions moveAct = Actions.Get("Context");

            moveAct.ID = "StandartAction";
            moveAct.AddChoice(ActionButtonInfo.Create("MoveTo").SetCallData("MoveToLocation").SetType(ActionType.Pack));
            moveAct.AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close));

            this.AddAction(moveAct);

            _act.PushPack("MoveToLocation", new List<GameEvent>()
            {

                ActivateLocationOnIsland.Create(parent.IslandNumber, false),
                ZoomIsland.Create("island_" + parent.IslandNumber),
                MoveIcon.Create("Player", parent.GetQuitObject().ID),
                MoveCameraToPoint.Create(parent.CameraPoint, false, parent.CameraFlyTime),
                SafeCallFunction.Create("OpenLocations", parent.ID),
                SafeCallFunction.Create("CallAfterMovePack", this.ID)
            });
        }
    }

    public void SetupMapIcon(MapLocationObject obj)
    {
        this.parent = obj;
        this.HardSet();
    }

    protected override void ClickedIcon()
    {
        if (GM.GameState != GameState.Game || this._lock)
            return;

        if (this.MainEvent.IsNullOrEmpty())
            this.MainEvent = "MoveToLocation";

        this.Actioned(this.MainEvent);
    }

    public void CallAfterMovePack(Action del = null)
    {
        del?.Invoke();

        if (!this.AfterZoomPack.IsNullOrEmpty())
            this.Activity.callActivityPack(this.AfterZoomPack);

    }

    //86 10.6 -0.45
    //14.6 -23.7
}