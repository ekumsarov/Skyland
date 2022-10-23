using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : UIItem
{
    public UIImage skillIcon;
    public SimpleText skillValue;

    public override void Setting()
    {
        base.Setting();

        this.skillIcon.Visible = true;
        this.skillValue.Visible = true;
    }

    public void Setup(SkillObject skill)
    {
        string tooltiptext = LocalizationManager.Get("AbilitieLabel").ToUpper() + ": " + SkillObject.SkillFullName(skill.ID).ToUpper();

        this.TooltipText = tooltiptext;
        this.skillValue.Text = skill.GetSkillString();
    }
}