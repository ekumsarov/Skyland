using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using System;
using GameEvents;

public class MapLocationObject : SceneObject
{
    #region Base
    /*
     * описание объекта
     */

    public float CameraFlyTime = 2f;

    public override void HardSet()
    {
        base.HardSet();

        this.IslandNumber = IM.GetIslandNumber(this.position);

        foreach (var loc in this._locations)
        {
            loc.HardSet();
            loc.Visible = false;
            GM.AddUniq(loc);
        }

        IconObject.Create(this.ID + "QuitIcon", "BackArrow", IconInteractType.Object, this._quitLocation);
        IconObject tempIcon = GM.GetIcon(this.ID + "QuitIcon");
        tempIcon.Activity.PushPack("QuitMapLocation", new List<GameEvents.GameEvent>()
        {
            SafeCallFunction.Create("CloseLocations", this.ID),
            MoveCamera.Create(Vector3.zero, false, "island_" + this.IslandNumber),
            ActivateLocationOnIsland.Create(this.IslandNumber)
        });
        tempIcon.MainEvent = "QuitMapLocation"; 

        MapLocationIcon temp = this.gameObject.GetComponentInChildren<MapLocationIcon>(true);
        if(temp!=null)
        {
            temp.name = this.ID + "MapIcon";
            temp.ID = this.ID + "MapIcon";
            temp.SetupMapIcon(this);
            GM.AddIcon(temp);
        }
    }

    #endregion

    public override bool Visible
    {
        get => base.Visible;
        set
        {
            if (this._visible == value)
                return;

            if (value && this._lock)
                return;

            this._visible = value;
            this.gameObject.SetActive(value);
        }
    }


    #region Icon Controll


    [SerializeField] protected List<LocationObject> _locations;
    [SerializeField] protected LocationObject _quitLocation;

    private bool _completeRotation = false;

    public virtual void OpenLocations(Action del = null)
    {
        this.LocationPanel.gameObject.SetActive(false);
        for (int i = 0; i < this._locations.Count; i++)
        {
            this._locations[i].RotatePanels(this.CameraPoint.Point);
            this._locations[i].Visible = true;
        }

        this.Visible = true;

        del?.Invoke();
    }

    public virtual void CloseLocations(Action del = null)
    {
        for (int i = 0; i < this._locations.Count; i++)
        {
            this._locations[i].Visible = false;
        }

        this.Visible = false;
        this.LocationPanel.gameObject.SetActive(true);

        del?.Invoke();
    }

    public LocationObject GetQuitObject()
    {
        return this._quitLocation;
    }
    #endregion
}