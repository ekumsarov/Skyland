using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FrameFilledImage : UIItem
{
    public UIImage HalfFrame;
    public UIImage HalfIcon;
    public UIImage FillFrame;
    public UIImage FillIcon;

    string Frame = "cogwheel_brown_back";
    string Icon = "info";

    Action CallBack = null;
    float step = 0.0f;

    public override void Setting()
    {
        base.Setting();
    }

    public void Setup(string frame, string icon, float filled = .0f)
    {
        this.Frame = frame;
        this.Icon = icon;

        this.HalfFrame.Image = this.Frame;
        this.FillFrame.Image = this.Frame;
        this.HalfIcon.Image = this.Icon;
        this.FillIcon.Image = this.Icon;

        this.FillFrame.Fill = filled;
    }

    public void SetIcon(string icon)
    {
        this.Icon = icon;
        this.HalfIcon.Image = this.Icon;
        this.FillIcon.Image = this.Icon;
    }

    public void SetFill(float filled)
    {
        this.FillFrame.Fill = filled;
    }

    public void AddFill(float filled)
    {
        this.FillFrame.AddFill(filled);
    }
}