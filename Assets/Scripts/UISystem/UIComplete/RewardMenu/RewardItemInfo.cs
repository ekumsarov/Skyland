using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemInfo
{
    private string _icon;
    public string Icon
    {
        get { return this._icon; }
    }

    private float _valueFirst;
    private float _valueSecond;
    public string Value
    {
        get { return this._presenter.Formatter(_valueFirst, _valueSecond); }
    }

    public bool HasValue
    {
        get { return this._presenter.Format != Represent.Type.Non; }
    }

    private Represent _presenter;

    public static RewardItemInfo Create(string icon, float fValue = 0.0f, float sValue = 0.0f, Represent.Type type = Represent.Type.Simple)
    {
        if (fValue == 0 && sValue == 0)
            type = Represent.Type.Non;

        return new RewardItemInfo()
        {
            _icon = icon,
            _valueFirst = fValue,
            _valueSecond = sValue,
            _presenter = new Represent() { Format = type }
        };
    }
}
