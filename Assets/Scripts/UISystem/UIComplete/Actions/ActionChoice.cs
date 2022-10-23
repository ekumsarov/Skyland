using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class ActionChoice : UIItem {

    public IconText ShowedText;
    public ActionButtonInfo date;

    public string text
    {
        set
        {
           ShowedText.Text(value);
           ShowedText.ShowComplete();
        }
    }

    public override void Pressed()
    {
        base.Pressed();
    }

    public override void Setting()
    {
        base.Setting();

        this.ItemTag = "ActionChoice";
    }

    public override void SetInfo(JSONNode node)
    {
        if (node["Text"] == null)
        {
            Debug.LogError("Something Wrong");
            return;
        }

        base.SetInfo(node);
        this.text = node["Text"].Value;
    }

    public void SetDate(ActionButtonInfo dat)
    {
        this.date = dat;
        this.text = LocalizationManager.Get(dat.Text);
    }

    public void SetDate(ActionButtonInfo dat, int point = -1)
    {
        this.date = dat;
        if(point == -1)
            this.text = LocalizationManager.Get(dat.Text);
        else
        {
            this.text = point + ". " + LocalizationManager.Get(dat.Text);
        }
            
    }

    public override void Reset()
    {
        base.Reset();

        this.ShowedText.ResetIconPosition();
    }
}
