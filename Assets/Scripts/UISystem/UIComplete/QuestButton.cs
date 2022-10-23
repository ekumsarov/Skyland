using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lodkod;

public class QuestButton : UIItem
{

    static QuestButton copyItem;

    public UIImage Icon;

    public SkyObject parent;
    public string EventID;

    bool firstShow = true;

    public override void Setting()
    {
        base.Setting();

        this.SetAnimation(UIM.ScaleOffset(this, new Vector3(0.12f, 0.12f, 0.12f), 0.3f).SetLoops(8, true),
        "PulseAnimation");
    }


    public static void Create(string ID, SkyObject parent, string iconID = "oprions_icon", string tooltipString = "", string eventID = "")
    {
        if (QuestButton.copyItem == null)
            QuestButton.copyItem = GameObject.Instantiate(Resources.Load<QuestButton>("Prefabs/UIeX/Complete/QuestButton"));

        QuestButton temp = GameObject.Instantiate(copyItem);

        temp.parent = parent;

        if (tooltipString.Equals(""))
            temp.TooltipText = ID;
        else
            temp.TooltipText = tooltipString;

        if (eventID.Equals(""))
            temp.EventID = ID;
        else
            temp.EventID = eventID;

        temp.Icon.Image = iconID;
        temp.ID = ID;

        UIM.AddMenuItem("QuestMenu", temp);
    }

    protected override void Show()
    {
        base.Show();

        if(firstShow)
            this.PlayAnimation("PulseAnimation");
    }

    public override void Pressed()
    {
        if (parent != null)
            parent.Actioned(EventID);
        else
            Debug.LogError("No parent in quest button: " + this.ID);
    }

    public override void Selected(bool enter)
    {
        if (enter)
        {
            if (firstShow)
            {
                firstShow = false;
                this.StopAllAnimations();
            }

            if (this._tooltipActive)
                UIM.ShowTooltip(this, Lodkod.TooltipFit.Auto, Lodkod.TooltipTimeMode.ObjectManagment, Lodkod.TooltipFillMode.Instantly, Lodkod.TooltipObject.UI, this._tooltipText, lSize: 30);
        }
        else
        {
            if (this._tooltipActive)
                UIM.HideTooltip(this);
        }
    }
}