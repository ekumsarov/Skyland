using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class UIImage : UIeX, IPointerEnterHandler, IPointerExitHandler
{

    public override void Setting()
    {
        this.Resize = UIResize.Fixed;

        if (this.image == null)
            this.image = gameObject.GetComponent<Image>();

//        ItemStore store = GetComponentInParent<ItemStore>();
//        if (store != null)
//            store._parent.AddImage(this);

        this.keepAlpha = this.image.color.a;
        
        base.Setting();
    }

    #region Base Parameters

    [UnityEngine.SerializeField]
    protected Image image;

    // May cause problem!!!!
    //
    // It needs to kepp half alpha
    //
    float keepAlpha = 1.0f;
    bool entered = false;
    #endregion

    #region 

    public string Image
    {
        set
        {
            if (image == null)
                this.image = gameObject.GetComponent<Image>();

            if (value == null)
            {
                this.image.overrideSprite = null;
                this.image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            else
                this.image.overrideSprite = GuiIconProvider.GetIcon(value);
        }
    }

    public Sprite GetSprite
    {
        get { return this.image.sprite; }
    }

    public Color Color
    {
        get
        {
            if (image == null)
                this.image = gameObject.GetComponent<Image>();

            return this.image.color;
        }
        set
        {
            if (image == null)
                this.image = gameObject.GetComponent<Image>();

            this.image.color = value;
        }
    }

    public override void SetAlpha(float alfa)
    {
        if (alfa > this.keepAlpha)
            alfa = keepAlpha;

        if (image == null)
            this.image = gameObject.GetComponent<Image>();
        
        this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, alfa);
    }

    public void Fade(float alfa, float time, bool ignore)
    {
        this.image.CrossFadeAlpha(alfa, time, ignore);
    }

    public bool Raycast
    {
        set { this.image.raycastTarget = value; }
    }

    public float Fill
    {
        get { return this.image.fillAmount; }
        set { this.image.fillAmount = value; }
    }

    public void AddFill(float fill)
    {
        this.image.fillAmount += fill;
    }


    public bool Filled
    {
        get { return this.image.fillAmount >= 1.0f; }
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

    public void OnPointerEnter(PointerEventData data)
    {
        if (entered)
            return;


        if (this._tooltipActive)
        {
            UIM.ShowTooltip(this, Lodkod.TooltipFit.Auto, Lodkod.TooltipTimeMode.ObjectManagment, Lodkod.TooltipFillMode.Instantly, Lodkod.TooltipObject.UI, this.TooltipText, lSize: 30);
            entered = true;
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!entered)
            return;

        
        if (this._tooltipActive)
        {
            UIM.HideTooltip(this);
            entered = false;
        }
            
    }

    /*public virtual void Selected(bool enter)
    {
        if (enter)
        {
            if (this._tooltipActive)
                UIM.ShowTooltip(this, Lodkod.TooltipFit.Auto, Lodkod.TooltipTimeMode.ObjectManagment, Lodkod.TooltipFillMode.Instantly, Lodkod.TooltipObject.UI, this.TooltipText, lSize: 30);
        }
        else
        {
            if (this._tooltipActive)
                UIM.HideTooltip(this);
        }
    }*/

    public delegate string TooltipTextDelegate();
    protected TooltipTextDelegate _myTooltipText;
    public void SetTextDelegate(TooltipTextDelegate del)
    {
        this._myTooltipText = del;
        this.TooltipActive = true;
    }

    #endregion
}
