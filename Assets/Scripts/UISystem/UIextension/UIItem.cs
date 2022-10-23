using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SimpleJSON;
using System.Linq;

[ExecuteInEditMode]
public class UIItem : UIeX {

    public enum ButtonTranstion { Image, Color }

    public enum FrameType { Selectable, Programm }

    public override void Setting()
    {
        this._complete = true;

        this.data = new ItemData();

        if (!_interactable && _but != null)
            _but.interactable = false;
        else
        {
            if(_but == null)
                _but = gameObject.GetComponent<Button>();

            if(_but == null)
            {
                this.gameObject.AddComponent<Button>();
                this._but = this.gameObject.GetComponent<Button>();
            }

            if(_interactable)
                _but.interactable = true;
            else
                _but.interactable = false;
        }

        _but.onClick.AddListener(Pressed);

        if (this._tooltipActive)
            this.TooltipActive = true;


        if (this.gameObject.GetComponent<UIItemSelector>() != null)
        {
            this.Selector = this.gameObject.GetComponent<UIItemSelector>();
            this.Selector.Setup(this);
        }
        else
        {
            this.gameObject.AddComponent<UIItemSelector>();
            this.Selector = this.gameObject.GetComponent<UIItemSelector>();
            this.Selector.Setup(this);
        }

        if(this._frame != null)
        {
            this._frame.Visible = this._frameOn;
            this._frameOriginColor = this._frame.Color;
        }
            


        PrepareChilds();

        base.Setting();
    }

    #region base parameters

    [UnityEngine.SerializeField]
    public string ItemTag = "base";

    public UIImage _frame;
    public RectTransform _backRect;
    public UIImage _back;
    public RectTransform _itemStore;
    
    public MenuEx _parentMenu;

    [UnityEngine.SerializeField]
    bool _frameOn = false;

    public FrameType _frameType = UIItem.FrameType.Selectable;

    public Button _but;
    [UnityEngine.SerializeField]
    bool _interactable = true;

    public bool SelectedAction = false;

    bool _interactableOrigin = true;

    ItemData data;
    #endregion

    #region base function

    // Call to check active. Call to activate or disable item
    public override bool Visible
    {
        get { return _visible; }
        set
        {

            if (_lock)
                return;

            this._visible = value;

            if (this._visible)
                this.Show();
            else
            {
                this.Frame = false;
                this.Hide();
            }
                
        }
    }

    public virtual void SetInfo(JSONNode node)
    {
        this.Info = node;
    }

    public override void Reset()
    {
        Canvas.ForceUpdateCanvases();
        if (Resize == UIResize.ContentDependence)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_itemStore);
            _backRect.sizeDelta = _itemStore.sizeDelta;
            this.Rect.sizeDelta = this.DependenceRect.rect.size;
        }
    }

    private Color _frameOriginColor = Color.clear;

    public bool Frame
    {
        get { return this._frameOn; }
        set
        {
            if(this._frame == null)
            {
//                Debug.LogError("No frame setup! FIX!!!");
                return;
            }

            this._frameOn = value;
            this._frame.Visible = value;
        }
    }

    public void ColorFrameRed()
    {
        if (this._frameOriginColor == Color.clear)
            this._frameOriginColor = this._frame.Color;

        this._frame.SetColor(new Color(0.9f, 0.1f, 0.12f, 1f));
    }

    public void ResetFrame()
    {
        this._frame.SetColor(this._frameOriginColor);
    }

    public bool Interactable
    {
        get { return this._interactable; }
        set
        {
            this._interactable = value;
            this._but.interactable = value;
        }
    }

    public void DisableItem()
    {
        this._interactableOrigin = this._interactable;
        this.Interactable = false;
    }

    public void ReturnOriginInteractableStatus()
    {
        this.Interactable = this._interactableOrigin;
    }

    public virtual void Pressed()
    {
        if (_parentMenu == null)
        {
            Debug.LogError("Clicked " + this.ID + " with no set of Menu");
            return;
        }

        _parentMenu.PressedItem(this);
    }

    public virtual void SetSize(Vector2 size)
    {
        this.rect.sizeDelta = size;
        this._back.Rect.sizeDelta = size;
        this._itemStore.sizeDelta = size;
    }
    #endregion

    #region Fading


    protected override void PrepareChilds()
    {
        if (_prepareCanvasUpdate && !_changeableItem)
            return;

        if (this.ID == "TestItem")
            Debug.LogError("TestItem");

        _prepareCanvasUpdate = true;

        simplesTexts = GetComponentsInChildren<SimpleText>(true).ToList();
        iconsTexts = GetComponentsInChildren<UIIconText>(true).ToList();
        imagess = GetComponentsInChildren<UIImage>(true).ToList();
        

        foreach (var tempSubItem in iconsTexts)
            tempSubItem.HasParentItem = true;
        foreach (var tempSubItem in simplesTexts)
            tempSubItem.HasParentItem = true;
        foreach (var tempSubItem in imagess)
            tempSubItem.HasParentItem = true;
    }

    public override void SetAlpha(float alfa) 
    {
        PrepareChilds();

        alfa = Mathf.Min(1, Mathf.Max(0, alfa));
        
        foreach (var l in simplesTexts)
            if (l != null)
                l.SetAlpha(alfa);
        foreach (var l in iconsTexts)
            if (l != null)
                l.SetAlpha(alfa);
        foreach (var l in imagess)
            if (l != null)
                l.SetAlpha(alfa);

    }

    /*
    protected void FadeIn(float duration = 1f, System.Action callback = null)
    {
        MakeInvisible();
        if (gameObject.activeSelf)
            StartCoroutine(FadeInCoroutine(duration, callback));
        Shutdown();
    }

    protected IEnumerator FadeInCoroutine(float fade_duration, System.Action callback = null)
    {
        float remains = fade_duration;
        while (remains > 0)
        {
            remains -= Time.deltaTime;
            SetAlpha(1 - remains / fade_duration);
            yield return null;
        }

        if (callback != null)
        {
            callback();
        }

        yield return null;
    }

    protected void FadeOut(float duration = 1f)
    {
        PrepareFade();
        if (gameObject.activeSelf)
            StartCoroutine(FadeOutCoroutine(duration));
        else
            Shutdown();
    }

    IEnumerator FadeOutCoroutine(float fade_duration)
    {
        yield return null;
        float remains = fade_duration;
        while (remains > 0)
        {
            remains -= Time.deltaTime;
            SetAlpha(remains / fade_duration);
            yield return null;
        }
        gameObject.SetActive(false);
    }
    */


    #endregion

    #region Tooltip
    [Separator("Tooltip Settings")]
    public bool _tooltipActive = false;
    public bool TooltipActive
    {
        get { return this._tooltipActive; }
        set
        {
            if (value)
            {
                if ((this._tooltipText == null || this._tooltipText.Equals("")) && this._myTooltipText == null)
                {
                    Debug.LogError("Set tooltip text for item: " + this.ID);
                    this._tooltipActive = false;
                    return;
                }

                this._tooltipActive = true;
            }
            else
            {
                UIM.HideTooltip(this);
                this._tooltipActive = false;
            }
                
        }
    }
    protected string _tooltipText = null;
    public string TooltipText
    {
        set
        {
            _tooltipText = value;
            
            if (value == null || value.Equals(""))
                this.TooltipActive = false;
            else
                this.TooltipActive = true;
        }
        get
        {
            if (_myTooltipText != null)
                _tooltipText = _myTooltipText();

            return this._tooltipText;
        }
    }
    UIItemSelector Selector = null;

    public virtual void Selected(bool enter)
    {
        if(enter)
        {
            if (this._frameType == FrameType.Selectable)
                this.Frame = true;

            if(this._tooltipActive)
                UIM.ShowTooltip(this, Lodkod.TooltipFit.Auto, Lodkod.TooltipTimeMode.ObjectManagment, Lodkod.TooltipFillMode.Instantly, Lodkod.TooltipObject.UI, this.TooltipText, lSize: 30);
        }
        else
        {
            if (this._frameType == FrameType.Selectable)
                this.Frame = false;

            if (this._tooltipActive)
                UIM.HideTooltip(this);
        }

        if (this.SelectedAction || this._parentMenu != null)
            this._parentMenu.SelectedItem(this, enter);

    }

    public delegate string TooltipTextDelegate();
    protected TooltipTextDelegate _myTooltipText;
    public void SetTextDelegate(TooltipTextDelegate del)
    {
        this._myTooltipText = del;
        this.TooltipActive = true;
    }

    #endregion

    #region Inside work

    public override Vector3 ScreePoint
    {
        get
        {
            return rect.TransformPoint(this.rect.rect.center);
        }
    }

    #endregion

    #region data setup
    public virtual int ItemNum
    {
        get { return this.data.ItemNum; }
        set { this.data.ItemNum = value; }
    }
    public virtual int Integer1
    {
        get { return this.data.Integer1; }
        set { this.data.Integer1 = value; }
    }
    public virtual int Integer2
    {
        get { return this.data.Integer2; }
        set { this.data.Integer2 = value; }
    }
    public virtual float Float
    {
        get { return this.data.Float; }
        set { this.data.Float = value; }
    }
    public virtual string TextIn
    {
        get { return this.data.TextIn; }
        set { this.data.TextIn = value; }
    }
    public virtual string DataInfo
    {
        get { return this.data.DataInfo; }
        set { this.data.DataInfo = value; }
    }
    public virtual JSONNode Info
    {
        get { return this.data.Info; }
        set { this.data.Info = value; }
    }
    #endregion



#if UNITY_EDITOR
    void Update()
    {
        if (this.Resize == UIResize.Fixed)
            return;

        if (_back.Rect.offsetMin.x != 0)
            _backRect.sizeDelta = new Vector2(this.Rect.sizeDelta.x + _back.Rect.offsetMin.x * 2, this.Rect.sizeDelta.y + _back.Rect.offsetMin.x * 2);
        else
            _backRect.sizeDelta = this.Rect.sizeDelta;

        _itemStore.sizeDelta = _backRect.sizeDelta;
    }
#endif
}

class ItemData
{
    public int ItemNum = -1;
    public int Integer1 = -1;
    public int Integer2 = -1;
    public float Float = 0.0f;
    public string TextIn = null;
    public string DataInfo = null;
    public JSONNode Info;
}

