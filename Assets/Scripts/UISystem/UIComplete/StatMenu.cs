using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatMenu : MenuEx
{
    PanelEx MainPanel;

    public override void Setting()
    {
        base.Setting();

        if (_allPanels.ContainsKey("StatPanel"))
            MainPanel = _allPanels["StatPanel"];
    }

    public override void AddItem(UIItem item, string PanelID = null)
    {
        base.AddItem(MainPanel.ID, item);
    }

    public override void RemoveItem(string ID, string PanelID = null)
    {
        base.RemoveItem(MainPanel.ID, ID);
    }

    
}
