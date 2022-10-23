using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.UI;
using Lodkod;
using SimpleJSON;

public class UIItemEx : UIeXBase
{
    public enum ButtonTranstion { Image, Color }

    public enum FrameType { Selectable, Programm }

    #region base parameters

    [UnityEngine.SerializeField]
    public string ItemTag = "base";

    public UIImage _frame;
    public RectTransform _backRect;
    public UIImage _back;
    public UIItemStoreEx _itemStore;

    public MenuEx _parentMenu;

    [UnityEngine.SerializeField]
    bool _frameOn = false;

    public FrameType _frameType = FrameType.Selectable;

    public Button _but;
    [UnityEngine.SerializeField]
    bool _interactable = true;

    public bool SelectedAction = false;

    bool _interactableOrigin = true;

    UIItemSelector Selector = null;
    UIController _controller;

    protected List<UIContainer> _containers;
    protected List<SimpleText> _simpleTexts;
    protected List<UIIconText> _iconTexts;
    protected List<UIImage> _images;
    #endregion

    #region setup

    public override void Setting()
    {
        this._complete = true;

        if (!_interactable && _but != null)
            _but.interactable = false;
        else
        {
            if (_but == null)
                _but = gameObject.GetComponent<Button>();

            if (_but == null)
            {
                this.gameObject.AddComponent<Button>();
                this._but = this.gameObject.GetComponent<Button>();
            }

            if (_interactable)
                _but.interactable = true;
            else
                _but.interactable = false;
        }

        _controller = GetComponent<UIController>();

        _but.onClick.AddListener(Pressed);

        if (this._tooltipActive)
            this.TooltipActive = true;


        if (this.gameObject.GetComponent<UIItemSelector>() != null)
        {
            this.Selector = this.gameObject.GetComponent<UIItemSelector>();
            //this.Selector.Setup(this);
        }
        else
        {
            this.gameObject.AddComponent<UIItemSelector>();
            this.Selector = this.gameObject.GetComponent<UIItemSelector>();
            //this.Selector.Setup(this);
        }

        if (this._frame != null)
            this._frame.Visible = this._frameOn;


        this._simpleTexts = new List<SimpleText>();
        this._iconTexts = new List<UIIconText>();
        this._images = new List<UIImage>();
        this._containers = new List<UIContainer>();

        this._itemStore.Parent = this;

        base.Setting();

    }

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

    public override void Reset()
    {
        Canvas.ForceUpdateCanvases();
        if (Resize == UIResize.ContentDependence)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_itemStore.Rect);
            _backRect.sizeDelta = _itemStore.Rect.sizeDelta;
            this.Rect.sizeDelta = this.DependenceRect.rect.size;
        }
    }

    public bool Frame
    {
        get { return this._frameOn; }
        set
        {
            if (this._frame == null)
                return;

            this._frameOn = value;
            this._frame.Visible = value;
        }
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

        //_parentMenu.PressedItem(this);
    }
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
                if (this._tooltipText == null || this._tooltipText.Equals(""))
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
    }

    public virtual void Selected(bool enter)
    {
        if (enter)
        {
            if (this._frameType == FrameType.Selectable)
                this.Frame = true;

            if (this._tooltipActive)
                UIM.ShowTooltip(this, Lodkod.TooltipFit.Auto, Lodkod.TooltipTimeMode.ObjectManagment, Lodkod.TooltipFillMode.Instantly, Lodkod.TooltipObject.UI, this._tooltipText, lSize: 30);
        }
        else
        {
            if (this._frameType == FrameType.Selectable)
                this.Frame = false;

            if (this._tooltipActive)
                UIM.HideTooltip(this);
        }

        //if (this.SelectedAction || this._parentMenu != null)
        //    this._parentMenu.SelectedItem(this, enter);

    }

    #endregion

    #region Inside work

    public void AddImage(UIImage item)
    {
        this._images.Add(item);
    }

    public void AddSimpleText(SimpleText item)
    {
        this._simpleTexts.Add(item);
    }

    public void AddIconText(UIIconText item)
    {
        this._iconTexts.Add(item);
    }

    public void AddContainer(UIContainer item)
    {
        this._containers.Add(item);
    }


    public override Vector3 ScreePoint
    {
        get
        {
            return rect.TransformPoint(this.rect.rect.center);
        }
    }

    #endregion
}