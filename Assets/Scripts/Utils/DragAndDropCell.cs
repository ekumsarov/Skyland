using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Every item's cell must contain this script
/// </summary>
public class DragAndDropCell : UIItem, IDropHandler
{
    #region Base
    public enum CellType
    {
        Swap,                                                               // Items will be swapped between cells
        DropOnly,                                                           // Item will be dropped into cell
        DragOnly,                                                           // Item will be dragged from this cell
        UnlimitedSource                                                     // Item will be cloned and dragged from this cell
    }
    public CellType cellType = CellType.Swap;                               // Special type of this cell


    /*
     * Predifined status of cell with items
     */
    public enum CellTypeStatus
    {
        Undefined,
        Closed,                                                        
        Empty,                                                 
        Defenite                                            
    }
    CellTypeStatus _cellTypeStatus = CellTypeStatus.Undefined; 
    public CellTypeStatus Status
    {
        get { return this._cellTypeStatus; }
        set
        {
            if ((value == _cellTypeStatus || value == CellTypeStatus.Defenite) && GetComponentInChildren<DragAndDropItem>() != null)
                return;

            _cellTypeStatus = value;

            if (_cellTypeStatus == CellTypeStatus.Closed)
                Place(DragAndDropItem.Close);
            else if (_cellTypeStatus == CellTypeStatus.Empty)
                Place(DragAndDropItem.Empty);
            
        }
    }

    private DragAndDropItem _myItem;
    public DragAndDropItem CellItem
    {
        get { return this._myItem; }
    }

    /*
     * Prefab to get DragAndDropCell
     */ 
    static DragAndDropCell _prefab;
    public static void PrepareDragCells()
    {
        _prefab = Resources.Load<DragAndDropCell>("Prefabs/DragButtonCell");
    }

    public static DragAndDropCell DragCell
    {
        get
        {
            DragAndDropCell temp = GameObject.Instantiate<DragAndDropCell>(_prefab);
            temp.Status = CellTypeStatus.Empty;
            temp.AllowedItems = new List<string>();
            return temp;
        }
    }

    /*
     * Allowed Items ID
     */
    List<string> AllowedItems;
    public void AddAllowedItems(List<string> collect)
    {
        if (AllowedItems == null)
            AllowedItems = new List<string>();

        AllowedItems.AddRange(collect);
    }
    public void AddAllowedItems(string ID)
    {
        if (AllowedItems == null)
            AllowedItems = new List<string>();

        AllowedItems.Add(ID);
    }


    public struct DropDescriptor                                            // Struct with info about item's drop event
    {
        public DragAndDropCell sourceCell;                                  // From this cell item was dragged
        public DragAndDropCell destinationCell;                             // Into this cell item was dropped
        public DragAndDropItem item;                                        // dropped item
    }

    public Color empty = new Color();                                       // Sprite color for empty cell
    public Color full = new Color();                                        // Sprite color for filled cell

    private System.Action<DropDescriptor> _notifyDelegate;
    public Action<DropDescriptor> NotifyDelegate
    {
        set { this._notifyDelegate += value; }
    }

    void OnEnable()
    {
        DragAndDropItem.OnItemDragStartEvent += OnAnyItemDragStart;         // Handle any item drag start
        DragAndDropItem.OnItemDragEndEvent += OnAnyItemDragEnd;             // Handle any item drag end
    }

    void OnDisable()
    {
        DragAndDropItem.OnItemDragStartEvent -= OnAnyItemDragStart;
        DragAndDropItem.OnItemDragEndEvent -= OnAnyItemDragEnd;
    }

    void Start()
    {
        if(AllowedItems == null)
            AllowedItems = new List<string>();

        SetBackgroundState(GetComponentInChildren<DragAndDropItem>() == null ? false : true);
    }
    #endregion
    /// <summary>
    /// On any item drag start need to disable all items raycast for correct drop operation
    /// </summary>
    /// <param name="item"> dragged item </param>
    private void OnAnyItemDragStart(DragAndDropItem item)
    {
        if (_myItem   != null)
        {
            _myItem.MakeRaycast(false);                                      // Disable item's raycast for correct drop handling
            if (_myItem == item)                                             // If item dragged from this cell
            {
                // Check cell's type
                switch (cellType)
                {
                    case CellType.DropOnly:
                        DragAndDropItem.icon.SetActive(false);              // Item will not be dropped
                        break;
                    case CellType.UnlimitedSource:
                        // Nothing to do
                        break;
                    default:
                        item.MakeVisible(false);                            // Hide item in cell till dragging
                        SetBackgroundState(false);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// On any item drag end enable all items raycast
    /// </summary>
    /// <param name="item"> dragged item </param>
    private void OnAnyItemDragEnd(DragAndDropItem item)
    {
        DragAndDropItem myItem = GetComponentInChildren<DragAndDropItem>(); // Get item from current cell
        if (myItem != null)
        {
            if (myItem == item)
            {
                SetBackgroundState(true);
            }
            myItem.MakeRaycast(true);                                       // Enable item's raycast
        }
        else
        {
            SetBackgroundState(false);
        }
    }

    /// <summary>
    /// Item is dropped in this cell
    /// </summary>
    /// <param name="data"></param>
    public void OnDrop(PointerEventData data)
    {
        if (DragAndDropItem.icon != null)
        {
            if (DragAndDropItem.icon.activeSelf == true)                    // If icon inactive do not need to drop item in cell
            {
                DragAndDropItem item = DragAndDropItem.draggedItem;
                DragAndDropCell sourceCell = DragAndDropItem.sourceCell;
                DropDescriptor desc = new DropDescriptor();

                
                if ((item != null) && (sourceCell != this))
                {
                    switch (sourceCell.cellType)                            // Check source cell's type
                    {
                        case CellType.UnlimitedSource:
                            string itemName = item.name;
                            item = Instantiate(item);                       // Clone item from source cell
                            item.name = itemName;
                            break;
                        default:
                            // Nothing to do
                            break;
                    }

                    if (Status == CellTypeStatus.Closed)
                    {
                        gameObject.SendMessageUpwards("OnItemPlaceUnavailable", desc, SendMessageOptions.DontRequireReceiver);
                        Destroy(item.gameObject);
                        return;
                    }
                    if (AllowedItems.Count > 0)
                    {
                        if(_myItem != null && _myItem.ItemID == item.ItemID)
                        {
                            Debug.LogError("Item is the same");
                            Destroy(DragAndDropItem.icon);
                            return;
                        }

                        bool approved = false;
                        foreach (var ID in AllowedItems)
                        {
                            if (item.ItemTag == ID)
                            {
                                approved = true;
                                break;
                            }
                        }

                        if (!approved)
                        {
                            Debug.LogError("Item not alloewd");
                            gameObject.SendMessageUpwards("OnItemPlaceUnapprove", desc, SendMessageOptions.DontRequireReceiver);
                            Destroy(DragAndDropItem.icon);
                            return;
                        }
                    }

                    switch (cellType)                                       // Check this cell's type
                    {
                        case CellType.Swap:
                            
                            switch (sourceCell.cellType)
                            {
                                case CellType.Swap:
                                    SwapItems(sourceCell, this);            // Swap items between cells
                                    // Fill event descriptor
                                    desc.item = item;
                                    desc.sourceCell = sourceCell;
                                    desc.destinationCell = this;
                                    // Send message with DragAndDrop info to parents GameObjects
                                    StartCoroutine(NotifyOnDragEnd(desc));
                                    if (_myItem != null)
                                    {
                                        // Fill event descriptor
                                        desc.item = _myItem;
                                        desc.sourceCell = this;
                                        desc.destinationCell = sourceCell;
                                        // Send message with DragAndDrop info to parents GameObjects
                                        StartCoroutine(NotifyOnDragEnd(desc));
                                    }
                                    break;
                                default:
                                    PlaceItem(item.gameObject);             // Place dropped item in this cell
                                    // Fill event descriptor
                                    desc.item = item;
                                    desc.sourceCell = sourceCell;
                                    desc.destinationCell = this;
                                    // Send message with DragAndDrop info to parents GameObjects
                                    StartCoroutine(NotifyOnDragEnd(desc));
                                    break;
                            }
                            break;
                        case CellType.DropOnly:
                            PlaceItem(item.gameObject);                     // Place dropped item in this cell
                            // Fill event descriptor
                            desc.item = item;
                            desc.sourceCell = sourceCell;
                            desc.destinationCell = this;
                            // Send message with DragAndDrop info to parents GameObjects
                            StartCoroutine(NotifyOnDragEnd(desc));
                            break;
                        default:
                            // Nothing to do
                            break;
                    }
                }
                if (item.GetComponentInParent<DragAndDropCell>() == null)   // If item have no cell after drop
                {
                    Destroy(item.gameObject);                               // Destroy it
                }
            }
        }
    }

    /// <summary>
    /// Change cell's sprite color on item put/remove
    /// </summary>
    /// <param name="condition"> true - filled, false - empty </param>
    private void SetBackgroundState(bool condition)
    {
        //GetComponent<Image>().color = condition ? full : empty;
    }

    /// <summary>
    /// Delete item from this cell
    /// </summary>
    public void RemoveItem()
    {
        foreach (DragAndDropItem item in GetComponentsInChildren<DragAndDropItem>())
        {
            Destroy(item.gameObject);
        }
        SetBackgroundState(false);
    }

    /// <summary>
    /// Put new item in this cell
    /// </summary>
    /// <param name="itemObj"> New item's object with DragAndDropItem script </param>
    void Place(GameObject itemObj)
    {
        RemoveItem();                                                       // Remove current item from this cell
        if (itemObj != null)
        {
            itemObj.transform.SetParent(_itemStore, false);
            itemObj.transform.localPosition = Vector3.zero;
            DragAndDropItem item = itemObj.GetComponent<DragAndDropItem>();
            if (item != null)
            {
                item.MakeRaycast(true);
                _myItem = item;
            }
            SetBackgroundState(true);
        }
    }

    /// <summary>
    /// Put new item in this cell
    /// </summary>
    /// <param name="itemObj"> New item's object with DragAndDropItem script </param>
    void Place(DragAndDropItem itemObj)
    {
        RemoveItem();                                                       // Remove current item from this cell
        if (itemObj != null)
        {
            itemObj.transform.SetParent(_itemStore, false);
            itemObj.transform.localPosition = Vector3.zero;
            itemObj.MakeRaycast(true);
            _myItem = itemObj;
            SetBackgroundState(true);
        }
    }

    /// <summary>
    /// Put new item in this cell
    /// </summary>
    /// <param name="itemObj"> New item's object with DragAndDropItem script </param>
    public void PlaceItem(GameObject itemObj)
    {
        if (Status != CellTypeStatus.Defenite)
            _cellTypeStatus = CellTypeStatus.Defenite;

        RemoveItem();                                                       // Remove current item from this cell
        if (itemObj != null)
        {
            itemObj.transform.SetParent(_itemStore, false);
            itemObj.transform.localPosition = Vector3.zero;
            DragAndDropItem item = itemObj.GetComponent<DragAndDropItem>();
            if (item != null)
            {
                _myItem = item;
                item.MakeRaycast(true);
            }
            SetBackgroundState(true);
        }
    }

    /// <summary>
    /// Put new item in this cell
    /// </summary>
    /// <param name="itemObj"> New item's object with DragAndDropItem script </param>
    public void PlaceItem(DragAndDropItem itemObj)
    {
        if (Status != CellTypeStatus.Defenite)
            _cellTypeStatus = CellTypeStatus.Defenite;

        RemoveItem();                                                       // Remove current item from this cell
        if (itemObj != null)
        {
            itemObj.transform.SetParent(_itemStore, false);
            itemObj.transform.localPosition = Vector3.zero;
            _myItem = itemObj;
            itemObj.MakeRaycast(true);
            SetBackgroundState(true);
        }
    }

    /// <summary>
    /// Put new item in this cell
    /// </summary>
    /// <param name="itemObj"> New item's object with DragAndDropItem script </param>
    public void PlaceItem(string icon, string ID = "", string itemTag = "DragItem")
    {
        DragAndDropItem itemObj = DragAndDropItem.Item(icon, ID, itemTag);

        if (Status != CellTypeStatus.Defenite)
            _cellTypeStatus = CellTypeStatus.Defenite;

        RemoveItem();                                                       // Remove current item from this cell
        if (itemObj != null)
        {
            itemObj.transform.SetParent(_itemStore, false);
            itemObj.transform.localPosition = Vector3.zero;
            _myItem = itemObj;
            itemObj.MakeRaycast(true);
            SetBackgroundState(true);
        }
    }

    /// <summary>
    /// Get item from this cell
    /// </summary>
    /// <returns> Item </returns>
    public DragAndDropItem GetItem()
    {
        return GetComponentInChildren<DragAndDropItem>();
    }

    /// <summary>
    /// Swap items between to cells
    /// </summary>
    /// <param name="firstCell"> Cell </param>
    /// <param name="secondCell"> Cell </param>
    public void SwapItems(DragAndDropCell firstCell, DragAndDropCell secondCell)
    {
        if ((firstCell != null) && (secondCell != null))
        {
            DragAndDropItem firstItem = firstCell.GetItem();                // Get item from first cell
            DragAndDropItem secondItem = secondCell.GetItem();              // Get item from second cell
            if (firstItem != null)
            {
                // Place first item into second cell
                firstItem.transform.SetParent(secondCell._itemStore, false);
                firstItem.transform.localPosition = Vector3.zero;
                secondCell.SetBackgroundState(true);
            }
            if (secondItem != null)
            {
                // Place second item into first cell
                secondItem.transform.SetParent(firstCell._itemStore, false);
                secondItem.transform.localPosition = Vector3.zero;
                firstCell.SetBackgroundState(true);
            }
        }
    }

    private IEnumerator NotifyOnDragEnd(DropDescriptor desc)
    {
        // Wait end of drag operation
        while (DragAndDropItem.draggedItem != null)
        {
            yield return new WaitForEndOfFrame();
        }
        // Send message with DragAndDrop info to parents GameObjects
        _notifyDelegate?.Invoke(desc);
        //gameObject.SendMessage("OnItemPlace", desc, SendMessageOptions.DontRequireReceiver);
    }
}
