using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class MenuEx : UIeX {

    static int UIItemStat = 0;
    static int UIImageStat = 0;
    static int UIIconTextStat = 0;
    static int SimpleTextStat = 0;

    public override void Setting()
    {
        this._complete = true;

        if (this.name == "TestMenu")
            Debug.LogError("TestMenu");
        
        // Get all Panels to control
        if (_allImages == null)
            this._allImages = new Dictionary<string, UIImage>();
        
        foreach (var but in gameObject.GetComponentsInChildren<UIImage>(true))
        {
            but.HardSet();

            if (this._allImages.ContainsKey(but.ID))
            {
                this._allImages.Add(but.ID + MenuEx.UIImageStat, but);
                MenuEx.UIImageStat += 1;
            }
            else
                this._allImages.Add(but.ID, but);
        }

        // Get all Panels to control
        if (_allIconTexts == null)
            this._allIconTexts = new Dictionary<string, UIIconText>();

        foreach (var but in gameObject.GetComponentsInChildren<UIIconText>(true))
        {
            but.HardSet();

            if (this._allIconTexts.ContainsKey(but.ID))
            {
                this._allIconTexts.Add(but.ID + MenuEx.UIIconTextStat, but);
                MenuEx.UIIconTextStat += 1;
            }
            else
                this._allIconTexts.Add(but.ID, but);
        }

        // Get all Panels to control
        if (_allItems == null)
            this._allItems = new Dictionary<string, UIItem>();

        foreach (var but in gameObject.GetComponentsInChildren<UIItem>(true))
        {
            but.HardSet();
            but._parentMenu = this;

            if (this._allItems.ContainsKey(but.ID))
            {
                this._allItems.Add(but.ID + MenuEx.UIItemStat, but);
                MenuEx.UIItemStat += 1;
            }
            else
                this._allItems.Add(but.ID, but);
        }

        // Get all Panels to control
        if (_allSimpleTexts == null)
            this._allSimpleTexts = new Dictionary<string, SimpleText>();

        foreach (var but in gameObject.GetComponentsInChildren<SimpleText>(true))
        {
            but.HardSet();
            but._parentMenu = this;

            if (this._allSimpleTexts.ContainsKey(but.ID))
            {
                this._allSimpleTexts.Add(but.ID + MenuEx.SimpleTextStat, but);
                MenuEx.SimpleTextStat += 1;
            }
            else
                this._allSimpleTexts.Add(but.ID, but);
        }

        // Get all Panels to control
        if (_allPanels == null)
            this._allPanels = new Dictionary<string, PanelEx>();

        foreach (var but in gameObject.GetComponentsInChildren<PanelEx>(true))
        {
            but.HardSet();
            this._allPanels.Add(but.ID, but);
        }

        // another base settings
        this.Resize = UIResize.Fixed;
        this._fitscreen = false;
        this._layout = false;

        base.Setting();
    }


    #region base parameters
    
    protected Dictionary<string, UIItem> _allItems;
    protected Dictionary<string, UIImage> _allImages;
    protected Dictionary<string, SimpleText> _allSimpleTexts;
    protected Dictionary<string, UIIconText> _allIconTexts;
    protected Dictionary<string, PanelEx> _allPanels;
    

    [UnityEngine.SerializeField]
    string FirstSelected = "nill";

    [UnityEngine.SerializeField]
    bool PanelsOverlap = false;

    #endregion

    #region base function

    public override bool Visible
    {
        get { return _visible; }
        set
        {

            if (value == true && this.Lock)
                return;
            
            this._visible = value;

            if (this._visible)
                this.Show();
            else
                this.Hide();
        }
    }

    public virtual void Open()
    {
        if(FirstSelected.Equals("first"))
        {

        }
        else if(!FirstSelected.Equals("nill") || !FirstSelected.Equals(""))
        {
            if(!_allItems.ContainsKey(FirstSelected))
            Debug.LogError("No such button: " + FirstSelected + " in menu: " + this.ID);
            else
                _allItems[FirstSelected].Selected(true);
        }

        this._lock = false;
        this.Visible = true;
    }

    public virtual void Close()
    {
        this._lock = true;
        this.Visible = false;
    }

    protected override void Show()
    {
        if (this._lock == true)
            return;

        this.gameObject.SetActive(true);

        if(PanelsOverlap)
            ResetAll();
        
        foreach (var panel in _allPanels)
        {
            panel.Value.Visible = true;
        }
    }

    protected override void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void PressedItem(UIItem data)
    {
        Debug.LogError("Pressed item: " + data.ID);
    }

    public virtual void SelectedItem(UIItem data, bool enter)
    {
        //Debug.LogError("Selected item: " + data.ID);
    }

    #endregion

    #region Inside work

    public void ResetAll()
    {
        foreach (var panel in _allItems)
        {
            panel.Value.AlphaZero();
        }

        foreach (var panel in _allPanels)
        {
            panel.Value.AlphaZero();
        }

        Canvas.ForceUpdateCanvases();

        foreach (var panel in _allItems)
        {
            panel.Value.Reset();
        }

        foreach (var panel in _allPanels)
        {
            panel.Value.Reset();
        }

        Overlapped();

        foreach (var panel in _allItems)
        {
            panel.Value.ReturnOrigin();
        }

        foreach (var panel in _allPanels)
        {
            panel.Value.ReturnOrigin();
        }
    }

    public override void Reset()
    {
        foreach(var panel in _allPanels)
        {
            if (panel.Value.Visible)
                panel.Value.Reset();
        }
    }

    public virtual void Overlapped()
    {

        for (int i = 0; i < _allPanels.Count; i++)
        {
            for (int j = 0; j < _allPanels.Count; j++)
            {
                if (i == j)
                    continue;

                PanelEx first = _allPanels.ElementAt(i).Value;
                PanelEx second = _allPanels.ElementAt(j).Value;

                Rect oRect = second.Rect.rect;
                oRect.center = second.Rect.TransformPoint(second.Rect.rect.center);
                oRect.size = second.Rect.TransformVector(second.Rect.rect.size);

                Rect sRect = first.Rect.rect;
                sRect.center = Rect.TransformPoint(first.Rect.rect.center);
                sRect.size = Rect.TransformVector(first.Rect.rect.size);

                if (sRect.Overlaps(oRect))
                {
                    if (first.Rect.position.x > second.Rect.position.x)
                        second.Rect.position = new Vector2(first.Rect.position.x - first.Rect.rect.width / 2 - second.Rect.rect.width / 2 + 10f, second.Rect.position.y);
                    else
                        first.Rect.position = new Vector2(second.Rect.position.x - first.Rect.rect.width / 2 - second.Rect.rect.width / 2 + 10f, first.Rect.position.y);
                }
            }

        }
    }

    /*
     * public virtual to control items
     */
    public virtual void AddItem(UIItem item, string PanelID = null)
    {
        if (_allPanels.Count == 0)
        {
            Debug.LogError("No Panels in Menu: " + this.ID);
            return;
        }
        item._parentMenu = this;

        if (PanelID != null)
            this.AddItem(PanelID, item);
        else
        {
            this._allItems.Add(item.ID, item);
            _allPanels.First().Value.AddItem(item);
        }
    }

    public virtual void RemoveItem(string ID, string PanelID = null)
    {
        if (!_allItems.ContainsKey(ID))
            return;

        if(PanelID != null)
        {
            _allItems[ID].gameObject.transform.SetParent(null);
            _allItems.Remove(ID);
            _allPanels[PanelID].RemoveItem();
        }
        else
        {
            _allItems[ID].gameObject.transform.SetParent(null);
            _allItems.Remove(ID);
            _allPanels.First().Value.RemoveItem();
        }
    }

    public virtual void RemoveItem(UIItem item, string PanelID = null)
    {
        string ID = item.ID;

        if (!_allItems.ContainsKey(ID))
            return;

        if (PanelID != null)
            this.RemoveItem(PanelID, item);
        else
        {
            _allItems[ID].gameObject.transform.SetParent(null);
            _allItems.Remove(ID);
            _allPanels.First().Value.RemoveItem();
        }
    }

    /*
     * protected to control panels in special panel
     */
    protected void AddItem(string PanelID, UIItem item)
    {
        if (_allPanels.ContainsKey(PanelID) == false)
        {
            Debug.LogError("No Panels in Menu: " + this.ID);
            return;
        }

        item._parentMenu = this;
        this._allItems.Add(item.ID, item);
        _allPanels[PanelID].AddItem(item);
    }

    protected void RemoveItem(string PanelID, UIItem item)
    {
        string ID = item.ID;

        if (!_allItems.ContainsKey(ID))
            return;

        _allItems[ID].gameObject.transform.SetParent(null);
        _allItems.Remove(ID);
        _allPanels[PanelID].RemoveItem();
    }

    public void HidePanel(string panel)
    {
        if (this._allPanels.ContainsKey(panel))
            this._allPanels[panel].Visible = false;
    }

    public void ShowPanel(string panel)
    {
        if (this._allPanels.ContainsKey(panel))
            this._allPanels[panel].Visible = true;
    }

    public bool HasItem(string ID)
    {
        return this._allItems.ContainsKey(ID);
    }

    public UIItem GetItem(string ID)
    {
        if (this._allItems.ContainsKey(ID))
            return this._allItems[ID];

        return null;
    }

    public void EnableItem(bool enable, string ID = "All")
    {
        if (ID.Equals("All"))
            foreach (var item in this._allItems)
            {
                if (enable)
                    item.Value.ReturnOriginInteractableStatus();
                else
                    item.Value.DisableItem();
            }
        else
            if (this._allItems.ContainsKey(ID))
            this._allItems[ID].Interactable = enable;
    }

    #endregion
}
