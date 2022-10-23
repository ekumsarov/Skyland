using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class MapMenu : MenuEx {

    float widthScale;
    float heightScale;

    float IslandIcoWidth;
    float IslandIcoHeight;

    List<MapObject> _islands;

    Activity act;

    PanelEx MapPanel;
    
    public override void Setting()
    {
        base.Setting();

        this._islands = new List<MapObject>();
        this.act = new Activity();
        this.MapPanel = this._allPanels["Map"];
        
    }

    public void InitMap()
    {
        MapObject copyItem = GameObject.Instantiate(Resources.Load<MapObject>("Prefabs/UIeX/Complete/MapObject"));

        this.widthScale = (MapPanel.Rect.sizeDelta.x - 160 - 10) / IM.MapSize.x ;
        this.heightScale = (MapPanel.Rect.sizeDelta.y - 120 - 10) / IM.MapSize.y ;

        foreach (var isl in IM.Islands)
        {
            MapObject temp = GameObject.Instantiate(copyItem);
            temp._connected = isl;
            temp._parentMenu = this;
            MapPanel.AddItem(temp);
            temp.HardSet();

            float x = isl.position.x * this.widthScale;
            float y = isl.position.y * this.heightScale;

            temp.transform.localPosition = new Vector3(x, y, 0f);
            _islands.Add(temp);
            this._allItems.Add(temp.ID, temp);
        }
    }

    public override void Open()
    {
        foreach(var item in _islands)
        {
            if (item.ItemTag.Equals("IslandMap"))
                item.IslandUpdate();
        }

        base.Open();
    }

    public override void PressedItem(UIItem data)
    {
        base.PressedItem(data);
    }
}
