using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;

namespace GameEvents
{
    public class IconGameEvent : GameEvent
    {
        protected IconObject _iconObject = null;
        public IconObject IconObject
        {
            get
            {
                if(_iconObject == null)
                    _iconObject = Object as IconObject;

                return _iconObject;
            }
        }

        public override SkyObject Object
        {
            get => base.Object;
            set
            {
                _object = value;
                _iconObject = _object as IconObject;
            }
        }
    }
}