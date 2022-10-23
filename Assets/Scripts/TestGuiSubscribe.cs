using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lodkod;

public class TestGuiSubscribe : MonoBehaviour {

    public Text DayPartText;

    string template = "День: {0}\nСейчас: {1}\nВсего произвелось: {2} раз";
    string template2 = "Угроза: В {0} день {1}";
    string dayStr;
    string dayPartStr;
    string tickStr;

    Subscriber subscribe;

    void Start()
    {
        subscribe = Subscriber.Create(this);
        subscribe.AddEvent(TriggerType.ChangedDaysPart.ToString());
        subscribe.AddEvent(TriggerType.NewDay.ToString());
        subscribe.AddEvent(TriggerType.ProductTick.ToString());

        dayStr = TM.Day.ToString();
        dayPartStr = TM.DayPart.ToString();
        tickStr = TM.Ticks.ToString();

        ReBuild();
    }

    public string Ch
    {
        set
        {
            dayPartStr = value;
        }
    }

    public virtual void ChangedDaysPart()
    {
        dayPartStr = LocalizationManager.Get(TM.DayPart.ToString());
        ReBuild();
    }

    public void NewDay()
    {
        dayStr = TM.Day.ToString();
        ReBuild();
    }

    public void ProductTick()
    {
        tickStr = TM.Ticks.ToString();
        ReBuild();
    }

    public void ReBuild()
    {
        string sText = string.Format(template, dayStr, LocalizationManager.Get(dayPartStr), tickStr);
        
        foreach(var day in TM.Threats)
        {
            sText += "\n" + "<color=red>" + string.Format(template2, day.Day.ToString(), LocalizationManager.Get(day.DayPart.ToString()) ) + "</color>";
        }

        DayPartText.text = sText;
    }

    public void NewThreat()
    {
        ReBuild();
    }
}
