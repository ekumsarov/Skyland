using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;

public class LocationObject : SceneObject
{
    #region Base
    /*
     * описание объекта
     */

    public override void HardSet()
    {
        base.HardSet();

        this.ID = gameObject.name;

        foreach (var child in this._locationChilds)
        {
            if (child.Initialized == false)
            {
                child.HardSet();
                child.Visible = false;
            }
            GM.AddUniq(child);
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
            foreach(var child in this._locationChilds)
            {
                child.Visible = value;
            }
        }
    }


    #region Icon Controll

    public List<LocationObject> _locationChilds;

    private MapLocationObject _parentObject;
    public MapLocationObject ParentObject
    {
        set { this._parentObject = value; }
    }

    public void RotatePanels(Vector3 look)
    {
        if (this.LocationPanel != null)
            this.LocationPanel.transform.LookAt(Camera.main.transform);

        foreach(var child in this._locationChilds)
        {
            child.RotatePanels(look);
        }
    }
    #endregion
}