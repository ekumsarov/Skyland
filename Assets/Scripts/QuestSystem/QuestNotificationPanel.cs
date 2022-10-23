using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNotificationPanel : PanelEx
{
    public UIImage icon;
    public UIIconText text;

    List<QuestSetups> _questsToShow;

    bool _isShowing;

    UIAnimation fAnim;
    UIAnimation sAnim;

    public override void Setting()
    {
        base.Setting();

        this._questsToShow = new List<QuestSetups>();

        this.Resize = UIResize.Fixed;
        this._fitscreen = false;

        Vector2 startPostition = this.rect.anchoredPosition;
        Vector2 endPosition = new Vector2(this.rect.anchoredPosition.x + 420f, this.rect.anchoredPosition.y);
        
        fAnim = UIM.Move(this, startPostition, endPosition, 2f);
        sAnim = UIM.Move(this, endPosition, startPostition, 0.6f).SetDelay(1.5f).SetCallback(CompleteShow);
        this.SetAnimation(new List<UIAnimation>() {
            fAnim,
            sAnim
        }, "SlideQuest");
    }           
    
    public void SetupItem(MainQuest quest, string TopTitle)
    {

        string label;

        if (!quest.Title.IsNullOrEmpty())
            label = LocalizationManager.Get(quest.Title);
        else
            label = LocalizationManager.Get(quest.ID);

        label += "\n\n" + TopTitle;

        this._questsToShow.Add(new QuestSetups()
        {
            text = label,
            iconID = quest.MainQuestIcon
        });

        if(!this._isShowing)
        {
            this._isShowing = true;
            this.icon.Image = quest.MainQuestIcon;
            this.text.IconText.Text(label);
            this.text.IconText.ShowComplete();
            this.PlayAnimation("SlideQuest");
        }
    }

    public void SetupItem(QuestNode quest, string TopTitle)
    {
        string label;

        if (!quest.Title.IsNullOrEmpty())
            label = LocalizationManager.Get(quest.Title);
        else
            label = LocalizationManager.Get(quest.ID);

        label += "\n\n" + TopTitle;

        this._questsToShow.Add(new QuestSetups()
        {
            text = label,
            iconID = quest.QuestIcon
        });

        if (!this._isShowing)
        {
            this._isShowing = true;
            this.icon.Image = quest.QuestIcon;
            this.text.IconText.Text(label);
            this.text.IconText.ShowComplete();
            this.PlayAnimation("SlideQuest");
        }
    }

    public void CompleteShow()
    {
        this._questsToShow.RemoveAt(0);
        if (this._questsToShow.Count>0)
        {
            this.icon.Image = this._questsToShow[0].iconID;
            this.text.IconText.Text(this._questsToShow[0].text);
            this.text.IconText.ShowComplete();
            this.PlayAnimation("SlideQuest");
        }
        else
        {
            this._isShowing = false;
        }
    }

    public class QuestSetups
    {
        public string iconID;
        public string text;
    }
}