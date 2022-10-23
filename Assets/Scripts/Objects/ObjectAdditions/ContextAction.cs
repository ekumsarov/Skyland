using System.Collections;
using System.Collections.Generic;
using Lodkod;
using UnityEngine;


public class ContextAction : Actions
{
    public ContextAction()
    {
        list = new List<ActionButtonInfo>();
        type = ActionsSet.Context;
        OnClear = true;
        restore = true;
        Text = "";
    }

    public override void CallAction(SkyObject parent)
    {
        UIParameters.NullAction();
        UIParameters.SetAction(list, parent, text: Text);
        UIM.OpenMenu("DialogueMenu");
    }
}