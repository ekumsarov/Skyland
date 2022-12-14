using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Every "drag and drop" item must contain this script
/// </summary>
[RequireComponent(typeof(Image))]
public class DragAndDropItem : UIImage, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    static public DragAndDropItem draggedItem;                                      // Item that is dragged now
    static public GameObject icon;                                                  // Icon of dragged item
    static public DragAndDropCell sourceCell;                                       // From this cell dragged item is

    public delegate void DragEvent(DragAndDropItem item);
    static public event DragEvent OnItemDragStartEvent;                             // Drag start event
    static public event DragEvent OnItemDragEndEvent;                               // Drag end event


    private string _itemTag;
    public string ItemTag
    {
        get { return this._itemTag; }
    }

    private string _itemID;
    public string ItemID
    {
        get { return this._itemID; }
    }

    static DragAndDropItem _prefab;
    public static void PrepareDragItems()
    {
        _prefab = Resources.Load<DragAndDropItem>("Prefabs/UIeX/DragItem");
    }

    public static DragAndDropItem Empty
    {
        get
        {
            DragAndDropItem temp = GameObject.Instantiate<DragAndDropItem>(_prefab);
            temp.GetComponent<Image>().sprite = GuiIconProvider.GetIcon("zoom_plus");
            temp._itemTag = "empty";
            return temp;
        }
    }

    public static DragAndDropItem Close
    {
        get
        {
            DragAndDropItem temp = GameObject.Instantiate<DragAndDropItem>(_prefab);
            temp.GetComponent<Image>().sprite = GuiIconProvider.GetIcon("lock");
            temp._itemTag = "lock";
            return temp;
        }
    }

    public static DragAndDropItem Item(string icon, string ID = "", string fItemTag = "DragItem")
    {
        DragAndDropItem temp = GameObject.Instantiate<DragAndDropItem>(_prefab);
        temp.HardSet();
        temp.GetComponent<Image>().sprite = GuiIconProvider.GetIcon(icon);

        temp.TooltipText = LocalizationManager.Get(ID);
        temp._itemTag = fItemTag;
        temp._itemID = ID;
        if (ID.Equals(""))
            temp._itemID = icon;

        return temp;
    }

    /// <summary>
    /// This item is dragged
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        sourceCell = GetComponentInParent<DragAndDropCell>();                       // Remember source cell
        draggedItem = this;                                                         // Set as dragged item
        icon = new GameObject("Icon");                                              // Create object for item's icon
        Image image = icon.AddComponent<Image>();
        image.sprite = GetComponent<Image>().sprite;
        image.raycastTarget = false;                                                // Disable icon's raycast for correct drop handling
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        // Set icon's dimensions
        iconRect.sizeDelta = new Vector2(   GetComponent<RectTransform>().rect.size.x,
                                            GetComponent<RectTransform>().rect.size.y);
        Canvas canvas = GetComponentInParent<Canvas>();                             // Get parent canvas
        if (canvas != null)
        {
            // Display on top of all GUI (in parent canvas)
            icon.transform.SetParent(canvas.transform, true);                       // Set canvas as parent
            icon.transform.SetAsLastSibling();                                      // Set as last child in canvas transform
        }

        if (OnItemDragStartEvent != null)
        {
            OnItemDragStartEvent(this);                                             // Notify all about item drag start
        }
    }

    /// <summary>
    /// Every frame on this item drag
    /// </summary>
    /// <param name="data"></param>
    public void OnDrag(PointerEventData data)
    {
        if (icon != null)
        {
            icon.transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// This item is dropped
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (icon != null)
        {
            Destroy(icon);                                                          // Destroy icon on item drop
        }
        MakeVisible(true);                                                          // Make item visible in cell
        if (OnItemDragEndEvent != null)
        {
            OnItemDragEndEvent(this);                                               // Notify all cells about item drag end
        }
        draggedItem = null;
        icon = null;
        sourceCell = null;
    }

    /// <summary>
    /// Enable item's raycast
    /// </summary>
    /// <param name="condition"> true - enable, false - disable </param>
    public void MakeRaycast(bool condition)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = condition;
        }
    }

    /// <summary>
    /// Enable item's visibility
    /// </summary>
    /// <param name="condition"> true - enable, false - disable </param>
    public void MakeVisible(bool condition)
    {
        GetComponent<Image>().enabled = condition;
    }
}
