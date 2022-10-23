using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public void NewGame()
    {
        animations = new List<UIAnimation>();
        removeList = new List<UIAnimation>();
        gAnimations = new Dictionary<string, List<UIAnimation>>();
        gRemoveList = new List<string>();
    }

    List<UIAnimation> animations;
    List<UIAnimation> removeList;

    Dictionary<string, List<UIAnimation>> gAnimations;
    List<string> gRemoveList;

    bool active = false;

    void FixedUpdate()
    {
        if (animations == null)
            animations = new List<UIAnimation>();

        if (gAnimations == null)
            gAnimations = new Dictionary<string, List<UIAnimation>>();

        if (animations.Count > 0)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (!animations[i].Update(Time.deltaTime))
                {
                    removeList.Add(animations[i]);
                }
            }
        }
        if (gAnimations.Count > 0)
        {
            foreach (var key in gAnimations.Keys)
            {
                if (!gAnimations[key][0].Update(Time.deltaTime))
                {
                    if (gAnimations[key][0].OnFinish != null)
                        gAnimations[key][0].OnFinish();

                    gAnimations[key].RemoveAt(0);
                    if (gAnimations[key].Count == 0)
                        gRemoveList.Add(key);
                }
            }
            
        }
        RemoveSafely();
    }

    private void RemoveSafely()
    {
        if (removeList == null)
            removeList = new List<UIAnimation>();

        if (gRemoveList == null)
            gRemoveList = new List<string>();

        if (removeList.Count > 0)
        {
            foreach (var item in removeList)
            {
                animations.Remove(item);
                if (item.OnFinish != null && item.stoped)
                    item.OnFinish();
            }
            removeList.Clear();
        }
        if(gRemoveList.Count > 0)
        {
            foreach(var key in gRemoveList)
            {
                gAnimations.Remove(key);
            }
            gRemoveList.Clear();
        }
    }

    public void AddAnimation(List<UIAnimation> anims, bool group = false, string idGroup = "base")
    {
        if (animations == null)
            animations = new List<UIAnimation>();

        if (gAnimations == null)
            gAnimations = new Dictionary<string, List<UIAnimation>>();

        if (group)
        {
            if (!gAnimations.ContainsKey(idGroup))
                gAnimations.Add(idGroup, new List<UIAnimation>());

            gAnimations[idGroup].AddRange(anims);
        }
        else
            animations.AddRange(anims);
    }

    public void AddAnimation(UIAnimation anims, bool group = false, string idGroup = "base")
    {
        if (animations == null)
            animations = new List<UIAnimation>();

        if (gAnimations == null)
            gAnimations = new Dictionary<string, List<UIAnimation>>();

        if (group)
        {
            if (!gAnimations.ContainsKey(idGroup))
                gAnimations.Add(idGroup, new List<UIAnimation>());

            if(!gAnimations[idGroup].Contains(anims))
                gAnimations[idGroup].Add(anims);
        }
        else
            if(!animations.Contains(anims))
                animations.Add(anims);
    }
}
