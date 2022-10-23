using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestSystem
{
    public List<MainQuest> _mainQuests;
    public List<QuestNode> _questNodes;

    public List<MainQuest> _completedQuests;
    public List<QuestNode> _completedNodes;

    public List<MainQuest> _failedQuests;
    public List<QuestNode> _failedNodes;

    public QuestSystem ()
    {
        this._mainQuests = new List<MainQuest>();
        this._questNodes = new List<QuestNode>();
        this._completedQuests = new List<MainQuest>();
        this._completedNodes = new List<QuestNode>();
        this._failedQuests = new List<MainQuest>();
        this._failedNodes = new List<QuestNode>();
    }

    public void AddQuest(MainQuest quest)
    {
        if (this._mainQuests.Any(que => que.ID.Equals(quest.ID)))
        {
            Debug.LogError("Already has main quest: " + quest.ID);
            return;
        }

        UIM.QuestNotification.NewQuest(quest);

        this._mainQuests.Add(quest);
        for(int i = 0; i < quest._nodes.Count; i++)
        {
            this.AddQuest(quest._nodes[i]);
        }
    }

    public void AddQuest(QuestNode quest)
    {
        if (this._questNodes.Any(que => que.ID.Equals(quest.ID)))
        {
            Debug.LogError("Already has quest node: " + quest.ID);
            return;
        }

        this._questNodes.Add(quest);

        if(quest.Visible)
            UIM.QuestNotification.NewQuest(quest);
    }

    public void FailQuest(string questID)
    {
        MainQuest mQuest = this._mainQuests.FirstOrDefault(que => que.ID.Equals(questID));
        if(mQuest != null)
        {
            UIM.QuestNotification.FailQuest(mQuest);
            this._failedQuests.Add(mQuest);

            for(int i = 0; i < mQuest._nodes.Count; i++)
            {
                this._failedNodes.Add(mQuest._nodes[i]);
                ES.NotifySubscribers("FailQuest", mQuest._nodes[i].ID);
                this._questNodes.Remove(mQuest._nodes[i]);
                UIM.QuestNotification.FailQuest(mQuest._nodes[i]);
            }

            ES.NotifySubscribers("CompleteQuest", mQuest.ID);
            this._mainQuests.Remove(mQuest);

            return;
        }

        QuestNode nQuest = this._questNodes.FirstOrDefault(que => que.ID.Equals(questID));
        if(nQuest != null)
        {
            ES.NotifySubscribers("CompleteQuest", nQuest.ID);
            UIM.QuestNotification.FailQuest(nQuest);
            this._failedNodes.Add(nQuest);
            this._questNodes.Remove(nQuest);
        }
    }

    public void CompleteQuest(string questID)
    {
        MainQuest mQuest = this._mainQuests.FirstOrDefault(que => que.ID.Equals(questID));
        if (mQuest != null)
        {
            UIM.QuestNotification.CompleteQuest(mQuest);
            this._completedQuests.Add(mQuest);

            for (int i = 0; i < mQuest._nodes.Count; i++)
            {
                this._completedNodes.Add(mQuest._nodes[i]);
                ES.NotifySubscribers("CompleteQuest", mQuest._nodes[i].ID);
                this._questNodes.Remove(mQuest._nodes[i]);
                UIM.QuestNotification.CompleteQuest(mQuest._nodes[i]);
            }

            ES.NotifySubscribers("CompleteQuest", mQuest.ID);

            this._mainQuests.Remove(mQuest);

            return;
        }

        QuestNode nQuest = this._questNodes.FirstOrDefault(que => que.ID.Equals(questID));
        if (nQuest != null)
        {
            nQuest.Complete = true;
            this._completedNodes.Add(nQuest);
            this._questNodes.Remove(nQuest);
            ES.NotifySubscribers("CompleteQuest", nQuest.ID);
        }

        for(int i = 0; i < this._questNodes.Count; i++)
        {
            if (this._questNodes[i].BindNode.Equals(nQuest.ID))
            {
                this._questNodes[i].Visible = true;
                UIM.QuestNotification.NewQuest(this._questNodes[i]);
            }
        }
    }

    public void RemoveQuest(string questID)
    {
        MainQuest mQuest = this._mainQuests.FirstOrDefault(que => que.ID.Equals(questID));
        if (mQuest != null)
        {
            for (int i = 0; i < mQuest._nodes.Count; i++)
            {
                this._questNodes.Remove(mQuest._nodes[i]);
            }

            this._mainQuests.Remove(mQuest);

            return;
        }

        QuestNode nQuest = this._questNodes.FirstOrDefault(que => que.ID.Equals(questID));
        if (nQuest != null)
        {
            this._questNodes.Remove(nQuest);
        }
    }

    public bool IsComplete(string questID)
    {
        return this._completedQuests.Any(quest => quest.ID.Equals(questID)) || this._completedNodes.Any(node => node.ID.Equals(questID));
    }

    public bool HasQuest(string questID)
    {
        return this._completedQuests.Any(quest => quest.ID.Equals(questID));
    }
}

public class QS
{
    private static QuestSystem _questSytem;
    public static void NewGame()
    {
        if (QS._questSytem != null)
            QS._questSytem = null;

        QS._questSytem = new QuestSystem();
    }

    public static void AddQuest(MainQuest quest)
    {
        QS._questSytem.AddQuest(quest);
    }

    public static void AddQuest(QuestNode node)
    {
        QS._questSytem.AddQuest(node);
    }

    public static void FailQuest(string questID)
    {
        QS._questSytem.FailQuest(questID);
    }

    public static void CompleteQuest(string questID)
    {
        QS._questSytem.CompleteQuest(questID);
    }

    public static bool IsComplete(string questID)
    {
        return QS._questSytem.IsComplete(questID);
    }
    public static bool HasQuest(string questID)
    {
        return QS._questSytem.HasQuest(questID);
    }

    public static void RemoveQuest(string questID)
    {
        QS._questSytem.RemoveQuest(questID);
    }

    public static List<MainQuest> GetAllMainQuests()
    {
        return QS._questSytem._mainQuests;
    }

    public static List<QuestNode> GetAllQuestNodes()
    {
        return QS._questSytem._questNodes;
    }
}
