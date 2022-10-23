using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIData : ObjectID
{
    string ObjectID;
    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

}

public class UIController : MonoBehaviour, ObjectID
{
    string ObjectID;
    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }

    public virtual void ApplyData(UIData data)
    {

    }

    public virtual void SetupUI()
    {

    }

    public virtual bool Visible
    {
        get;
        set;
    }

    public virtual void Pressed()
    {

    }
}

public class UIMenuController : UIController
{
    private UIMenuEx _parentMenu;
    public UIMenuEx Parent
    {
        set { this._parentMenu = value; this.SetupUI(); }
    }

    protected Dictionary<string, UIItemController> containers;
}

public class UIItemController : UIController
{

}

/*
public interface IUIData<T>
   where T : UIData
{
    public abstract void ApplyChanges(T parameters);
}

public abstract class UIItemController<T> : MonoBehaviour, IUIData<T>
    where T : UIData
{
    public virtual void ApplyChanges(T parameters)
    { }

    public virtual void SetupUI()
    { }

    public virtual void Pressed()
    { }

    public virtual bool Visible
    {
        get;
        set;
    }
}
*/
