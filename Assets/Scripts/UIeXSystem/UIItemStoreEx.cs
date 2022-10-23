using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemStoreEx : MonoBehaviour
{
    public UIItemEx Parent;

    protected RectTransform rect;
    public RectTransform Rect
    {
        get
        {
            if (rect == null)
                rect = this.gameObject.GetComponent<RectTransform>();

            return rect;
        }
    }
}
