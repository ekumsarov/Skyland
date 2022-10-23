using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleButton : UIItem
{
    [SerializeField]
    private SimpleText _text;

    public Action callback;

    public override string TextIn
    {
        get => base.TextIn;
        set
        {
            this._text.Text = value;
        }
    }

    public override void Pressed()
    {
        if (callback != null)
        {
            callback.Invoke();
            return;
        }

        if (_parentMenu != null)
            _parentMenu.Close();
    }
}
