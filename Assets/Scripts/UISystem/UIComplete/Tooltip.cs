using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lodkod;
using GameEvents;
using System.Linq;

public class Tooltip : PanelEx {

    public override void Setting()
    {
        if(this.button != null)
        {
            this.button.Visible = false;
            this.button.callback = ButtonClicked;
            this.button.TextIn = "Close";
        }

        this._visible = false;
        this.Active = false;

        text = "NoText";

        ShowedText.PrintCompleted.AddListener(this.CompletedText);

        base.Setting();
    }

    #region base parameters
    

    [UnityEngine.SerializeField]
    protected IconText ShowedText;

    public SkyObject obj;

    private string _buttonText = string.Empty;

    public TooltipFit fit;
    public TooltipTimeMode timeMode;
    public TooltipFillMode fillMode;
    public TooltipObject objectMode;

    public int _lenghtSize;

    public float exTime;

    public string Text;
    public bool CompleteText;
    bool ShowedCompleteText = true;
    public bool Active;

    bool click;
    float temps;

    public Action ActionCallback;

    public SimpleButton button;
    #endregion



    #region base function

    public int LenghtSize
    {
        set
        {
            _lenghtSize = value;
            ShowedText.StringSymbols = value;
        }
    }

    string text
    {
        set
        {
            ShowedText.Text(value);
            /*
            if (!value.Equals(""))
                Reset();*/
        }
    }


    #endregion

    #region base work

    protected override void Show()
    {
        this.Active = false;
        click = false;
        CompleteText = true;

        if (this.timeMode == TooltipTimeMode.ButtonClick)
        {
            if (this.button != null)
                this.button.Visible = true;
            else
                this.timeMode = TooltipTimeMode.Click;
        }
        else if(this.button != null)
            this.button.Visible = false;

        text = LocalizationManager.Get(Text);

        Vector3 tar;
        if (obj)
            tar = obj.ScreePoint;
        else
            tar = target;
        Rect.position = new Vector3(tar.x, tar.y, 0f);

        if (fit == TooltipFit.Auto)
            Fit();

        Reset();

        if (timeMode == TooltipTimeMode.ObjectManagment)
            this._back.Raycast = false;
        else
            this._back.Raycast = true;

        if (fillMode == TooltipFillMode.Instantly)
        {
            ShowedText.ShowComplete();
            ShowedCompleteText = true;
            CompleteText = true;
            this.Active = true;

 //           if (_animation)
//                this.FadeIn(1.0f);
//            else
//                this.gameObject.SetActive(true);
        }
        else
        {
            CompleteText = false;
            ShowedCompleteText = false;
            this.Active = false;
            StartType();

//            if (_animation)
//                this.FadeIn(1.0f, this.StartType);
//            else
//            {
//                this.gameObject.SetActive(true);
//                this.StartType();
//            }
                
        }
    }

    protected override void Hide()
    {
        CompleteText = true;
        click = false;
        obj = null;
        if (timeMode != TooltipTimeMode.Dialog)
        {
            base.Hide();
            this.text = "";
        }
        
        ActionCallback?.Invoke();
    }


    public void CompletedText()
    {
        this.Active = true;
        CompleteText = true;
    }

    void StopType()
    {
        CompleteText = true;
        ShowedText.Skip();
    }

    public void StartType()
    {
        CompleteText = false;

        ShowedText.TypeText();
    }

    protected override void PrepareChilds()
    {
        if (_prepareCanvasUpdate && !_changeableItem)
            return;

        _prepareCanvasUpdate = true;

        simplesTexts = GetComponentsInChildren<SimpleText>(true).ToList();
        iconsTexts = GetComponentsInChildren<UIIconText>(true).ToList();
        imagess = GetComponentsInChildren<UIImage>(true).ToList();
    }

    public override void SetAlpha(float alfa)
    {
        alfa = Mathf.Min(1, Mathf.Max(0, alfa));

        foreach (var l in simplesTexts)
            if (l != null)
                l.Color = new Color(l.Color.r, l.Color.g, l.Color.b, alfa);
        foreach (var l in iconsTexts)
            if (l != null)
                l.IconText.TextComponent.color = new Color(l.IconText.TextComponent.color.r, l.IconText.TextComponent.color.g, l.IconText.TextComponent.color.b, alfa);
        foreach (var l in imagess)
            if (l != null)
                l.Color = new Color(l.Color.r, l.Color.g, l.Color.b, alfa);
    }

    void Update()
    {
        if (!CompleteText)
        {
            if (Input.GetMouseButtonDown(0) && !click)
            {
                temps = Time.time;
                click = true;
            }

            if (Input.GetMouseButtonUp(0) && click)
            {
                click = false;
                if (Time.time - temps < 0.2)
                    StopType();
            }

            return;
        }

        if (!this.Visible)
            return;

        if (timeMode == TooltipTimeMode.ButtonClick)
        {

        }

        if (timeMode == TooltipTimeMode.Tootip)
        {
            if (!CompleteText)
                return;

            exTime -= Time.deltaTime;

            if (exTime <= 0)
            {
                this.Visible = false;
                ActionCallback?.Invoke();
            }
        }

        if (timeMode == TooltipTimeMode.Click)
        {
            if (Input.GetMouseButtonDown(0) && !click)
            {
                temps = Time.time;
                click = true;
            }

            if (Input.GetMouseButtonUp(0) && click)
            {
                click = false;
                if (Time.time - temps < 0.2)
                    this.Visible = false;
            }
        }

        if (timeMode == TooltipTimeMode.MousOn)
        {
            if (!Rect.rect.Contains(Input.mousePosition))
                this.Visible = false;
        }

        if (timeMode == TooltipTimeMode.MouseOnClick)
        {
            if (Rect.rect.Contains(Input.mousePosition))
            {
                if (Input.GetMouseButtonDown(0) && !click)
                {
                    temps = Time.time;
                    click = true;
                }

                if (Input.GetMouseButtonUp(0) && click)
                {
                    click = false;
                    if (Time.time - temps < 0.2)
                        this.Visible = false;
                }
            }
        }

        if (timeMode == TooltipTimeMode.Dialog)
        {
            if (CompleteText && !ShowedCompleteText && ActionCallback != null)
            {
                ShowedCompleteText = true;
                CompleteText = true;
                click = false;
                obj = null;
                this.Active = false;

                ActionCallback?.Invoke();
            }
        }
    }

    public void ButtonClicked()
    {
        this.Visible = false;
    }

    #endregion
}
