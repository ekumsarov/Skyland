using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InfoButton : UIItem
{
    [SerializeField]
    private bool _hasIcon = true;

    [SerializeField]
    private bool _hasText = false;

    [SerializeField]
    private string _iconID;

    [SerializeField]
    private UIImage _icon;

    [SerializeField]
    private SimpleText _text;

    public override string TextIn
    {
        get => base.TextIn;
        set
        {
            this._text.Text = value;
        }
    }

    public string Icon
    {
        set
        {
            this._iconID = value;
            this._icon.Image = value;
        }
    }

    public bool HasIcon
    {
        set
        {
            this._hasIcon = value;
            this._icon.Visible = value;
        }
    }

    public bool HasText
    {
        set
        {
            this._hasText = value;
            this._text.Visible = value;
        }
    }


    public override void Setting()
    {
        base.Setting();

        this._icon.Visible = _hasIcon;
        this._text.Visible = _hasText;

        if (!_iconID.IsNullOrEmpty())
            _icon.Image = _iconID;
    }
}