using System.Collections;
using System.Collections.Generic;
using Lodkod;
using UnityEngine;

public class MapAction : Actions
{
    public MapAction()
    {
        list = new List<ActionButtonInfo>();
        type = ActionsSet.Map;
        OnClear = true;
        restore = true;
        Text = "";
    }

    public override void CallAction(SkyObject parent)
    {
        UIM.OpenMenu("MapMenu");
    }
}