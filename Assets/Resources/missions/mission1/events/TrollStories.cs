using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class TrollStories : GameEvent
    {

        Subscriber subscriber;

        bool _haveStory;
        List<string> _allStories;

        public override void Init()
        {
            this.ID = "TrollStories";

            Simple = false;

            subscriber = Subscriber.Create(this);

            _haveStory = true;

            _allStories = new List<string>()
            {
                "ds",
                "yterwt"
            };

            initialized = false;
        }

        public void Play()
        {
            if (!initialized)
            {
                initialized = true;
                End();
                return;
            }

            End();
        }

        public void TellStory()
        {
            if(_haveStory == true)
            {
                if(this._allStories.Count>0)
                {
                    int index = UnityEngine.Random.Range(0, this._allStories.Count);
                    string storyKey = this._allStories[index];
                    this._allStories.RemoveAt(index);

                    Object.CallAction(storyKey);
                    _haveStory = false;
                    ExpiredDay.ExpiredAfterDay(DayInfo.Create(12), act: CanTellStory);
                }
                else
                {
                    Object.CallAction("NoStory");
                }

            }
        }

        public void CanTellStory()
        {
            this._haveStory = true;
        }
    }
}