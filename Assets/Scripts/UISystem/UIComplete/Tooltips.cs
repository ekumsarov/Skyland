using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lodkod;
using GameEvents;

public class Tooltips : MenuEx {

    List<Tooltip> _tolltips;
    public override void Setting()
    {
        base.Setting();

        _tolltips = new List<Tooltip>();
        foreach(var panel in _allPanels)
        {
            _tolltips.Add(panel.Value as Tooltip);
        }
    }

    protected override void Show()
    {
        if (this._lock == true)
            return;

        this.gameObject.SetActive(true);
    }

    public void ShowTooltip(SkyObject obj, TooltipFit fit, TooltipTimeMode timeMode, TooltipFillMode fillMode, TooltipObject objectMode, string Text, GameEvent gEvent = null, Action callback = null, float time = 1.0f, int lSize = 0)
    {
        Tooltip temp = null;

        for (int i = 0; i < _tolltips.Count; i++)
        {
            if (_tolltips[i].obj == obj)
                return;
        }

        for (int i = 0; i < _tolltips.Count; i++)
        {
            if (!_tolltips[i].Visible)
            {
                temp = _tolltips[i];
                break;
            }
        }

        temp.exTime = time;
        temp.obj = obj;
        temp.fit = fit;
        temp.timeMode = timeMode;
        temp.fillMode = fillMode;
        temp.objectMode = objectMode;
        temp.LenghtSize = lSize;
        temp.Text = Text;

        if (gEvent != null)
            temp.ActionCallback = gEvent.End;
        else
            temp.ActionCallback = callback;

        temp.Visible = true;
    }

    public void ShowTooltip(Vector3 target, TooltipFit fit, TooltipTimeMode timeMode, TooltipFillMode fillMode, TooltipObject objectMode, string Text, GameEvent gEvent = null, Action callback = null, float time = 1.0f, int lSize = 0)
    {
        Tooltip temp = null;
        for (int i = 0; i < _tolltips.Count; i++)
        {
            if (!_tolltips[i].Visible)
            {
                temp = _tolltips[i];
                break;
            }
        }

        temp.exTime = time;
        temp.target = target;
        temp.fit = fit;
        temp.timeMode = timeMode;
        temp.fillMode = fillMode;
        temp.objectMode = objectMode;
        temp.LenghtSize = lSize;
        temp.Text = Text;

        if (gEvent != null)
            temp.ActionCallback = gEvent.End;
        else
            temp.ActionCallback = callback;
            

        temp.Visible = true;
    }

    public void HideTooltip(SkyObject obj)
    {
        foreach (var tooltip in _tolltips)
        {
            if (tooltip.obj == obj)
            {
                tooltip.obj = null;
                tooltip.Visible = false;
            }
        }
    }
}
