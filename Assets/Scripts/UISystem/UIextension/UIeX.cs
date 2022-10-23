using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.UI;
using Lodkod;



public class UIeX : SkyObject {

    /*
     * UIeX типы
     */
    public enum UIResize { Fixed, ContentDependence, Layout }
    public enum UIeXType { Menu, Panel, Item, SimpleText, UIImage, UIButton, UIIconText }

    /*
     * Initialization function for all UIeX inherited classes
     */
    #region set
    protected bool _complete = false;
    
    public override void HardSet()
    {
        if (this._complete)
            return;

        base.HardSet();
        this.Setting();
    }
    #endregion

    /*
     * Virtual Settinf function for all inherited classes. Must be called in every subclass
     */
    public virtual void Setting()
    {
        this._complete = true;

        this.ID = gameObject.name;

        rect = this.gameObject.GetComponent<RectTransform>();
        parentCanvas = UIM.Root;

        if (this.gameObject.GetComponent<HorizontalLayoutGroup>() != null || this.gameObject.GetComponent<VerticalLayoutGroup>() != null || this.gameObject.GetComponent<GridLayoutGroup>() != null)
            this._layout = true;

        if (this._layout)
            this.Resize = UIResize.Layout;

        if (this.Resize == UIResize.ContentDependence && this.DependenceRect == null)
        {
            Debug.LogError("Not set dependence for UI: " + this.ID);
            this.Resize = UIResize.Fixed;
        }

        this._selfIconText = this.gameObject.GetComponent<UIIconText>();
        this._selfText = this.gameObject.GetComponent<SimpleText>();
        this._selfImage = this.gameObject.GetComponent<UIImage>();

        _parallelAnimations = new Dictionary<string, List<UIAnimation>>();
        _groupAnimations = new Dictionary<string, List<UIAnimation>>();

        if (Animations.Count > 0)
            _parallelAnimations.Add("base", new List<UIAnimation>());

        foreach(UIAnimationInfo anim in Animations)
        {
            UIAnimation temp;

            if (anim.Animation == UIAnimationType.Scale)
                temp = UIM.Scale(this, anim._Scalestart, anim._Scaleend, anim._duration);
            else if(anim.Animation == UIAnimationType.Rotate)
                temp = UIM.Scale(this, anim._Rotatestart, anim._Rotateend, anim._duration);
            else if (anim.Animation == UIAnimationType.Fade)
                temp = UIM.FadeAnimation(this, anim._duration, anim._in);
            else 
                temp = UIM.Move(this, anim._start, anim._end, anim._duration);

            temp.SetDelay(anim._delay);
            _parallelAnimations["base"].Add(temp);
        }

        if (_parallelAnimations.ContainsKey("base") && _parallelAnimations["base"].Count > 0)
            this._animation = true;

        PrepareChilds();

        if (this._visible)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    /*
     * Base Parameters for UIeX
     */
    #region base parameters

    // Rect hash
    protected RectTransform rect;
    // Root canvas
    Canvas parentCanvas;

    // Content dependence. Need to reset or not the rect size of parent item
    public UIResize Resize = UIResize.ContentDependence;
    [ConditionalField("Resize", UIResize.ContentDependence)] public RectTransform DependenceRect;

    // Active?
    [UnityEngine.SerializeField]
    protected bool _visible = true;

    // LayoutComponent
    protected bool _layout = false;

    // Flag need to be fit the screen
    [UnityEngine.SerializeField]
    protected bool _fitscreen = true;

    // Flag to clode UIeX. Any of base working functions could not call item
    [UnityEngine.SerializeField]
    protected bool _lock = false;
    
    #endregion

    #region base function

    public RectTransform Rect
    {
        get
        {
            if (rect == null)
                rect = this.gameObject.GetComponent<RectTransform>();

            return rect;
        }
    }

    // Call to check active. Call to activate or disable item
    public virtual bool Visible
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
                this.Hide();
        }
    }
    // Lock item to close it
    public virtual bool Lock
    {
        get { return this._lock; }
        set
        {
            this._lock = value;
        }
    }

    // How to activate item
    protected virtual void Show()
    {
        if (_fitscreen)
            Fit();

        if (Resize != UIResize.Fixed)
            Reset();

        this.gameObject.SetActive(true);
    }

    // How to hide item
    protected virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }

    #endregion

    /*
     * Base states for UI Animation
     */
    #region Animation
    
    [Separator("Animation")]

    public bool _changeableItem = false;
    
    protected bool _animation = false;
    public bool HasAnimation { get { return this._animation; } }


    protected string _animationDependence = null;

    [SerializeField] List<UIAnimationInfo>  Animations;

    Dictionary<string, List<UIAnimation>> _parallelAnimations;
    Dictionary<string, List<UIAnimation>> _groupAnimations;
    #endregion

    /*
     * Needs to reset the size of all items
     */
    #region Update Canvas

    [SerializeField] protected bool _hasItemParent = false;
    public bool HasParentItem
    {
        get { return this._hasItemParent; }
        set { this._hasItemParent = value; }
    }

    protected SimpleText _selfText;
    protected UIIconText _selfIconText;
    protected UIImage _selfImage;

    protected List<SimpleText> simplesTexts = null;
    protected List<UIIconText> iconsTexts = null;
    protected List<UIImage> imagess = null;

    protected  bool _prepareCanvasUpdate = false;
    
    protected virtual void PrepareChilds()
    {

    }

    public void AlphaZero()
    {
        PrepareChilds();
        SetAlpha(0);
        gameObject.SetActive(true);
    }

    public void ReturnOrigin()
    {
        gameObject.SetActive(this._visible);
        SetAlpha(1);
    }

    public virtual void SetAlpha(float alfa)
    {
        alfa = Mathf.Min(1, Mathf.Max(0, alfa));

        if (this._selfIconText != null)
            this._selfIconText.SetAlpha(alfa);
        if (this._selfText != null)
            this._selfText.SetAlpha(alfa);
        if (this._selfImage != null)
            this._selfImage.SetAlpha(alfa); 
    }

    protected void Fit()
    {
        float ScreenWidth = (Screen.width / UIM.ScreenScale) / 2;
        float ScreenHeight = (Screen.height / UIM.ScreenScale) / 2;

        float topX = Rect.anchoredPosition.x + Rect.rect.width / 2;
        float minX = Rect.anchoredPosition.x - Rect.rect.width / 2;

        float topY = Rect.anchoredPosition.y + Rect.rect.height / 2;
        float minY = Rect.anchoredPosition.y - Rect.rect.height / 2;

        if (topX > ScreenWidth)
            Rect.anchoredPosition = new Vector3(ScreenWidth - Rect.rect.width / 2 - 6f, Rect.anchoredPosition.y);
        else if (minX < -ScreenWidth)
            Rect.anchoredPosition = new Vector3(Rect.rect.width / 2 + 6f - ScreenWidth, Rect.anchoredPosition.y);


        if (topY > ScreenHeight)
            Rect.anchoredPosition = new Vector3(Rect.anchoredPosition.x, Rect.anchoredPosition.y - (topY - ScreenHeight) - 6f);
        else if (minY < -ScreenHeight)
            Rect.anchoredPosition = new Vector3(Rect.anchoredPosition.x, Rect.rect.height / 2 + 6f - ScreenHeight);

    }

    public bool Overlaps(RectTransform over)
    {
        Rect oRect = over.rect;
        oRect.center = over.TransformPoint(over.rect.center);
        oRect.size = over.TransformVector(over.rect.size);

        Rect sRect = Rect.rect;
        sRect.center = Rect.TransformPoint(this.Rect.rect.center);
        sRect.size = Rect.TransformVector(this.Rect.rect.size);

        return sRect.Overlaps(oRect);
    }

    public virtual void Reset()
    {
        AlphaZero();
        Canvas.ForceUpdateCanvases();
        if (Resize == UIResize.ContentDependence && DependenceRect != null)
            this.rect.sizeDelta = DependenceRect.sizeDelta;

        gameObject.SetActive(false);
        ReturnOrigin();
    }

    #endregion

    #region special function

        // Gets the screen point
    public override Vector3 ScreePoint
    {
        get
        {
            return rect.TransformPoint(this.rect.rect.center);
        }
    }

    // Call some special function from item
    public void SafeCall(string methodName)
    {
        if (string.IsNullOrEmpty(methodName)) return;
        MethodInfo mi = this.GetType().GetMethod(methodName);
        if (mi != null)
            mi.Invoke(this, null);
        else
            Debug.LogError(ID + ": Can't find method " + methodName);
    }

    // Sets the color
    public void SetColor(Color vol)
    {
        if (this._selfIconText != null)
            this._selfIconText.IconText.TextComponent.color = vol;
        if (this._selfText != null)
            this._selfText.Color = vol;
        if (this._selfImage != null)
            this._selfImage.Color = vol;
    }

    public void SetOriginColor()
    {

    }

    public void SaveOriginColor()
    {

    }

    #endregion

    #region Animation sets

    public void StopAllAnimations()
    {
        foreach (var ainm in this._groupAnimations)
            foreach (var anim in ainm.Value)
                anim.Stop();

        foreach (var ainm in this._parallelAnimations)
            foreach (var anim in ainm.Value)
                anim.Stop();
    }

    public void PlayAnimation(string anim = "base")
    {
        if(this._parallelAnimations.ContainsKey(anim))
        {
            foreach (var ans in this._parallelAnimations[anim])
            {
                ans.SetDelay(ans.Delay);
                ans.stoped = false;
            }
            UIM.UIanimator.AddAnimation(this._parallelAnimations[anim]);
        }
        else if(this._groupAnimations.ContainsKey(anim))
            UIM.UIanimator.AddAnimation(this._groupAnimations[anim], true, anim);
    }
    

    public void SetAnimation(List<UIAnimation> anims, string id, bool clear = false, bool group = false)
    {
        if(group)
        {
            if (clear && this._groupAnimations.ContainsKey(id))
                this._groupAnimations[id].Clear();

            if (this._groupAnimations.ContainsKey(id))
                this._groupAnimations[id].AddRange(anims);
            else
                this._groupAnimations.Add(id, anims);

            return;
        }

        if (clear && this._parallelAnimations.ContainsKey(id))
            this._parallelAnimations[id].Clear();

        if (this._parallelAnimations.ContainsKey(id))
            this._parallelAnimations[id].AddRange(anims);
        else
            this._parallelAnimations.Add(id, anims);

    }

    public void SetAnimation(UIAnimation anim, string id, bool clear = false, bool group = false)
    {
        if (group)
        {
            if (clear && this._groupAnimations.ContainsKey(id))
                this._groupAnimations[id].Clear();

            if (this._groupAnimations.ContainsKey(id))
                this._groupAnimations[id].Add(anim);
            else
                this._groupAnimations.Add(id, new List<UIAnimation>() { anim });

            return;
        }

        if (clear && this._parallelAnimations.ContainsKey(id))
            this._parallelAnimations[id].Clear();

        if (this._parallelAnimations.ContainsKey(id))
            this._parallelAnimations[id].Add(anim);
        else
            this._parallelAnimations.Add(id, new List<UIAnimation>() { anim });
    }



    #endregion
}

