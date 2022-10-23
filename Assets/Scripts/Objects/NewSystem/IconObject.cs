using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lodkod;

[RequireComponent(typeof(Button))]
public class IconObject : SceneObject
{
    ObjectTooltip oTooltip;
    public override void HardSet()
    {
        base.HardSet();

        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClickedIcon);

        _icon = GetComponentInChildren<Image>();

        this._trans = this._button.transform;
    }

    protected virtual void ClickedIcon()
    {
        if (GM.GameState != GameState.Game || this._lock)
            return;

        this.Actioned(this.MainEvent);
    }

    #region Properties

    [SerializeField]
    protected Button _button;
    protected Image _icon;

    protected SceneObject _parent;

    private IconInteractType _interactType;
    public IconInteractType InteractType
    {
        set { this._interactType = value; }
        get { return this._interactType; }
    }

    private IconInteractType _layoutType;
    public IconInteractType LayoutType
    {
        set { this._layoutType = value; }
        get { return this._layoutType; }
    }

    private bool _interacteble;
    public bool IsActive
    {
        get { return this._interacteble; }
    }

    public override bool Visible
    {
        get => base.Visible;
        set
        {
            if (_visible == value)
                return;

            _visible = value;
            this.gameObject.SetActive(value);
        }
    }

    #endregion

    #region Access

    public virtual void SetObjectParent(SceneObject parent)
    {
        if (parent.LocationPanel == null)
        {
            Debug.LogError("Object " + parent.ID + " has no LocationPanel");
            return;
        }

        this._parent = parent;
        this.SetIconTransform(this._parent.LocationPanel.transform);
    }

    protected virtual void SetIconTransform(Transform parent)
    {
        this._button.transform.SetParent(parent);
        this._button.transform.localRotation = new Quaternion(0, 0, 0, 0);
        this._button.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        this._button.transform.localPosition = new Vector3(0f, 0f ,0f);
    }

    public virtual void RemoveIcon()
    {
        this.SetIconTransform(GM.HideIcons);
    }

    public void SetIcon(string icon)
    {
        if(this._icon != null)
            this._icon.sprite = GuiIconProvider.GetIcon(icon);
        else
            this._button.image.sprite = GuiIconProvider.GetIcon(icon);
    }

    #endregion

    #region Another

    public override Transform GetTransform
    {
        get
        {
            if (_trans == null)
                _trans = this._button.transform;

            return _trans;
        }
    }

    #endregion

    #region Icon Tooltip
    bool hasTooltip = false;
    ObjectTooltip _tooltip;
    string _text = null;
    public string Text
    {
        set
        {
            _text = value;

            if (_text == null || _text.Equals("") || _text.Equals("nil"))
                return;

            _text = LocalizationManager.Get(_text);

            if (_tooltip == null)
            {
                _tooltip = gameObject.GetComponent<ObjectTooltip>();
                if (_tooltip != null)
                {
                    hasTooltip = true;
                    _tooltip.objectMode = TooltipObject.UI;
                }
            }


            if (hasTooltip)
            {
                _tooltip.Text = _text;
            }
        }
    }
    #endregion

    #region Wait Bar

    public float timer;
    public float TimerAmount = 3.0f;

    public GameObject BarObject;
    public SimpleHealthBar bar;

    public void ActiveBar()
    {
        this.bar.UpdateBar(0, 1);
        this.BarObject.gameObject.SetActive(true);
    }

    public void DisableBar()
    {
        this.BarObject.gameObject.SetActive(false);
    }

    public void UpdateBar(float percent)
    {
        this.bar.UpdateBar(percent, 1);
    }

    IEnumerator UpdateReact()
    {
        float upTime = this.TimerAmount;
        this.bar.UpdateBar(0, 1);
        this.BarObject.gameObject.SetActive(true);
        _parent.LockLocation(false);
        yield return null;

        upTime -= Time.deltaTime;

        while (upTime > 0)
        {
            this.UpdateBar(1 - (upTime / this.TimerAmount));
            yield return null;
        }

        this.BarObject.gameObject.SetActive(false);
        _parent.Actioned(this.MainEvent);
        this.Lock = false;
        _parent.LockLocation();
        StopAllCoroutines();
    }

    #endregion

    #region Create instance

    private static LocationIcon LocationPrefab;
    private static MapLocationIcon MapLocationPrefab;
    private static IconObject SimpleIconPrefab;

    public static void InitializePrafabs()
    {
        LocationPrefab = Resources.Load<LocationIcon>("Prefabs/IconObjects/LocationInstance");
        MapLocationPrefab = Resources.Load<MapLocationIcon>("Prefabs/IconObjects/MapLocationIcon");
        SimpleIconPrefab = Resources.Load<IconObject>("Prefabs/IconObjects/IconObject");
    }

    public static IconObject GetIcon(string id, string icon)
    {
        IconObject temp = Instantiate(SimpleIconPrefab);
        temp.HardSet();
        temp.ID = id;
        temp.SetIcon(icon);
        temp.InteractType = IconInteractType.Object;
        temp.LayoutType = IconInteractType.Object;

        return temp;
    }

    public static void Create(string id, string icon, IconInteractType type, SceneObject parent = null)
    {
        IconObject temp;
        if(type == IconInteractType.TopLocation)
        {
            temp = Instantiate(MapLocationPrefab);
            temp.HardSet();
            temp.ID = id;
            temp.SetIcon(icon);
            temp.InteractType = type;
            temp.LayoutType = type;
        }
        else if(type == IconInteractType.SubLocation)
        {
            temp = Instantiate(LocationPrefab);
            temp.HardSet();
            temp.ID = id;
            temp.SetIcon(icon);
            temp.InteractType = type;
            temp.LayoutType = type;
        }
        else
        {
            temp = Instantiate(SimpleIconPrefab);
            temp.HardSet();
            temp.ID = id;
            temp.SetIcon(icon);
            temp.InteractType = type;
            temp.LayoutType = type;
        }

        if(parent == null)
            temp.RemoveIcon();
        else
            parent.AddIcon(temp);

        GM.AddIcon(temp);
    }

    public static void Create(string id, string icon, IconInteractType type, IconInteractType layout, SceneObject parent = null)
    {
        IconObject temp;
        if (type == IconInteractType.TopLocation)
        {
            temp = Instantiate(MapLocationPrefab);
            temp.HardSet();
            temp.ID = id;
            temp.SetIcon(icon);
            temp.InteractType = type;
            temp.LayoutType = layout;
        }
        else if (type == IconInteractType.SubLocation)
        {
            temp = Instantiate(LocationPrefab);
            temp.HardSet();
            temp.ID = id;
            temp.SetIcon(icon);
            temp.InteractType = type;
            temp.LayoutType = layout;
        }
        else
        {
            temp = Instantiate(SimpleIconPrefab);
            temp.HardSet();
            temp.ID = id;
            temp.SetIcon(icon);
            temp.InteractType = type;
            temp.LayoutType = layout;
        }

        if (parent == null)
            temp.RemoveIcon();
        else
            parent.AddIcon(temp);

        GM.AddIcon(temp);
    }

    #endregion
}
