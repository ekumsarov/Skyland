using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using BattleActions;
using BattleEffects;

public class HeroPanel : UIItem
{
    
    [SerializeField] public List<SkillItem> _skills;

    [SerializeField] public UIItem _actionPanel;
    [SerializeField] public List<BattleActionItem> _actions;

    [SerializeField] public UIItem _effectPanel;
    [SerializeField] public List<BattleEffectItem> _effects;

    [SerializeField] public UIItem _battleCard;
    [SerializeField] public UIImage _cardIcon;
    [SerializeField] public SimpleText _cardName;
    [SerializeField] public SimpleText _cardInfo;

    [SerializeField] public UIItem _checkPanel;
    [SerializeField] public List<FilledItem> _checks;

    [SerializeField] public UIItem _resultPanel;
    [SerializeField] public List<ResultSkillItem> _results;

    [SerializeField] public UIItem _confirmButton;

    [SerializeField] public SimpleText _skillLabel;
    [SerializeField] public SimpleText _actionLabel;
    [SerializeField] public SimpleText _effectLabel;
    [SerializeField] public SimpleText _cardLabel;
    [SerializeField] public SimpleText _checkLabel;
    [SerializeField] public SimpleText _resultLabel;

    private HeroInfoItem _panelBindedUnit;

    private string _maxResultString;
    private string _resultString;

    private string _needsString;
    private string _checkString;

    private string _blockLabel;

    public override void Setting()
    {
        base.Setting();

        _skillLabel.Text = LocalizationManager.Get("SkillLabel").ToUpper();
        _actionLabel.Text = LocalizationManager.Get("ActionLabel").ToUpper();
        _effectLabel.Text = LocalizationManager.Get("EffectLabel").ToUpper();
        _cardLabel.Text = LocalizationManager.Get("CardLabel").ToUpper();
        _checkLabel.Text = LocalizationManager.Get("MaxResultLabel").ToUpper();
        _resultLabel.Text = LocalizationManager.Get("CheckLabel").ToUpper();

        _maxResultString = LocalizationManager.Get("MaxResultLabel").ToUpper();
        _resultString = LocalizationManager.Get("ResultLabel").ToUpper();

        _needsString = LocalizationManager.Get("NeedBattleLabel").ToUpper();
        _checkString = LocalizationManager.Get("CheckBattleLabel").ToUpper();

        _blockLabel = LocalizationManager.Get("BlockLabel").ToUpper();

        _cardIcon.Visible = false;
        _cardInfo.Visible = false;
        _cardName.Visible = false;

        foreach(var item in _actions)
        {
            item.Visible = false;
        }

        foreach(var item in _effects)
        {
            item.Visible = false;
        }

        foreach(var item in _results)
        {
            item.Visible = false;
        }

        foreach(var item in _checks)
        {
            item.Visible = false;
        }
    }

    public void RegisterHero(HeroInfoItem unit)
    {
        if(unit == null)
        {

            this._panelBindedUnit = null;

            for(int i = 0; i < this._actions.Count; i++)
            {
                this._actions[i].Visible = false;
            }

            for(int i = 0; i < this._effects.Count; i++)
            {
                this._effects[i].Visible = false;
            }

            _cardIcon.Visible = false;
            _cardName.Visible = false;
            _cardInfo.Visible = false;

            for(int i = 0; i < this._results.Count; i++)
            {
                this._results[i].Visible = false;
            }

            for(int i = 0; i < this._checks.Count; i++)
            {
                this._checks[i].Visible = false;
            }

            return;
        }

        this._panelBindedUnit = unit;

        int index = 0;
        foreach(var skill in unit.bindUnit.skills)
        {
            _skills[index].Setup(skill.Value);
            index++;
            if (index >= 5)
                break;
        }

        for(int i = 0; i < _actions.Count; i++)
        {
            if (i < unit.bindUnit.actions.Count)
            {
                BattleActionInfo actInfo = null;
                if (IOM.BattleActionInfoDic.ContainsKey(unit.bindUnit.actions[i]))
                    actInfo = IOM.BattleActionInfoDic[unit.bindUnit.actions[i]];
                else
                    continue;

                _actions[i].SetAction(actInfo.Icon, actInfo.Name, BattleAction.loadBattleAction(actInfo.Name, unit));
                _actions[i].Visible = true;
            }
            else
                _actions[i].Visible = false;
            
        }

        for(int i = 0; i < _effects.Count; i++)
        {
            if (i < unit.bindUnit._effects.Count)
            {
                _effects[i].SetEffect(unit.bindUnit._effects[i].Icon, unit.bindUnit._effects[i].ID, unit.bindUnit._effects[i]);
                _effects[i].Visible = true;
            }
            else
                _effects[i].Visible = false;
        }

        this.Visible = true;
    }

    public void RegisterAction(BattleAction action)
    {
        _cardIcon.Image = action.Icon;
        _cardName.Text = LocalizationManager.Get(action.ActName).ToUpper();
        _cardInfo.Text = action.GetDescription();

        for(int i = 0; i < _results.Count; i++)
        {
            if (i < action.SuccessCheck.Skills.Count)
            {
                _results[i].Setup(action.SuccessCheck.Skills[i]);
                _results[i].Visible = true;
            }
            else
                _results[i].Visible = false;
        }

        int max = action.SuccessCheck.MaxResult(_panelBindedUnit);
        for(int i = 0; i < _checks.Count; i++)
        {
            if (i < max)
            {
                _checks[i].SetIcon("SuccessIcon");
                _checks[i].SetFill(1f);
                _checks[i].Visible = true;
            }
            else
                _checks[i].Visible = false;
        }

        _checkLabel.Text = _needsString;
        _resultLabel.Text = _maxResultString;

        _cardIcon.Visible = true;
        _cardName.Visible = true;
        _cardInfo.Visible = true;
    }

    public void RegisterActionAnswer(BattleAction action)
    {
        _cardIcon.Image = "Shield";
        _cardName.Text = _blockLabel;
        _cardInfo.Text = action.GetActionAnswer();

        if(action.AvoidCheck != null)
        {
            for (int i = 0; i < _results.Count; i++)
            {
                if (i < action.AvoidCheck.Skills.Count)
                {
                    _results[i].Setup(action.AvoidCheck.Skills[i]);
                    _results[i].Visible = true;
                }
                else
                    _results[i].Visible = false;
            }
        }

        int max = action.AvoidCheck.MaxResult(_panelBindedUnit);
        for (int i = 0; i < _checks.Count; i++)
        {
            if (i < max)
            {
                _checks[i].SetIcon("SuccessIcon");
                _checks[i].Visible = true;
            }
            else
                _checks[i].Visible = false;
        }

        _checkLabel.Text = _needsString;
        _resultLabel.Text = _maxResultString;

        _cardIcon.Visible = true;
        _cardName.Visible = true;
        _cardInfo.Visible = true;
    }

    public void ClearCheck()
    {
        _cardIcon.Visible = false;
        _cardName.Visible = false;
        _cardInfo.Visible = false;

        for(int i = 0; i < _results.Count; i++)
        {
            _results[i].Visible = false;
        }

        for (int i = 0; i < _checks.Count; i++)
        {
            _checks[i].Visible = false;
        }
    }

    public void ShowCheckSkills(GroupSkillCheck skills)
    {
        _checkLabel.Text = _checkString;
        _resultLabel.Text = _resultString;

        for (int i = 0; i < _checks.Count; i++)
            _checks[i].Visible = false;

        for(int i = 0; i < skills.Skills.Count; i++)
        {
            if(_results[i].Visible)
                _results[i].StartAnimation(skills.Skills[i].ComplexResult);
        }
    }

    public void ShowResult(GroupSkillCheck skills, string icon, System.Action callback)
    {
        StartCoroutine(StartShowResult(skills, icon, callback));
    }

    IEnumerator StartShowResult(GroupSkillCheck skills, string icon, System.Action callback)
    {
        float stepFill = 1.0f / skills.CombinedComplex;
        float step = 0.5f / skills.CombinedComplex;
        float timer = 0.0f;
        int flip = 0;
        int resAm = skills.ComplexResult;
        int diceResult = skills.FinalResult;

        int startDice = 0;

        this._checks[startDice].SetIcon(icon);
        this._checks[startDice].SetFill(0f);
        this._checks[startDice].Visible = true;

        List<float> skillSteps = new List<float>();
        List<float> skillStepSave = new List<float>();
        int biggest = 0;
        for(int i = 0; i < skills.Skills.Count; i++)
        {
            if (biggest < skills.Skills[i].Complex)
                biggest = skills.Skills[i].Complex;
        }

        for (int i = 0; i < skills.Skills.Count; i++)
        {
            skillSteps.Add(skills.Skills[i].ComplexResult / biggest);
            skillStepSave.Add(skills.Skills[i].ComplexResult);
        }

        while(diceResult > 0)
        {
            timer = 0.0f;
            while (timer < step)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            flip += 1;
            resAm -= 1;
            //            this.ChanceText.Text = LocalizationManager.Get("ResultSkillCheck", resAm
            if (flip >= skills.CombinedComplex)
            {
                this._checks[startDice].SetFill(1.0f);
                flip = 0;
                startDice += 1;
                this._checks[startDice].SetIcon(icon);
                this._checks[startDice].SetFill(0f);
                this._checks[startDice].Visible = true;
                diceResult--;
            }
            else
            {
                this._checks[startDice].AddFill(stepFill);
                for (int j = 0; j < skills.Skills.Count; j++)
                {
                    skillStepSave[j] -= skillSteps[j];
                    this._results[j].SetValue((int)skillSteps[j]);
                }
            }

            yield return null;
        }

        if (skills.CompleteDice)
            this._checks[startDice].SetFill(1);
        else
            this._checks[startDice].Visible = false;

        callback?.Invoke();
    }
}
