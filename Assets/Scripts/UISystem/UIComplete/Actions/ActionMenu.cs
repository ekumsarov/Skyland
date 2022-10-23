using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;
using GameEvents;

public class ActionMenu : MenuEx {

    public IconText Text;
    PanelEx ActionChoices;

    [SerializeField] PanelEx _textPanel;

    SkyObject parentDialogue;

    List<ActionButtonInfo> NextStep;
    List<ActionChoice> choices;

    bool HasText = false;

    public override void Setting()
    {
        base.Setting();

        this.NextStep = new List<ActionButtonInfo>();
        this.choices = new List<ActionChoice>();

        if (_allPanels.ContainsKey("ActionChoices"))
            ActionChoices = _allPanels["ActionChoices"];

        ActionChoices.Visible = true;

        foreach(var item in _allItems)
        {
            if (item.Value.ItemTag.Equals("ActionChoice"))
            {
                item.Value._parentMenu = this;
                choices.Add(item.Value.GetComponent<ActionChoice>());
            }
                
        }

        this.Text.PrintCompleted.AddListener(this.CompleteTextPrint);
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


        if (HasText)
        {
            _textPanel.Visible = true;
            this.Text.TypeText();
        }
        else
        {
            _textPanel.Visible = false;
            for (int i = 0; i < 6; i++)
            {
                if (i < NextStep.Count)
                {
                    this.choices[i].SetDate(NextStep[i]);
                    if (!this.choices[i].date.Avaliable())
                        this.choices[i].Visible = false;
                    else
                    {
                        this.choices[i].Reset();
                        this.choices[i].Visible = true;
                    }

                }
                else
                    this.choices[i].Visible = false;
            }
        }
        this.gameObject.SetActive(true);
        ActionReset();
    }

    public void PlayNextStep()
    {
        if (UIParameters.Action.list.Count == 0)
        {
            UIParameters.NullAction();
            this.NextStep.Clear();
            this.Close();
            GM.GameState = GameState.Game;
            return;
        }

        NextStep.AddRange(UIParameters.Action.list);

        parentDialogue = UIParameters.Action._parent;

        HasText = false;

        for (int i = 0; i < 6; i++)
        {
            this.choices[i].Visible = false;
        }

        if (!UIParameters.Action.text.IsNullOrEmpty())
        {
            this.Text.Text(UIParameters.Action.text);
            HasText = true;
        }
        else
            _textPanel.Visible = false;

        base.Open();
    }

    public override void PressedItem(UIItem data)
    {
        if (!data.ItemTag.Equals("ActionChoice"))
            return;

        ActionChoice choice = data as ActionChoice;

        if (!choice.date.CanCall())
            return;

        this.NextStep.Clear();
        UIParameters.NullAction();

        choice.date.CallAction(parentDialogue);


        this.PlayNextStep();
    }

    public void CompleteTextPrint()
    {
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
    }

    private void ActionReset()
    {
        foreach (var panel in _allItems)
        {
            panel.Value.AlphaZero();
        }

        foreach (var panel in _allPanels)
        {
            panel.Value.AlphaZero();
        }

        Canvas.ForceUpdateCanvases();

        foreach (var panel in _allItems)
        {
            panel.Value.Reset();
        }

        foreach (var panel in _allPanels)
        {
            panel.Value.Reset();
        }

        foreach (var panel in _allItems)
        {
            panel.Value.ReturnOrigin();
        }

        foreach (var panel in _allPanels)
        {
            panel.Value.ReturnOrigin();
        }
    }
}
