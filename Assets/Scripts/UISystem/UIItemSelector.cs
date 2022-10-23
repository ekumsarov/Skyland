using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Lodkod;

public class UIItemSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIItem _parent;
    bool entered = false;

    public void Setup(UIItem par)
    {
        this._parent = par;
        this.entered = false;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (entered)
            return;

        if (_parent == null)
            _parent = this.gameObject.GetComponent<UIItem>();

        if (_parent == null)
            return;
        
        _parent.Selected(true);
        entered = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!entered)
            return;

        if (_parent == null)
            _parent = this.gameObject.GetComponent<UIItem>();

        if (_parent == null)
            return;
        
        entered = false;
        _parent.Selected(false);
    }
}