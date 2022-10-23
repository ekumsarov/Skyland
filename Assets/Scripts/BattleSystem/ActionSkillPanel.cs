using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleActions;
using System;

public class ActionSkillPanel : UIItem
{
    public UIImage Icon;
    public SimpleText Description;
    public SimpleText ActName;

    public List<FilledItem> successes;
    public SimpleText ChanceText;

    public List<FilledItem> defends;
    public SimpleText NessaryText;

    BattleAction regAct;

    public override void Setting()
    {
        base.Setting();

        for(int i = 0; i < this.successes.Count; i++)
        {
            this.successes[i].Visible = false;
            this.defends[i].Visible = false;
        }
    }

    public void RegisterAction(BattleAction act)
    {
        for(int i = 0; i < 12; i++)
        {
            this.successes[i].Visible = false;
            this.defends[i].Visible = false;
        }

//        this.ChanceText.Text = LocalizationManager.Get("ResultSkillCheck", 0);
//        this.NessaryText.Text = LocalizationManager.Get("ResultSkillCheck", 0);
        this.ChanceText.Visible = false;
        this.NessaryText.Visible = false;

        this.regAct = act;

        this.Icon.Image = this.regAct.Icon;
        this.ActName.Text = LocalizationManager.Get(this.regAct.ActName);
        this.Description.Text = this.regAct.GetDescription();

        this.Description.Visible = true;
        this.ActName.Visible = true;

        this.Visible = true;
    }

    public void ShowDetails()
    {

    }

    public void StartCheck(int complexAmount, int complex, bool complexComplete, int side, Action callb, string addText, string icon = "BaseIcon")
    {
        this.ChanceText.Visible = true;
        this.ChanceText.Text = addText;
        int sd = 0;
        if(side == 0)
        {
            for (int i = 0; i < this.successes.Count; i++)
            {
                if (this.successes[i].Visible == false)
                {
                    sd = i;
                    break;
                }
            }
        }    
        else
        {
            for (int i = 0; i < this.defends.Count; i++)
            {
                if (this.defends[i].Visible == false)
                {
                    sd = i;
                    break;
                }
            }
        }

        StartCoroutine(AttackCheck(complexAmount, complex, complexComplete, sd, side, callb, icon));
    }

    IEnumerator AttackCheck(int amount, int complex, bool complexComplete, int startDice, int side, Action callb, string icon)
    {
        float stepFill = 1.0f / complex;
        float step = 0.5f / complex;
        float timer = 0.0f;
        int flip = 0;
        int resAm = amount;

        if(side == 0)
        {
            this.successes[startDice].SetIcon(icon);
            this.successes[startDice].SetFill(0f);
            this.successes[startDice].Visible = true;
        }
        else
        {
            this.defends[startDice].SetIcon(icon);
            this.defends[startDice].SetFill(0f);
            this.defends[startDice].Visible = true;
        }
        
        for (int i = 0; i < amount; i++)
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
            if(side == 0)
            {
                if (flip >= complex)
                {
                    this.successes[startDice].SetFill(1.0f);
                    flip = 0;
                    startDice += 1;
                    this.successes[startDice].SetIcon(icon);
                    this.successes[startDice].SetFill(0f);
                    this.successes[startDice].Visible = true;
                }
                else
                    this.successes[startDice].AddFill(stepFill);
            }
            else
            {
                if (flip >= complex)
                {
                    this.defends[startDice].SetFill(1.0f);
                    flip = 0;
                    startDice += 1;
                    this.defends[startDice].SetIcon(icon);
                    this.defends[startDice].SetFill(0f);
                    this.defends[startDice].Visible = true;
                }
                else
                    this.defends[startDice].AddFill(stepFill);
            }
        }

        if(side == 0)
        {
            if (complexComplete)
                this.successes[startDice].SetFill(1);
            else
                this.successes[startDice].Visible = false;
        }
        else
        {
            if (complexComplete)
                this.defends[startDice].SetFill(1);
            else
                this.defends[startDice].Visible = false;
        }

        callb?.Invoke();
    }
}
