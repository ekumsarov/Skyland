using System.Collections;
using System.Collections.Generic;
using Lodkod;
using UnityEngine;


public class QuestAction : Actions
{
    public QuestAction()
    {
        list = new List<ActionButtonInfo>();
        type = ActionsSet.quest;
        OnClear = true;
        restore = true;
        Text = "AIPangramm";
    }

    public override void CallAction(SkyObject parent)
    {
        UIM.ShowTooltip(new Vector3(Screen.width / 2, Screen.height / 2, 1f), TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, Text, null, null, 0, 40);
    }
}