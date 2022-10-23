using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FilledItem : UIItem
{
    public UIImage HalfIcon;
    public UIImage FillIcon;
    string Icon = "BaseIcon";

    Action CallBack = null;
    float step = 0.0f;

    public override void Setting()
    {
        base.Setting();
    }

    public void Setup(string icon, float filled = .0f)
    {
        this.Icon = icon;
        this.HalfIcon.Image = this.Icon;
        this.FillIcon.Image = this.Icon;

        this.FillIcon.Fill = filled;
    }

    public void SetIcon(string icon)
    {
        this.Icon = icon;
        this.HalfIcon.Image = this.Icon;
        this.FillIcon.Image = this.Icon;
    }

    public void SetFill(float filled)
    {
        this.FillIcon.Fill = filled;
    }

    public void AddFill(float filled)
    {
        this.FillIcon.AddFill(filled);
    }
}
