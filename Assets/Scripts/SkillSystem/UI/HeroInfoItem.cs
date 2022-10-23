using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using BattleEffects;
using BattleActions;

public class HeroInfoItem : UIItem
{
    public SimpleHealthBar bar;

    public UIImage icon;

    public SimpleText ActionPoints;

    /*public SkillInfoItem skill1;
    public SkillInfoItem skill2;
    public SkillInfoItem skill3;
    public SkillInfoItem skill4;
    public SkillInfoItem skill5;*/

    public UnitInfoItem group1;
    public UnitInfoItem group2;
    public UnitInfoItem group3;

    public HeroUnit bindUnit;

    public List<BattleAction> Actions;

    protected int _side = 0;
    public int Side
    {
        get { return this._side; }
    }

    public override void Setting()
    {
        base.Setting();

        UIAnimation anim = UIM.ColorAnimation(this.icon, this.icon.Color, Color.red, 0.5f);
        UIAnimation ranim = UIM.ColorAnimation(this.icon, Color.red, this.icon.Color, 0.5f, callback:UpdateItem);
        this.icon.SetAnimation(new List<UIAnimation>() { anim, ranim }, "Damage", true, true);

        UIAnimation hanim = UIM.ColorAnimation(this.icon, this.icon.Color, Color.green, 0.5f);
        UIAnimation hranim = UIM.ColorAnimation(this.icon, Color.green, this.icon.Color, 0.5f, callback: UpdateItem);
        this.icon.SetAnimation(new List<UIAnimation>() { hanim, hranim }, "Heal", true, true);

        Actions = new List<BattleAction>();
    }

    public virtual void SetupItem(HeroUnit uni)
    {
        this.bindUnit = uni;

        this.bar.UpdateBar(this.bindUnit.CurrentHP, this.bindUnit.HP);

        this.icon.Image = this.bindUnit.Icon;

        /*int index = 0;
        foreach(var skill in this.bindUnit.skills)
        {
            if (index == 0)
                this.skill1.Setup(skill.Value);
            else if (index == 1)
                this.skill2.Setup(skill.Value);
            else if (index == 2)
                this.skill3.Setup(skill.Value);
            else if (index == 3)
                this.skill4.Setup(skill.Value);
            else if (index == 4)
                this.skill5.Setup(skill.Value);
            else
                Debug.LogError("New skill: " + skill.Key);

            index++;
        }*/

        this.Actions.Clear();
        for(int i = 0; i < uni.actions.Count; i++)
        {
            this.Actions.Add(BattleAction.loadBattleAction(uni.actions[i], this));
        }
        

        for(int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                if (i < this.bindUnit.units.Count)
                {
                    this.group1.unitIcon.Image = this.bindUnit.units.ElementAt(i).Key.Icon;
                    this.group1.unitAmount.Text = this.bindUnit.units.ElementAt(i).ToString();
                }
                else
                    this.group1.Visible = false;
            }
            else if (i == 1)
            {
                if (i < this.bindUnit.units.Count)
                {
                    this.group2.unitIcon.Image = this.bindUnit.units.ElementAt(i).Key.Icon;
                    this.group2.unitAmount.Text = this.bindUnit.units.ElementAt(i).ToString();
                }
                else
                    this.group2.Visible = false;
            }
            else if (i == 2)
            {
                if (i < this.bindUnit.units.Count)
                {
                    this.group3.unitIcon.Image = this.bindUnit.units.ElementAt(i).Key.Icon;
                    this.group3.unitAmount.Text = this.bindUnit.units.ElementAt(i).ToString();
                }
                else
                    this.group3.Visible = false;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if(this.bindUnit.CurrentHP < amount)
        {
            this.Visible = false;
            this.bindUnit.CurrentHP = 0;
            return;
        }

        this.bindUnit.Damage(amount);
        if(this.bindUnit.CurrentHP < 1)
        {
            this.Visible = false;
            this.bindUnit.CurrentHP = 0;
            return;
        }

        this.icon.PlayAnimation("Damage");
    }

    public void HealUnit(int amount)
    {
        this.bindUnit.HealHero(amount);
        this.icon.PlayAnimation("Heal");
    }

    public void UpdateItem()
    {
        this.bar.UpdateBar(this.bindUnit.CurrentHP, this.bindUnit.HP);

        /*int index = 0;
        foreach (var skill in this.bindUnit.skills)
        {
            if (index == 0)
                this.skill1.Setup(skill.Value);
            else if (index == 1)
                this.skill2.Setup(skill.Value);
            else if (index == 2)
                this.skill3.Setup(skill.Value);
            else if (index == 3)
                this.skill4.Setup(skill.Value);
            else if (index == 4)
                this.skill5.Setup(skill.Value);
            else
                Debug.LogError("New skill: " + skill.Key);

            index++;
        }
        */
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                if (i < this.bindUnit.units.Count)
                {
                    this.group1.unitIcon.Image = this.bindUnit.units.ElementAt(i).Key.Icon;
                    this.group1.unitAmount.Text = this.bindUnit.units.ElementAt(i).ToString();
                }
                else
                    this.group1.Visible = false;
            }
            else if (i == 1)
            {
                if (i < this.bindUnit.units.Count)
                {
                    this.group2.unitIcon.Image = this.bindUnit.units.ElementAt(i).Key.Icon;
                    this.group2.unitAmount.Text = this.bindUnit.units.ElementAt(i).ToString();
                }
                else
                    this.group2.Visible = false;
            }
            else if (i == 2)
            {
                if (i < this.bindUnit.units.Count)
                {
                    this.group3.unitIcon.Image = this.bindUnit.units.ElementAt(i).Key.Icon;
                    this.group3.unitAmount.Text = this.bindUnit.units.ElementAt(i).ToString();
                }
                else
                    this.group3.Visible = false;
            }
        }
    }

    public void AddEffect(string effect)
    {
        if (this.bindUnit.effectsImmune != null && this.bindUnit.effectsImmune.Any(eff => eff.Equals(effect)))
            return;

        this.bindUnit.AddEffect(BattleEffect.loadBattleEffect(effect, this));
        this.UpdateItem();
    }
}
