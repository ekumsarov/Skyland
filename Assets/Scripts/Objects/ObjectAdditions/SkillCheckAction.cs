using System.Collections;
using System.Collections.Generic;
using System;
using Lodkod;
using UnityEngine;
using EventPackSystem;

public class SkillCheckAction : Actions
{
    string ObjectID = "self";

    string FailText = string.Empty;

    List<string> LootHelp;

    GroupSkillCheck SuccessCheck;
    List<SkillCheckObject> BadCheck;

    ResultID result = null;

    public SkillCheckAction()
    {
        list = new List<ActionButtonInfo>();
        type = ActionsSet.SkillCheck;
        OnClear = true;
        restore = true;
        Text = "";

        BadCheck = new List<SkillCheckObject>();
    }

    // Full instance of SkillcheckAction
    public static SkillCheckAction Make(string text, string actID, string fText, ResultID res,
        List<SkillCheckObject> successCheck,
        List<string> lootHelp = null,
        string objectID = "self",
        List<SkillCheckObject> badCheck = null)
    {
        return new SkillCheckAction()
        {
            ID = actID,
            type = ActionsSet.SkillCheck,
            Text = LocalizationManager.Get(text),
            FailText = LocalizationManager.Get(fText),
            LootHelp = lootHelp,
            ObjectID = objectID,
            SuccessCheck = GroupSkillCheck.Create(successCheck),
            BadCheck = badCheck,
            result = res
        };
    }

    public GroupSkillCheck GetCheck()
    {
        return this.SuccessCheck;
    }

    public override void CallAction(SkyObject parent)
    {
        UIParameters.SetSkillCheck(this.Text, this.FailText, result, this.SuccessCheck, this.Parent,
            this.LootHelp,
            ObjectID,
            this.BadCheck);

        UIM.OpenMenu("DialogueMenu");
    }
}