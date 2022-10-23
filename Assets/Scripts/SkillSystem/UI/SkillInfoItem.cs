using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoItem : UIItem
{
    public SimpleText skillName;
    public SimpleText skillValue;

    public override void Setting()
    {
        base.Setting();

        this.skillName.Visible = true;
        this.skillValue.Visible = true;
    }

    public void Setup(SkillObject skill)
    {
        this.skillName.Text = LocalizationManager.Get(skill.Skill + "SRT");
        this.skillValue.Text = skill.GetSkillString();
    }
}
