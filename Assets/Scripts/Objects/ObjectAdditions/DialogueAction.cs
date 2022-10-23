using System.Collections;
using System.Collections.Generic;
using Lodkod;
using UnityEngine;


public class DialogueAction : Actions
{
    public DialogueAction()
    {
        list = new List<ActionButtonInfo>();
        type = ActionsSet.dialogue;
        OnClear = true;
        restore = true;
        Text = "AIPangramm";
    }

    public override void CallAction(SkyObject parent)
    {
        UIParameters.SetAction(list, parent, text: Text);
        UIM.OpenMenu("ActionMenu");
    }
}