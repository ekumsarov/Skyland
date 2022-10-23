using System.Collections;
using System.Collections.Generic;
using Lodkod;
using UnityEngine;

public class BuildAction : Actions
{
    public BuildAction()
    {
        list = new List<ActionButtonInfo>();
        type = ActionsSet.build;
        OnClear = true;
        restore = true;
        Text = "";
    }

    public override void CallAction(SkyObject parent)
    {
        //if(parent.GetComponent<MainBuild>() != null)
       //     UIM.OpenBuildMenu(parent.GetComponent<MainBuild>());
    }
}