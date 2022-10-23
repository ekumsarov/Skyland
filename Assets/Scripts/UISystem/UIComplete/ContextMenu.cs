using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using Lodkod;
using GameEvents;

public class ContextMenu : MenuEx
{

    Tooltip TooltipView;
    PanelEx ActionChoices;

    SkyObject parentDialogue;
    SkyObject ActionPosition;

    List<ActionButtonInfo> NextStep;
    List<ActionChoice> choices;

    string text;
    bool HasText;

    public override void Setting()
    {
        base.Setting();

        this.NextStep = new List<ActionButtonInfo>();

        if (_allPanels.ContainsKey("TooltipView"))
            TooltipView = _allPanels["TooltipView"] as Tooltip;

        if (_allPanels.ContainsKey("ActionChoices"))
            ActionChoices = _allPanels["ActionChoices"];

        this.choices = new List<ActionChoice>();
        foreach (var item in _allItems)
        {
            if (item.Value.ItemTag.Equals("ActionChoice"))
            {
                item.Value._parentMenu = this;
                choices.Add(item.Value.GetComponent<ActionChoice>());
            }
        }

        TooltipView.timeMode = TooltipTimeMode.Dialog;
        TooltipView.fillMode = TooltipFillMode.Type;
        TooltipView.objectMode = TooltipObject.Game;
        TooltipView.LenghtSize = 45;
        TooltipView.exTime = 0.0f;
        TooltipView.Text = "NoText";
        TooltipView.ActionCallback = null;
        TooltipView.ActionCallback = CompleteTextPrint;
    }

    public override void Open()
    {
        GM.GameState = GameState.Action;
        PlayNextStep();
    }

    protected override void Show()
    {
        if (this._lock == true)
            return;

        this.gameObject.SetActive(true);

        ResetAll();

        if (HasText)
        {
            ActionChoices.Visible = false;
            TooltipView.Visible = true;

            Overlapped();
        }
        else
        {
            ActionChoices.Visible = true;
            TooltipView.Visible = false;
        }

    }

    public void PlayNextStep()
    {
        if (UIParameters.Action.list.Count == 0 && UIParameters.Action.text == null)
        {
            UIParameters.NullAction();
            this.NextStep.Clear();
            this.TooltipView.Visible = false;
            this.ActionChoices.Visible = false;
            this.Close();

            GM.GameState = GameState.Game;
            
            return;
        }
        GM.GameState = GameState.Action;
        NextStep.AddRange(UIParameters.Action.list);

        this.TooltipView.Visible = false;
        this.ActionChoices.Visible = false;

        parentDialogue = UIParameters.Action._parent;
        ActionPosition = UIParameters.Action.ActionPosition;

        text = UIParameters.Action.text;
        HasText = false;

        for (int i = 0; i < 6; i++)
        {
            if (i < NextStep.Count)
            {
                this.choices[i].SetDate(NextStep[i]);
                if (!this.choices[i].date.Avaliable())
                    this.choices[i].Visible = false;
                else 
                    this.choices[i].Visible = true;
            }
            else
                this.choices[i].Visible = false;
        }

        if (!text.IsNullOrEmpty())
        {
            TooltipView.Text = LocalizationManager.Get(text);
            TooltipView.obj = parentDialogue;
            //TooltipView.target = new Vector3(600f, 600f);
            HasText = true;
        }
        else
            ActionChoices.transform.position = parentDialogue.ScreePoint;

        this.NextStep.Clear();

        base.Open();
    }

    public override void PressedItem(UIItem data)
    {
        if (!data.ItemTag.Equals("ActionChoice"))
            return;

        ActionChoice choice = data as ActionChoice;

        if(!choice.date.CanCall())
        {
            string text = choice.date.CantCallText();
            if(!text.IsNullOrEmpty())
                UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Tootip, TooltipFillMode.Instantly, TooltipObject.UI,
                    text, lSize:35, time:2f);
            return;
        }
            

        this.TooltipView.Visible = false;
        this.ActionChoices.Visible = false;
        this.NextStep.Clear();
        UIParameters.NullAction();

        choice.date.CallAction(parentDialogue);

        
        this.PlayNextStep();
    }

    public void CompleteTextPrint()
    {
        if (ActionPosition == null)
            ActionChoices.transform.position = new Vector3(TooltipView.Rect.position.x - ActionChoices.Rect.sizeDelta.x / 2 - TooltipView.Rect.sizeDelta.x / 2 - 10f, TooltipView.transform.position.y, TooltipView.Rect.position.z);
        else
            ActionChoices.transform.position = ActionPosition.transform.position;

//        ActionChoices.transform.position = new Vector3(300f, 600f);

        ActionChoices.Visible = true;
    }
}