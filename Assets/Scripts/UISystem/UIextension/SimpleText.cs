using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SimpleText : UIeX
{
    public override void Setting()
    {
        //this._visible = false;
//        ItemStore store = GetComponentInParent<ItemStore>();
//        if (store != null)
//            store._parent.AddSimpleText(this);

        base.Setting();
    }

    #region Base Parameters

    [UnityEngine.SerializeField]
    protected Text text;

    public MenuEx _parentMenu;

    #endregion

    #region 

    public string Text
    {
        get
        {
            if (this.text == null)
                this.text = gameObject.GetComponent<Text>();

            return text.text;
        }
        set
        {
            if (this.text == null)
                this.text = gameObject.GetComponent<Text>();

            this.text.text = LocalizationManager.Get(value);
        }
    }

    public Color Color
    {
        get
        {
            if (text == null)
                this.text = gameObject.GetComponent<Text>();

            return this.text.color;
        }
        set
        {
            if (text == null)
                this.text = gameObject.GetComponent<Text>();

            this.text.color = value;
        }
    }

    public override void SetAlpha(float alfa)
    {
        this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, alfa);
    }

    #endregion
}
