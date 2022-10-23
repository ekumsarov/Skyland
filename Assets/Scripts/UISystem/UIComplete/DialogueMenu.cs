using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Lodkod;
using GameEvents;
using System;

public class DialogueMenu : MenuEx
{

    [SerializeField] IconText Text;
    [SerializeField] PanelEx GlobalPanel;
    [SerializeField] UIItem ActionChoices;

    [SerializeField] UIItem _textPanel;
    [SerializeField] DialogueAdditionalText _textAddPanel;
    [SerializeField] DialogueAdditionalResource _resourcePanel;
    [SerializeField] DialogueAdditionalSkillcheck _skillcheckPanel;

    SkyObject parentDialogue;
    SkyObject ActionPosition;

    List<ActionButtonInfo> NextStep;
    List<ActionChoice> choices;

    bool HasText = false;
    bool StartSkillCheck = false;

    public override void Setting()
    {
        base.Setting();

        this.NextStep = new List<ActionButtonInfo>();
        this.choices = new List<ActionChoice>();

        ActionChoices.Visible = false;

        foreach (var item in _allItems)
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
        if(StartSkillCheck)
        {
            _skillcheckPanel.StartSkillCheck(CompleteCheck);
            return;
        }

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
            _textPanel.Visible = true;
            this.Text.TypeText();
            return;
        }
        else
        {
            _textPanel.Visible = false;
            ActionChoices.Visible = true;
        }
        
    }

    public void PlayNextStep()
    {
        if (UIParameters.Action.list.Count == 0)
        {
            UIParameters.NullAction();
            this.NextStep.Clear();
            this.Close();
            GM.GameState = GameState.Game;
            StartSkillCheck = false;
            return;
        }

        _resourcePanel.Visible = false;
        _textAddPanel.Visible = false;
        _textPanel.Visible = false;
        _skillcheckPanel.Visible = false;
        ActionChoices.Visible = false;

        NextStep.AddRange(UIParameters.Action.list);

        parentDialogue = UIParameters.Action._parent;
        ActionPosition = UIParameters.Action.ActionPosition;

        HasText = false;

        int choiceIndex = 1;
        for (int i = 0; i < 6; i++)
        {
            if (i < NextStep.Count)
            {
                this.choices[i].SetDate(NextStep[i], choiceIndex);
                if (!this.choices[i].date.Avaliable())
                    this.choices[i].Visible = false;
                else
                {
                    this.choices[i].Selected(false);
                    this.choices[i].Reset();
                    this.choices[i].Visible = true;
                    choiceIndex++;
                }

            }
            else
                this.choices[i].Visible = false;
        }

        if (!UIParameters.Action.text.IsNullOrEmpty())
        {
            this.Text.Text(LocalizationManager.Get(UIParameters.Action.text));
            HasText = true;
        }

        base.Open();
    }

    public override void SelectedItem(UIItem data, bool enter)
    {
        if (StartSkillCheck)
            return;

        if (!data.ItemTag.Equals("ActionChoice"))
            return;

        if(enter)
        {
            ActionChoice info = data as ActionChoice;
            if (info.date.AdditionalType == ActionChoiceType.AdditionalText)
                _textAddPanel.SetupPanel(info.date);
            else if (info.date.AdditionalType == ActionChoiceType.Resource)
                _resourcePanel.SetupPanel(info.date);
            else if (info.date.AdditionalType == ActionChoiceType.Skillcheck)
                _skillcheckPanel.SetupPanel(parentDialogue, info.date);
        }
        else
        {
            _textAddPanel.Visible = false;
            _resourcePanel.Visible = false;
            _skillcheckPanel.Visible = false;
        }
    }

    public override void PressedItem(UIItem data)
    {
        if (StartSkillCheck)
            return;

        if (!data.ItemTag.Equals("ActionChoice"))
            return;

        ActionChoice choice = data as ActionChoice;

        if (!choice.date.CanCall())
            return;

        if (choice.date.CallType == ActionType.SkillCheck)
        {
            this.NextStep.Clear();
            UIParameters.NullAction();

            this.StartSkillCheck = true;
            choice.date.CallAction(parentDialogue);

            return;
        }

        this.NextStep.Clear();
        UIParameters.NullAction();

        choice.date.CallAction(parentDialogue);


        this.PlayNextStep();
    }

    public void CompleteTextPrint()
    {
        Text.ShowComplete();
        if (ActionPosition != null)
                ActionChoices.transform.position = ActionPosition.transform.position;

        ActionChoices.Visible = true;
    }

    public void CompleteCheck()
    {
        UIParameters.SkillCheck.result.CallResult(UIParameters.SkillCheck._parent, UIParameters.SkillCheck.SuccessCheck.CheckResult);
        StartSkillCheck = false;

        NextStep.Clear();
        this.PlayNextStep();
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

    void Update()
    {
        if (Input.inputString != "")
        {
            int number;
            bool is_a_number = Int32.TryParse(Input.inputString, out number);
            if (is_a_number && number >= 1 && number < 10)
            {
                if(number - 1 < this.choices.Count && this.choices[number-1].Visible)
                {
                    this.KeySelected(this.choices[number-1]);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            CompleteTextPrint();
        }
    }

    private bool isSelectenInKey = false;
    private void KeySelected(ActionChoice choice)
    {
        if (isSelectenInKey)
        {
            isSelectenInKey = false;
            this.PressedItem(choice);
        }
        else
        {
            if(choice.date.AdditionalType == ActionChoiceType.AdditionalText ||
                choice.date.AdditionalType == ActionChoiceType.Resource ||
                choice.date.AdditionalType == ActionChoiceType.Skillcheck)
            {
                isSelectenInKey = true;
                this.SelectedItem(choice, true);
            }
            else
            {
                isSelectenInKey = false;
                this.PressedItem(choice);
            }
        }
    }
}