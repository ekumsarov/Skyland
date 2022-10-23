using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using Lodkod;
using GameEvents;

public class SkillCheckMenu : MenuEx
{
    PanelEx ActionChoices;

    List<ActionChoice> choices;

    List<FilledItem> successes;
    public SimpleText ChanceText;

    List<FilledItem> nessarys;
    public SimpleText NessaryText;

    public override void Setting()
    {
        base.Setting();

        if (_allPanels.ContainsKey("ActionChoices"))
            ActionChoices = _allPanels["ActionChoices"];

        this.choices = new List<ActionChoice>();
        this.successes = new List<FilledItem>();
        this.nessarys = new List<FilledItem>();
        foreach (var item in _allItems)
        {
            if (item.Value.ItemTag.Equals("ActionChoice"))
            {
                item.Value._parentMenu = this;
                choices.Add(item.Value.GetComponent<ActionChoice>());
                item.Value.Visible = false;
            }

            if (item.Value.ItemTag.Equals("FilledImaged"))
            {
                item.Value._parentMenu = this;
                successes.Add(item.Value.GetComponent<FilledItem>());
                item.Value.Visible = false;
            }

            if (item.Value.ItemTag.Equals("NecImaged"))
            {
                item.Value._parentMenu = this;
                nessarys.Add(item.Value.GetComponent<FilledItem>());
                item.Value.Visible = false;
            }
        }

        this.choices[0].Visible = true;
        this.choices[0].SetDate(
                ActionButtonInfo.Create("Start")
                .SetText("StartCkillCheck")
                .SetCallData("Start")
            );
        this.choices[4].Visible = true;
        this.choices[4].SetDate(
                ActionButtonInfo.Create("Back")
                .SetText("BackAction")
                .SetCallData("Back")
            );
    }

    public override void Open()
    {
        Prepare();

    }

    protected override void Show()
    {
        if (this._lock == true)
            return;

        this.gameObject.SetActive(true);

        ResetAll();

        ActionChoices.Visible = true;
        this._allPanels["Result"].Visible = true;

    }

    public void Prepare()
    {
        if(UIParameters.SkillCheck.LootHelp != null)
        {
            int count = 1;
            for(int i = 0; i < 3; i++)
            {
                if (i >= 3)
                    break;

                if(i < UIParameters.SkillCheck.LootHelp.Count)
                {
                    LootItem lItem = LS.GetItem(UIParameters.SkillCheck.LootHelp[i]);
                    if (lItem == null)
                        continue;

                    string helpParameters = "";
                    foreach (var skill in lItem.skillCheckHelp)
                        helpParameters += "\n" + LocalizationManager.Get(skill.ID) + " " + skill.Min.ToString();

                    this.choices[count].SetDate(
                        ActionButtonInfo.Create(UIParameters.SkillCheck.LootHelp[i])
                            .SetText(LocalizationManager.Get("SkillHelp", UIParameters.SkillCheck.LootHelp[i], helpParameters))
                            .SetCallData(UIParameters.SkillCheck.LootHelp[i])
                            .SetType(ActionType.Event));
                    this.choices[count].Visible = true;
                }
                else
                    this.choices[count].Visible = true;

                count += 1;
            }
        }

        this.ChanceText.Text = LocalizationManager.Get("SkillCheckResult", 0);

        foreach(var item in this.successes)
        {
            item.Visible = false;
        }

        foreach (var item in this.nessarys)
        {
            item.Visible = false;
        }

        for (int i = 0; i < this.nessarys.Count; i++)
        {
            if (i < UIParameters.SkillCheck.SuccessCheck.Amount)
            {
                this.nessarys[i].SetFill(1.0f);
                this.nessarys[i].SetIcon(SkillObject.SkillIcon("SuccessIcon"));
                this.nessarys[i].Visible = true;
            }
            else
                this.nessarys[i].Visible = false;
        }

        /*int nesResult = 0;
        int startCount = 0;
        int skillAmount = 0;
        int remainder;
        foreach (var skill in UIParameters.SkillCheck.SuccessCheck)
        {
            foreach (var hero in GM.Player.Group.GetUnits())
            {
                if (!hero.skills.ContainsKey(skill.Skill))
                    continue;

                nesResult += hero.skills[skill.Skill].Max;
            }

            skillAmount = Math.DivRem(nesResult, skill.Complex, out remainder);


            for (int i = startCount; i < skillAmount + startCount; i++)
            {
                if (i < this.successes.Count)
                {
                    this.successes[i].SetFill(1.0f);
                    this.successes[i].SetIcon(SkillObject.SkillIcon(skill.Skill));
                    this.successes[i].Visible = true;
                }
            }

            startCount += skillAmount;

            if(remainder > 0 && startCount < this.successes.Count)
            {
                this.successes[startCount].SetFill((float)remainder / skill.Complex);
                this.successes[startCount].SetIcon(SkillObject.SkillIcon(skill.Skill));
                this.successes[startCount].Visible = true;
                startCount += 1;
            }

            this.ChanceText.Visible = true;
            this.ChanceText.Text = LocalizationManager.Get("ResultSkillCheck", startCount);
        }

        if (startCount < this.successes.Count)
            for (int i = startCount; i < this.successes.Count; i++)
                this.successes[i].Visible = false;*/

        base.Open();
    }

    public override void PressedItem(UIItem data)
    {
        if (!data.ItemTag.Equals("ActionChoice"))
            return;

        ActionChoice item = data as ActionChoice;

        if (item.date.ID.Equals("Start"))
        {
            StartCoroutine("MakeResult");
            return;
        }
  
    }

    IEnumerator MakeResult()
    {
        foreach(var image in this.successes)
        {
            image.SetFill(0.0f);
            image.Visible = false;
        }

        yield return null;

        /*int startDice = 0;
        for (int i = 0; i < UIParameters.SkillCheck.SuccessCheck.Count; i++)
        {
            UIParameters.SkillCheck.SuccessCheck[i].CompleteCheck(GM.Player.Group.GetUnits());
            this.ChanceText.Text = LocalizationManager.Get("ResultSkillCheck", UIParameters.SkillCheck.SuccessCheck[i].ResultaAmount);

            yield return new WaitForSeconds(1.0f);

            yield return StartCoroutine(FullfillResult(UIParameters.SkillCheck.SuccessCheck[i].Complex, UIParameters.SkillCheck.SuccessCheck[i].ComplexResult, startDice));

            if(UIParameters.SkillCheck.SuccessCheck[i].CompleteDice)
            {
                int dice = UIParameters.SkillCheck.SuccessCheck[i].ResultaAmount - 1;
                if (this.successes.Count < dice)
                    this.successes[dice].SetFill(1.0f);
            }

            startDice = UIParameters.SkillCheck.SuccessCheck[i].ResultaAmount;
        }*/

        CompleteCheck();
    }

    IEnumerator FullfillResult(int complex, int amount,  int startDice)
    {
        float stepFill = 1.0f / complex;
        float step = 0.5f / complex;
        float timer = 0.0f;
        int flip = 0;
        int resAm = amount;

        this.successes[startDice].Visible = true;
        for (int i = 0; i < amount; i ++)
        {
            timer = 0.0f;
            while (timer < step)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            flip += 1;
            resAm -= 1;
            this.ChanceText.Text = LocalizationManager.Get("ResultSkillCheck", resAm);
            if (flip >= complex)
            {
                this.successes[startDice].SetFill(1.0f);
                flip = 0;
                startDice += 1;
                this.successes[startDice].Visible = true;
            }
            else
                this.successes[startDice].AddFill(stepFill);
        }
    }

    void CompleteCheck()
    {
        bool success = true;

        /*for(int i = 0; i < UIParameters.SkillCheck.SuccessCheck.Count; i++)
        {
            if(UIParameters.SkillCheck.SuccessCheck[i].CheckResult == false)
            {
                success = false;
                break;
            }
        }*/

        UIParameters.SkillCheck.result.CallResult(UIParameters.SkillCheck._parent, success);

        this.Close();
    }
}