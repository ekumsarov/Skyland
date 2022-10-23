using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : UIItem
{
    public UIImage icon;
    public UIImage completeMark;
    [SerializeField] SimpleText titleText;

    private bool ShowMark;
    private bool CloseOnComplete;
    public string text;
    public string QuestID;

    public override void Setting()
    {
        base.Setting();
        this.AddSubscriber("QuestCompleted");
    }

    public void SetupItem(MainQuest quest)
    {
        this.titleText.Visible = true;
        this.AddListeningObject("QuestCompleted", quest.ID);
        this.completeMark.Visible = false;

        if (!quest.Title.IsNullOrEmpty())
        {
            this.text = LocalizationManager.Get(quest.Title) + "\n\n";
            this.titleText.Text = LocalizationManager.Get(quest.Title);
        }
        else
            this.titleText.Text = LocalizationManager.Get(quest.ID);
            

        
        this.icon.Image = quest.MainQuestIcon;
        this.text += LocalizationManager.Get(quest.Description);
        this.QuestID = quest.ID;
        this.Visible = true;
        this.ShowMark = false;
        this.CloseOnComplete = true;
    }

    public void SetupItem(QuestNode quest)
    {
        this.titleText.Visible = true;
        this.AddListeningObject("QuestCompleted", quest.ID);
        this.completeMark.Visible = false;

        if (!quest.Title.IsNullOrEmpty())
        {
            this.text = LocalizationManager.Get(quest.Title) + "\n\n";
            this.titleText.Text = LocalizationManager.Get(quest.Title);
        }
        else
            this.titleText.Text = LocalizationManager.Get(quest.ID);

        this.icon.Image = quest.QuestIcon;
        this.text += LocalizationManager.Get(quest.Description);
        this.QuestID = quest.ID;
        this.Visible = true;
        this.ShowMark = quest.ShowCompleteMark;
        this.CloseOnComplete = quest.HideOnComplete;
    }

    public void QuestCompleted()
    {
        this.ClearListeningObject("QuestCompleted");

        this.completeMark.Visible = this.ShowMark;
        this.Visible = !this.CloseOnComplete;
    }
}