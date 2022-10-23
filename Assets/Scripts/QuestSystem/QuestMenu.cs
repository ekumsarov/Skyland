using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;
using System;
using Lodkod;

public class QuestMenu : MenuEx
{
    [SerializeField] List<QuestItem> _mainItems;
    [SerializeField] UIIconText _description;

    public override void Setting()
    {
        base.Setting();

        //this._mainItems = new List<QuestItem>();

        
        //this._mainItems.AddRange(this._allItems.Values.Cast<QuestItem>().Where(que => que.ItemTag.Equals("NodeQuest")).ToList());
    }

    public override void Open()
    {
        List<MainQuest> quests = QS.GetAllMainQuests();
        int startindex = 0;
        for(int i = 0; i < this._mainItems.Count; i++)
        {
            if (i < quests.Count)
            {
                this._mainItems[i].SetupItem(quests[i]);
                this._mainItems[i].Visible = true;
                startindex = i + 1; 
            }
            else
                this._mainItems[i].Visible = false;
        }

        List<QuestNode> questNodes = QS.GetAllQuestNodes();
        for (int i = 0; i < questNodes.Count; i++)
        {
            if (startindex < this._mainItems.Count)
            {
                this._mainItems[startindex].SetupItem(questNodes[i]);
                this._mainItems[startindex].Visible = true;
                startindex += 1;
            }
            else
                break;
        }

        if(this._mainItems[0].Visible)
        {
            this._description.IconText.Text(this._mainItems[0].text);
            this._description.IconText.ShowComplete();
            this._mainItems[0].Selected(true);
        }

        base.Open();
    }

    public override void PressedItem(UIItem data)
    {
        if(data.ItemTag.Equals("Close"))
        {
            this.Close();
        }
        else if(data.ItemTag.Equals("NodeQuest"))
        {
            QuestItem item = data as QuestItem;
            if(item != null)
            {
                this._description.IconText.Text(item.text);
                this._description.IconText.ShowComplete();
            }
        }
    }
}