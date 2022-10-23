using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAdditionalResource : UIItem
{
    [SerializeField] SimpleText _title;
    [SerializeField] UIIconText _adds;
    [SerializeField] List<StatItem> _stats;

    public override void Setting()
    {
        base.Setting();

        _title.Text = LocalizationManager.Get("ResourceRequirement");
        _title.Visible = true;

        _adds.Visible = false;
    }

    public void SetupPanel(ActionButtonInfo choice)
    {
        List<iStat> choiceStats = choice.GetStatList();

        for (int i = 0; i < _stats.Count; i++)
        {
            if (i < choiceStats.Count)
            {
                string res1;
                if (!SM.Stats.ContainsKey(choiceStats[i].type))
                    res1 = choiceStats[i].amount + "/" + LocalizationManager.Mark(Mark.red, 0);
                else if(SM.Stats[choiceStats[i].type].Count < choiceStats[i].amount)
                    res1 = choiceStats[i].amount + "/" + LocalizationManager.Mark(Mark.red, SM.Stats[choiceStats[i].type].Count);
                else
                    res1 = choiceStats[i].amount + "/" + LocalizationManager.Mark(Mark.green, SM.Stats[choiceStats[i].type].Count);

                _stats[i].SetupStatItem(choiceStats[i].type, res1);
                _stats[i].Visible = true;
            }
            else
                _stats[i].Visible = false;
        }

        if (!choice.AdditionalText.IsNullOrEmpty())
        {
            _adds.IconText.Text(choice.AdditionalText);
            _adds.IconText.ShowComplete();
            _adds.Visible = true;
        }
        else
            _adds.Visible = false;

        this.Visible = true;
    }
}
