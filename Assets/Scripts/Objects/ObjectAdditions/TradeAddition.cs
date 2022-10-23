using System.Collections;
using System.Collections.Generic;
using Lodkod;
using UnityEngine;

public enum TradeObjectType
{
    Wood,
    Stone,
    Food,
    Skystone,
    Warrior,
    Archer,
    Custom
}

public class TradeAction : Actions
{
    public string TradeText;
    public string WaitTradeText;
    public bool active;

    List<TradeObject> _tradeList;
    List<TradeObject> _tradeCost;

    List<TradeObject> _buyList;
    List<TradeObject> _buyCost;

    public TradeAction self;

    public TradeAction(string text, string waitText = "")
    {
        this.ID = "Trade";
        _tradeList = new List<TradeObject>();
        _tradeCost = new List<TradeObject>();
        _buyList = new List<TradeObject>();
        _buyCost = new List<TradeObject>();
        active = true;

        TradeText = text;

        if (waitText == "")
            waitText = "AIWaitTradeSettler1";
    }

    public List<TradeObject> TradeList
    {
        get { return _tradeList; }
        set { _tradeList = value; }
    }
    public List<TradeObject> TradeCost
    {
        get { return _tradeCost; }
        set { _tradeCost = value; }
    }
    public List<TradeObject> BuyList
    {
        get { return _buyList; }
        set { _buyList = value; }
    }
    public List<TradeObject> BuyCost
    {
        get { return _buyCost; }
        set { _buyCost = value; }
    }

    public void AddTrade(TradeObjectType type,  int amount, float cooldown, TradeObjectType typeCost, int amountCost, string IconText = "wood", string IconTextCost = "wood")
    {
        TradeObject obj = new TradeObject();
        obj.Type = type;
        obj.amount = amount;
        obj.Cooldown = cooldown;
        obj.iconText = TradeObjectTypeToIcon(type);
        TradeList.Add(obj);

        obj = new TradeObject();
        obj.Type = typeCost;
        obj.amount = amountCost;
        obj.iconText = TradeObjectTypeToIcon(typeCost);
        TradeCost.Add(obj);
    }

    public void AddBuy(TradeObjectType type, int amount, float cooldown, TradeObjectType typeCost, int amountCost, string IconText = "wood", string IconTextCost = "wood")
    {
        TradeObject obj = new TradeObject();
        obj.Type = type;
        obj.amount = amount;
        obj.Cooldown = cooldown;
        obj.iconText = TradeObjectTypeToIcon(type);
        BuyList.Add(obj);

        obj = new TradeObject();
        obj.Type = typeCost;
        obj.amount = amountCost;
        obj.iconText = TradeObjectTypeToIcon(typeCost);
        BuyCost.Add(obj);
    }

    public override void CallAction(SkyObject parent)
    {
        List<ActionButtonInfo> list = new List<ActionButtonInfo>();

        int t_count = 0;
        foreach (var tr in TradeList)
        {
            if (tr.disposable && tr.dispodableAmount > 0)
            {
                t_count += 1;
                break;
            }
            else if (tr.Timer <= 0)
            {
                t_count += 1;
                break;
            }
        }

        foreach (var tr in BuyList)
        {
            if (tr.disposable && tr.dispodableAmount > 0)
            {
                t_count += 1;
                break;
            }
            else if (tr.Timer <= 0)
            {
                t_count += 1;
                break;
            }
        }

        /*if (t_count == 0)
        {
            ActionButtonInfo wmain = new ActionButtonInfo();
            wmain.Text = LocalizationManager.Get(WaitTradeText);
            list.Add(wmain);

            ActionButtonInfo button = new ActionButtonInfo();
            button.Text = LocalizationManager.Get("AIWaitTradeSettlerButton1");
            button.CallID = "Greeting";
            button.Type = ActionType.Action;
            list.Add(button);

            UIParameters.NullAction();
            UIParameters.SetAction(list, parent, text: Text);
            UIM.OpenMenu("ContextMenu");
            return;
        }


        string addingIcons = "";
        for (int i = 0; i < TradeList.Count; i++)
        {
            if (i == 0)
                addingIcons += TradeList[i].iconText;
            else
                addingIcons += ", " + TradeList[i].iconText;
        }

        string addingIconsBuy = "";
        for (int i = 0; i < BuyCost.Count; i++)
        {
            if (i == 0)
                addingIconsBuy += BuyCost[i].iconText;
            else
                addingIconsBuy += ", " + BuyCost[i].iconText;
        }

        ActionButtonInfo tmain = new ActionButtonInfo();
        tmain.Text = LocalizationManager.Get(TradeText, addingIcons, addingIconsBuy);
        list.Add(tmain);

        int buttonNumber = 0;

        for (int i = 0; i < TradeList.Count; i++)
        {
            if (TradeList[i].Timer > 0)
                continue;

            ActionButtonInfo button = new ActionButtonInfo();
            button.Text = LocalizationManager.Get("PlayerTradeAiButton" + UnityEngine.Random.Range(1,3)) + " <color=green>" + TradeList[i].amount + TradeList[i].iconText + "</color> - " + "<color=red>" + TradeCost[i].amount + "</color>" + TradeCost[i].iconText;
            button.CallID = "Trade";
            button.SelectID = buttonNumber;
            button.Type = ActionType.Special;
            list.Add(button);
            buttonNumber += 1;
        }

        for (int i = 0; i < BuyList.Count; i++)
        {
            if (BuyList[i].Timer > 0)
                continue;

            ActionButtonInfo button = new ActionButtonInfo();
            button.Text = LocalizationManager.Get("PlayerTradeAiButton" + UnityEngine.Random.Range(1, 3)) + " <color=green>" + BuyList[i].amount + BuyList[i].iconText + "</color> - " + "<color=red>" + BuyCost[i].amount + "</color>" + BuyCost[i].iconText;
            button.CallID = "Trade";
            button.SelectID = buttonNumber;
            button.Type = ActionType.Special;
            list.Add(button);
            buttonNumber += 1;
        }*/

        UIParameters.NullAction();
        UIParameters.SetAction(list, parent, text: Text);
        UIM.OpenMenu("ContextMenu");
    }

    public void trade(int number)
    {
        TradeObject trades = null;
        TradeObject sells = null;

        if (number < TradeList.Count)
        {
            trades = TradeList[number];
            sells = TradeCost[number];
        }
        else
        {
            number -= TradeList.Count;
            trades = BuyList[number];
            sells = BuyCost[number];
        }

        if(isResType(trades.Type))
        {
            if (!iStat.CheckAdd(getResType(trades.Type).ToString(), trades.amount))
            {
                this.Parent.CallAction("Trade");
                return;
            }
        }

        if (isResType(sells.Type))
        {
            if (!iStat.CheckResource(getResType(sells.Type).ToString(), sells.amount))
            {
                this.Parent.CallAction("Trade");
                return;
            }
        }

        if (isResType(trades.Type))
        {
            SM.Stats[getResType(trades.Type).ToString()].Count += trades.amount;
        }
        if (isResType(sells.Type))
        {
            SM.Stats[getResType(sells.Type).ToString()].Count -= sells.amount;
        }

        this.Parent.CallAction("Trade");
    }

    string TradeObjectTypeToString(TradeObjectType type)
    {
        if (type == TradeObjectType.Stone)
            return "stone";
        else if (type == TradeObjectType.Food)
            return "apple";
        else if (type == TradeObjectType.Skystone)
            return "grav_ore";

        return "log";
    }
    string TradeObjectTypeToIcon(TradeObjectType type)
    {
        if (type == TradeObjectType.Stone)
            return "<icon=stone/>";
        else if (type == TradeObjectType.Food)
            return "<icon=apple/>";
        else if (type == TradeObjectType.Skystone)
            return "<icon=grav_ore/>";

        return "<icon=log/>";
    }
    bool isResType(TradeObjectType type)
    {
        if (type == TradeObjectType.Stone || type == TradeObjectType.Food || type == TradeObjectType.Wood || type == TradeObjectType.Skystone)
            return true;
        return false;
    }
    StatType getResType(TradeObjectType type)
    {
        if (type == TradeObjectType.Stone)
            return StatType.Stone;
        else if (type == TradeObjectType.Food)
            return StatType.Food;
        else if (type == TradeObjectType.Skystone)
            return StatType.Skystone;
        else if (type == TradeObjectType.Wood)
            return StatType.Wood;

        Debug.LogError("Error res type");
        return StatType.Wood;
    }
}

public class TradeObject
{
    public TradeObjectType Type;
    public string iconText;

    public int amount;
    public int dispodableAmount;

    public float Cooldown;
    public float Timer;
    public bool disposable;
}
