using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

[RequireComponent(typeof(IconText))]
public class UIIconText : UIeX
{
    public override void Setting()
    {
        if (this._iconText == null)
            this._iconText = gameObject.GetComponent<IconText>();

        this._textColor = this._iconText.TextComponent;

        if (this._iconText == null)
            Debug.LogError("Some trouble with IconText");

        base.Setting();
    }

    #region Base parameters

    [UnityEngine.SerializeField]
    protected IconText _iconText;
    public IconText IconText
    {
        get { return this._iconText; }
    }

    protected Text _textColor;
    public Text TextColor
    {
        get { return this._textColor; }
    }

    public override void SetAlpha(float alfa)
    { 

        this._textColor.color = new Color(this._textColor.color.r, this._textColor.color.g, this._textColor.color.b, alfa);
    }

    #endregion
}
