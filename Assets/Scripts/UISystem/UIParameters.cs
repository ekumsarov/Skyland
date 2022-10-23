using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;
using System;
using Lodkod;

public class UIParameters
{
    private ActionParameter _action;
    private DialogueParameter _dialogue;
    private SkillCheckParameter _skillCheck;
    private RewardParameter _reward;
    private MainBuild _build;

    private static UIParameters instance = null;
    public static void NewGame()
    {
        if (UIParameters.instance != null)
            UIParameters.instance = null;

        UIParameters.instance = new UIParameters();

        UIParameters.instance._action = new ActionParameter();
        UIParameters.instance._action.list = new List<ActionButtonInfo>();
        UIParameters.instance._action._parent = null;

        UIParameters.instance._dialogue = new DialogueParameter();
        UIParameters.instance._skillCheck = new SkillCheckParameter();
        UIParameters.instance._reward = new RewardParameter();
        UIParameters.instance._reward.list = new List<RewardItemInfo>();
    }

    public static MainBuild ActiveBuild
    {
        get { return UIParameters.instance._build; }
        set
        {
            UIParameters.instance._build = value;
        }
    }

    #region Reward
    public static RewardParameter Reward
    {
        get { return UIParameters.instance._reward; }
    }
    public static void SetReward(List<RewardItemInfo> lis)
    {
        UIParameters.instance._reward.list = lis;
    }
    public static void NullReward()
    {
        UIParameters.instance._action.list.Clear();
    }
    #endregion

    #region Action
    public static ActionParameter Action
    {
        get { return UIParameters.instance._action; }
    }
    public static void SetAction(List<ActionButtonInfo> lis, SkyObject par, SkyObject ActionPosition = null, string Paper = null, string text = null)
    {
        UIParameters.instance._action.Paper = Paper;
        UIParameters.instance._action.text = text;
        UIParameters.instance._action.list.Clear();
        UIParameters.instance._action.list.AddRange(lis);
        UIParameters.instance._action._parent = par;
        UIParameters.instance._action.ActionPosition = ActionPosition;
    }
    public static void NullAction()
    {
        UIParameters.instance._action.Paper = null;
        UIParameters.instance._action.text = null;
        UIParameters.instance._action.list.Clear();
        UIParameters.instance._action._parent = null;
        UIParameters.instance._action.ActionPosition = null;
    }
    #endregion

    #region SkillCheck
    public static SkillCheckParameter SkillCheck
    {
        get { return UIParameters.instance._skillCheck; }
    }
    public static void SetSkillCheck(string text,  string fText, ResultID result,
        GroupSkillCheck successCheck, 
        SkyObject par,
        List<string> lootHelp = null,
        string objectID = "self",
        List<SkillCheckObject> badCheck = null)
    {
        UIParameters.instance._skillCheck.text = text;
        UIParameters.instance._skillCheck._parent = par;
        UIParameters.instance._skillCheck.FailText = fText;
        UIParameters.instance._skillCheck.LootHelp = lootHelp;
        UIParameters.instance._skillCheck.SuccessCheck = successCheck;
        UIParameters.instance._skillCheck.BasCheck = badCheck;
        UIParameters.instance._skillCheck.result = result;
    }
    public static void NullSkillCheck()
    {
        UIParameters.instance._skillCheck.text = "MissionPangramm";
        UIParameters.instance._skillCheck._parent = null;
        UIParameters.instance._skillCheck.SuccessCheck = null;
        UIParameters.instance._skillCheck.SuccessCheck = null;
        UIParameters.instance._skillCheck.BasCheck.Clear();
        UIParameters.instance._skillCheck.BasCheck = null;
        UIParameters.instance._skillCheck.result = null;
    }
    #endregion

    #region Dialogue
    public static DialogueParameter Dialogue
    {
        get { return UIParameters.instance._dialogue; }
    }
    public static void SetDialogue(string Ico = null, string text = null, GameEvents.GameEvent ev = null)
    {
        UIParameters.instance._dialogue.Ico = Ico;
        UIParameters.instance._dialogue.Text = text;
        UIParameters.instance._dialogue.Event = ev;
    }
    public static void NullDialogue()
    {
        UIParameters.instance._dialogue.Ico = null;
        UIParameters.instance._dialogue.Text = null;
        UIParameters.instance._dialogue.Event = null;
    }
    #endregion
}

public class DialogueParameter
{
    public string Ico;
    public string Text;
    public GameEvents.GameEvent Event;
}

public class ActionParameter
{
    public string Paper;
    public string text;
    public List<ActionButtonInfo> list;
    public SkyObject _parent;
    public SkyObject ActionPosition;
}

public class RewardParameter
{
    public List<RewardItemInfo> list;
}

public class SkillCheckParameter
{
    public string text;
    public SkyObject _parent;

    public string FailText = string.Empty;

    public List<string> LootHelp;

    public GroupSkillCheck SuccessCheck;
    public List<SkillCheckObject> BasCheck;

    public ResultID result;
}

public class BattleParameter
{
    public string PlayerGoal;
    public WarStats PlayerStats;

    public string EnemyGaol;
    public WarStats EnemyStats;

    public List<string> additionalGoals;
    public SkyObject _parent;
}
