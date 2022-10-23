using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Lodkod;

public class UISelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIItem _parent;

    bool Entered = false;
    bool Showed = false;
    public void OnPointerEnter(PointerEventData data)
    {
        
    }

    public void OnPointerExit(PointerEventData data)
    {

    }
}
