using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;
using System;
using Lodkod;

public class QuestNotificationMenu : MenuEx
{
    [SerializeField] QuestNotificationPanel _panel;
    string newQuestLabel;
    string completeQuestLabel;
    string failQuestLabel;

    bool _isShowing = false;

    public override void Setting()
    {
        base.Setting();

        newQuestLabel = LocalizationManager.Get("NewQuestLabel");
        completeQuestLabel = LocalizationManager.Get("CompleteQuestLabel");
        failQuestLabel = LocalizationManager.Get("FailQuestLabel");
    }

    public void NewQuest(MainQuest quest)
    {
        this._panel.SetupItem(quest, newQuestLabel);
    }

    public void NewQuest(QuestNode quest)
    {
        this._panel.SetupItem(quest, newQuestLabel);
    }

    public void CompleteQuest(MainQuest quest)
    {
        this._panel.SetupItem(quest, completeQuestLabel);
    }

    public void CompleteQuest(QuestNode quest)
    {
        this._panel.SetupItem(quest, completeQuestLabel);
    }

    public void FailQuest(MainQuest quest)
    {
        this._panel.SetupItem(quest, failQuestLabel);
    }

    public void FailQuest(QuestNode quest)
    {
        this._panel.SetupItem(quest, failQuestLabel);
    }
}