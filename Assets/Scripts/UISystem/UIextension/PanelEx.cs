using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[ExecuteInEditMode]
public class PanelEx : UIeX {

    public override void Setting()
    {
        this._complete = true;

        if (EmptyBack)
            _back.Image = null;
            
        items = gameObject.GetComponentsInChildren<UIItem>().Length;
        _itemsRect = _itemStore.Rect;

        itemsList = new List<UIItem>();

        fadeList = new List<UIeX>();
        childsAnimations = new List<UIeX>();

        fadeCallback = AfterFadeOpen;

        base.Setting();
    }

    #region base parameters
    

    [Separator("Another Settings")]

    public RectTransform _backRect;
    public UIImage _back;
    public ItemStore _itemStore;
    RectTransform _itemsRect;

    [UnityEngine.SerializeField]
    bool HideEmptyPanel = false;

    [UnityEngine.SerializeField]
    bool EmptyBack = false;

    [UnityEngine.SerializeField]
    bool _fade = false;

    public MenuEx _parentMenu;

    int items = 0;

    #endregion

    #region base function

    protected override void Show()
    {
        if (_fitscreen)
            Fit();

        if (Resize != UIResize.Fixed)
            Reset();

        if (_fade)
        {
            this.AlphaZero();
            this.StartFade(true, callback: fadeCallback);
        }
        else
        {
            this.gameObject.SetActive(true);
            AfterFadeOpen();
        }
    } 
    
    public virtual void AddItem(UIItem item)
    {
        items += 1;
        item.transform.SetParent(this._itemStore.transform);
        item.HardSet();
        item.Visible = true;

        if (this.Resize != UIResize.Fixed)
            Reset();
    }

    public virtual void RemoveItem()
    {
        items -= 1;
        if(HideEmptyPanel && items <= 0)
        {
            items = 0;
            this.gameObject.SetActive(false);
        }

    }
    
    #endregion

    #region Panel Animation childs 

    List<UIeX> childsAnimations;

    void AfterFadeOpen()
    {
        childsAnimations.Clear();

        foreach (var item in gameObject.GetComponentsInChildren<UIImage>())
        {
            if (!item.HasParentItem && item.HasAnimation)
                childsAnimations.Add(item);
        }

        foreach (var item in gameObject.GetComponentsInChildren<SimpleText>())
        {
            if (!item.HasParentItem && item.HasAnimation)
                childsAnimations.Add(item);
        }

        foreach (var item in gameObject.GetComponentsInChildren<UIIconText>())
        {
            if (!item.HasParentItem && item.HasAnimation)
                childsAnimations.Add(item);
        }

        foreach (var item in gameObject.GetComponentsInChildren<UIItem>())
        {
            if (item.HasAnimation)
                childsAnimations.Add(item);
        }

        foreach(var child in childsAnimations)
        {
            child.PlayAnimation();
        }
    }

    #endregion

    #region Panel Fading

    List<UIeX> fadeList;
    bool fadingIn = false;
    bool prepareFade = false;

    System.Action fadeCallback;

    public void SetFadeCallback(System.Action call)
    {
        fadeCallback = call;
    }


    void PrepareFade()
    {
        if (!prepareFade || _changeableItem)
        {
            prepareFade = true;
            this.fadeList.Clear();

            foreach (var item in gameObject.GetComponentsInChildren<UIImage>(true))
            {
                if (item.HasParentItem || !item.Visible)
                    continue;

                this.fadeList.Add(item);
            }

            foreach (var item in gameObject.GetComponentsInChildren<SimpleText>(true))
            {
                if (item.HasParentItem || !item.Visible)
                    continue;

                this.fadeList.Add(item);
            }

            foreach (var item in gameObject.GetComponentsInChildren<UIIconText>(true))
            {
                if (item.HasParentItem || !item.Visible)
                    continue;

                this.fadeList.Add(item);
            }

            foreach (var item in gameObject.GetComponentsInChildren<UIItem>(true))
            {
                if (item.HasAnimation || !item.Visible)
                    continue;

                this.fadeList.Add(item);
            }
        }

        if (this.EmptyBack)
            this.fadeList.Remove(this._back);

        foreach(var item in this.fadeList)
        {
            if (this.fadingIn)
                item.SetAlpha(0);
            else
                item.SetAlpha(1);
        }
    }

    void StartFade(bool _in, float duration = 0.5f, System.Action callback = null)
    {
        this.fadingIn = _in;

        PrepareFade();

        if(this.fadingIn)
        {
            this.gameObject.SetActive(true);
            if (gameObject.activeSelf)
                StartCoroutine(FadeCoroutine(duration, callback));
            else
                ReturnOrigin();
        }
        else
        {
            if (gameObject.activeSelf)
                StartCoroutine(FadeCoroutine(duration));
            else
                ReturnOrigin();
        }
        
    }

    protected IEnumerator FadeCoroutine(float fade_duration, System.Action callback = null)
    {
        float remains = fade_duration;

        if(this.fadingIn)
        {
            while (remains > 0)
            {
                remains -= Time.deltaTime;
                float alfa = 1 - remains / fade_duration;
                foreach (var item in this.fadeList)
                    if (item == this._back && this.EmptyBack)
                        continue;
                    else
                        item.SetAlpha(alfa);
                yield return null;
            }
        }
        else
        {
            while (remains > 0)
            {
                remains -= Time.deltaTime;
                float alpa = remains / fade_duration;
                foreach (var item in this.fadeList)
                    item.SetAlpha(alpa);
                yield return null;
            }
            gameObject.SetActive(false);
        }

        callback?.Invoke();

        yield return null;
    }

    #endregion

    #region Canvas Update

    protected List<UIItem> itemsList;

    public override void Reset()
    {
        if (_itemsRect == null)
            _itemsRect = _itemStore.Rect;

        AlphaZero();
        Canvas.ForceUpdateCanvases();
        if (Resize == UIResize.ContentDependence)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_itemsRect);
            _backRect.sizeDelta = _itemsRect.sizeDelta;
            this.Rect.sizeDelta = this.DependenceRect.rect.size;
        }

        if (_fitscreen)
            Fit();

        ReturnOrigin();
    }
    
    protected override void PrepareChilds()
    {
        if (_prepareCanvasUpdate && !_changeableItem)
            return;

        _prepareCanvasUpdate = true;
        
        itemsList = GetComponentsInChildren<UIItem>(true).Where(temp => !temp.HasAnimation).ToList();
        imagess = GetComponentsInChildren<UIImage>(true).Where(temp => !temp.HasParentItem).ToList();
        simplesTexts = GetComponentsInChildren<SimpleText>(true).Where(temp => !temp.HasParentItem).ToList();
        iconsTexts = GetComponentsInChildren<UIIconText>(true).Where(temp => !temp.HasParentItem).ToList();

        if (this.EmptyBack)
            imagess.Remove(this._back);
    }
    
    public override void SetAlpha(float alfa)
    {
        alfa = Mathf.Min(1, Mathf.Max(0, alfa));

        if (this.ID == "TestPanel")
            Debug.LogError("fd");

        foreach (var l in simplesTexts)
            if (l != null)
                l.SetAlpha(alfa);
        foreach (var l in iconsTexts)
            if (l != null)
                l.SetAlpha(alfa);
        foreach (var l in imagess)
            if (l != null)
                l.SetAlpha(alfa);
        foreach (var l in itemsList)
            if (l != null)
                l.SetAlpha(alfa);
    }

    /*
    protected void FadeIn(float duration = 1f, System.Action callback = null)
    {
        AlphaZero();
        if (gameObject.activeSelf)
            StartCoroutine(FadeInCoroutine(duration, callback));
        else
            ReturnOrigin();
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
    }*/
    #endregion

    #region Inside work

    public override Vector3 ScreePoint
    {
        get
        {
            return rect.TransformPoint(this.rect.rect.center);
        }
    }

    protected new void Fit()
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
            Rect.anchoredPosition = new Vector3(Rect.anchoredPosition.x, ScreenHeight - Rect.rect.height / 2 - 6f);
        else if (minY < -ScreenHeight)
            Rect.anchoredPosition = new Vector3(Rect.anchoredPosition.x, Rect.rect.height / 2 + 6f - ScreenHeight);

    }

    public new bool Overlaps(RectTransform over)
    {
        Rect oRect = over.rect;
        oRect.center = over.TransformPoint(over.rect.center);
        oRect.size = over.TransformVector(over.rect.size);

        Rect sRect = Rect.rect;
        sRect.center = Rect.TransformPoint(this.Rect.rect.center);
        sRect.size = Rect.TransformVector(this.Rect.rect.size);

        return sRect.Overlaps(oRect);
    }

    #endregion

#if UNITY_EDITOR
    void Update()
    {
        if (_back.Rect.offsetMin.x != 0)
            _backRect.sizeDelta = new Vector2(this.Rect.sizeDelta.x + _back.Rect.offsetMin.x * 2, this.Rect.sizeDelta.y + _back.Rect.offsetMin.x * 2);
        else
            _backRect.sizeDelta = this.Rect.sizeDelta;

        _itemStore.Rect.sizeDelta = _backRect.sizeDelta;
    }
#endif
}
