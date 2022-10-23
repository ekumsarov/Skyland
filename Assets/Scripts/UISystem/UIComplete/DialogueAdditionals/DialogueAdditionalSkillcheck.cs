using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lodkod;

public class DialogueAdditionalSkillcheck : UIItem
{
    [SerializeField] SimpleText Title;
    [SerializeField] SimpleText NeedLabel;
    [SerializeField] UIIconText ResultLabel;

    [SerializeField] List<ResultSkillItem> skills;
    [SerializeField] List<FilledItem> results;

    public override void Setting()
    {
        base.Setting();

        Title.Text = LocalizationManager.Get("RequirementLabel");
        NeedLabel.Text = LocalizationManager.Get("CheckLabel");

        Title.Visible = true;
        NeedLabel.Visible = true;
        ResultLabel.Visible = true;

        for (int i = 0; i < results.Count; i++)
        {
            results[i].SetIcon("SuccessIcon");
            results[i].SetFill(0);
            results[i].Visible = false;
        }
    }

    public void SetupPanel(SkyObject parent, ActionButtonInfo choice)
    {
        

        SkillCheckAction action = parent.GetAction(choice.GetCallID()) as SkillCheckAction;
        if(action != null)
        {
            GroupSkillCheck checks = action.GetCheck();
            for(int i = 0; i < skills.Count; i++)
            {
                if (i < checks.Skills.Count)
                {
                    skills[i].Setup(checks.Skills[i]);
                    skills[i].Visible = true;
                }
                else
                    skills[i].Visible = false;
            }

            int maxResult = checks.MaxResult(GM.PlayerIcon.Group.GetUnits());
            for(int i = 0; i < results.Count; i++)
            {
                if (i < maxResult)
                {
                    results[i].Setup("SuccessIcon", 1f);
                    results[i].Visible = true;
                }
                else
                    results[i].Visible = false;
            }

            ResultLabel.IconText.Text(LocalizationManager.Get("ResultCehckLabel", checks.Amount));
            ResultLabel.IconText.ShowComplete();
            

            this.Visible = true;
        }
    }

    public void StartSkillCheck(Action callback)
    {
        UIParameters.SkillCheck.SuccessCheck.CompleteCheck(GM.Player.Group.GetUnits());
        ResultLabel.IconText.Text(LocalizationManager.Get("ResultLabel"));
        ResultLabel.IconText.ShowComplete();

        StartCoroutine(Animation(callback));
    }

    IEnumerator Animation(Action callback)
    {
        for (int i = 0; i < this.results.Count; i++)
        {
            this.results[i].Visible = false;
            this.results[i].SetFill(0);
        }
        yield return null;

        for (int i = 0; i < UIParameters.SkillCheck.SuccessCheck.Skills.Count; i++)
        {
            if (skills[i].Visible)
                skills[i].StartAnimation(UIParameters.SkillCheck.SuccessCheck.Skills[i].ComplexResult);
        }

        yield return new WaitForSeconds(2f);

        float stepFill = 1.0f / UIParameters.SkillCheck.SuccessCheck.CombinedComplex;
        float step = 0.5f / UIParameters.SkillCheck.SuccessCheck.CombinedComplex;
        float timer = 0.0f;
        int flip = 0;
        int resAm = UIParameters.SkillCheck.SuccessCheck.ComplexResult;
        int diceResult = UIParameters.SkillCheck.SuccessCheck.FinalResult;

        int startDice = 0;

        this.results[startDice].SetFill(0f);
        this.results[startDice].Visible = true;

        List<float> skillSteps = new List<float>();
        List<float> skillStepSave = new List<float>();
        int biggest = 0;
        for (int i = 0; i < UIParameters.SkillCheck.SuccessCheck.Skills.Count; i++)
        {
            if (biggest < UIParameters.SkillCheck.SuccessCheck.Skills[i].Complex)
                biggest = UIParameters.SkillCheck.SuccessCheck.Skills[i].Complex;
        }

        for (int i = 0; i < UIParameters.SkillCheck.SuccessCheck.Skills.Count; i++)
        {
            skillSteps.Add(UIParameters.SkillCheck.SuccessCheck.Skills[i].ComplexResult / biggest);
            skillStepSave.Add(UIParameters.SkillCheck.SuccessCheck.Skills[i].ComplexResult);
        }

        while (diceResult > 0)
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
            if (flip >= UIParameters.SkillCheck.SuccessCheck.CombinedComplex)
            {
                this.results[startDice].SetFill(1.0f);
                flip = 0;
                startDice += 1;
                this.results[startDice].SetFill(0f);
                this.results[startDice].Visible = true;
                diceResult--;
            }
            else
            {
                this.results[startDice].AddFill(stepFill);
                for (int j = 0; j < UIParameters.SkillCheck.SuccessCheck.Skills.Count; j++)
                {
                    skillStepSave[j] -= skillSteps[j];
                    this.skills[j].SetValue((int)skillSteps[j]);
                }
            }

            yield return null;
        }

        if (UIParameters.SkillCheck.SuccessCheck.CompleteDice)
            this.results[startDice].SetFill(1);
        else
            this.results[startDice].Visible = false;

        yield return new WaitForSeconds(0.8f);

        if(UIParameters.SkillCheck.SuccessCheck.CheckResult)
            ResultLabel.IconText.Text(LocalizationManager.Get("SuccessLabel").ToUpper());
        else
            ResultLabel.IconText.Text(LocalizationManager.Get("FailLabel").ToUpper());
        ResultLabel.IconText.ShowComplete();

        yield return new WaitForSeconds(1.2f);

        callback?.Invoke();
    }
}
