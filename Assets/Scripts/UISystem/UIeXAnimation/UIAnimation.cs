using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIAnimationInfo 
{
    [UnityEngine.SerializeField] public UIAnimationType Animation = UIAnimationType.None;

    [ConditionalFieldReverse("Animation", UIAnimationType.None)] [UnityEngine.SerializeField] public UIeX UIObject = null;
    [ConditionalFieldReverse("Animation", UIAnimationType.None)] [UnityEngine.SerializeField] public float _duration = 1f;
    [ConditionalFieldReverse("Animation", UIAnimationType.None)] [UnityEngine.SerializeField] public float _delay = 0f;
    [ConditionalField("Animation", UIAnimationType.Fade)] [UnityEngine.SerializeField] public bool _in = true;
    [ConditionalField("Animation", UIAnimationType.Color)] [UnityEngine.SerializeField] public Color _colotstart = Color.white;
    [ConditionalField("Animation", UIAnimationType.Color)] [UnityEngine.SerializeField] public Color _colorend = Color.white;
    [ConditionalField("Animation", UIAnimationType.Move)] [UnityEngine.SerializeField] public Vector3 _start = Vector3.zero;
    [ConditionalField("Animation", UIAnimationType.Move)] [UnityEngine.SerializeField] public Vector3 _end = Vector3.zero;
    [ConditionalField("Animation", UIAnimationType.Scale)] [UnityEngine.SerializeField] public Vector3 _Scalestart = Vector3.zero;
    [ConditionalField("Animation", UIAnimationType.Scale)] [UnityEngine.SerializeField] public Vector3 _Scaleend = Vector3.zero;
    [ConditionalField("Animation", UIAnimationType.Rotate)] [UnityEngine.SerializeField] public Vector3 _Rotatestart = Vector3.zero;
    [ConditionalField("Animation", UIAnimationType.Rotate)] [UnityEngine.SerializeField] public Vector3 _Rotateend = Vector3.zero;
}



public abstract class UIAnimation
{
    public delegate void AnimationCallback();

    private float timer = 0;
    private float delay = 0;
    private bool paused = false;
    private AnimationCallback onFinish;
    protected float duration;
    protected UpdateBehaviour updateBehaviour;

    public bool stoped = false;
    public int Loops;
    public float Duration { get { return duration; } }
    public float Delay { get { return delay; } }
    public AnimationCallback OnFinish { get { return onFinish; } }
    public UIeX AnimObject;
    public UIeXBase AnimObjectSecond;

    public Action UICallback;

    /*  Update
     *  Updates the animation.
     *
     *  Sums the time variation to the timer. Then
     *  calls the abstract function OnUpdate so each
     *  animation tells what need to be update.
     *  Timer goes from (-delay/duration) to 1 in
     *  (delay + duration) seconds.
     */

    public virtual bool Update(float deltaTime)
    {
        if (paused)
            return true;

        if(stoped)
        {
            OnEnd();
            return false;
        }

        timer += deltaTime / duration;
        //Debug.Log("timer: " + timer);

        if (timer < 0)
            OnUpdate(0);
        else if (timer < 1)
        {
            OnUpdate(timer);
        }
        else
        {
            if(Loops == -1)
            {
                timer = 0;
                OnUpdate(0);
                return true;
            }
            else if(Loops > 0)
                Loops -= 1;
            else if(Loops == 0)
            {
                stoped = true;
                OnEnd();
                return false;
            }

            OnEnd();
            return false;
        }
        return true;
    }

    /*  On Update, On End
     *      Abstract class.
     *
     *      Must be override by subclass.
     */

    public abstract void OnUpdate(float timer);

    public abstract void OnEnd();

    public abstract void Reverse();

    public abstract UIAnimation GetReverseAnimation();

    /*  Set Effect
     *      Add effect to animation
     *
     *      Effects may behave differently depending
     *      on the animation, so it can be might be
     *      overridden by subclass.
     */

    public virtual UIAnimation SetEffect(Effect.EffectUpdate effect, Quaternion rotation = default(Quaternion))
    {
        return this;
    }

    /*  Set Callback
     *      Add callback to animation.
     *
     *      Callbacks are called when the animation ends.
     */

    public UIAnimation SetCallback(AnimationCallback callback, bool add = false)
    {
        if (add) onFinish += callback;
        else onFinish = callback;
        return this;
    }

    public UIAnimation SetUICallback(Action callback)
    {
        this.UICallback = callback;
        return this;
    }

    /*  Set Modifier
     *      Add modifier to animation.
     *
     *      Modifier change how the timer affects the animation.
     */

    public UIAnimation SetModifier(UpdateBehaviour updateBehaviour)
    {
        this.updateBehaviour = updateBehaviour;
        return this;
    }

    /*  Set Delay
     *      Add delay to animation.
     */

    public UIAnimation SetDelay(float delay)
    {
        this.delay = delay;
        timer = -delay / duration;
        return this;
    }

    /*  Set Loop
     *      Set the animation to be a loop.
     *      The animation can also be PingPong
     *      (replays from the end to beginning).
     *      A value can be set as a delay.
     *      Please notice that overrides any previously setted callback.
     */

    public UIAnimation SetLoop(bool pingPong = false)
    {
        SetCallback(() =>
        {
            if (pingPong) Reverse();
            Play();
        });
        return this;
    }

    public UIAnimation SetLoops(int count, bool pingPong = false)
    {
        this.Loops = count;
        if(pingPong)
            this.Loops = this.Loops*2 - 1;

        SetCallback(() =>
        {
            if (pingPong) Reverse();
            Play();
        });

        return this;
    }

    /*  Play
     *      Play animation.
     */

    public void Play()
    {
        if (paused)
            paused = false;
        else
            Restart();
    }

    /*  Pause
     *      Pause animation.
     */

    public void Pause(bool playIfPaused = false)
    {
        if (playIfPaused)
            if (paused)
                Play();
            else
                paused = true;
        else
            paused = true;
    }

    /*  Restart
     *      Restart animation.
     */

    public void Restart()
    {
        SetDelay(delay);
        stoped = false;
        UIM.UIanimator.AddAnimation(this);
    }

    /*  Stop
     *      Stop animation.
     */

    public void Stop()
    {
        stoped = true;
        //            UIAnimator.RemoveAnimation(this);
    }
}

/*! UI Position Animation
 *      UI Animation - Movement Animation
 *
 *      Overrides superclass abstract methods.
 *      Updates transform position.
 */

public class UIPositionAnimation : UIAnimation
{
    private Vector2 originPosition;
    private Vector2 targetPosition;
    protected Effect.EffectBehaviour effectBehaviour;

    public UIPositionAnimation(UIeX obj, UIPositionAnimation animation) :
    this(obj, animation.originPosition, animation.targetPosition, animation.duration)
    {
        originPosition = animation.originPosition;
        targetPosition = animation.targetPosition;
        updateBehaviour = animation.updateBehaviour;
        effectBehaviour = animation.effectBehaviour;
    }

    public UIPositionAnimation(UIeX obj, Vector2 origin, Vector2 target, float duration)
    {
        this.AnimObject = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;
        this.targetPosition = target;
        this.originPosition = origin;

//        if(target.x > 1.0f)
//            this.targetPosition = target - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
//        else
//            this.targetPosition = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;

//        this.originPosition = Vector2.Scale(origin, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
//        this.targetPosition = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
    }

    public UIPositionAnimation(UIeXBase obj, Vector2 origin, Vector2 target, float duration)
    {
        this.AnimObjectSecond = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;

        if (target.x > 1.0f)
            this.targetPosition = target - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        else
            this.targetPosition = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;

        this.originPosition = Vector2.Scale(origin, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        //        this.targetPosition = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
    }

    public override void OnUpdate(float timer)
    {
        this.AnimObject.Rect.anchoredPosition = Vector2.Lerp(originPosition, targetPosition, updateBehaviour(timer)) + effectBehaviour(timer);
    }

    public override void OnEnd()
    {
        this.AnimObject.Rect.anchoredPosition = targetPosition;
        UICallback?.Invoke();
    }

    public override void Reverse()
    {
        Vector3 aux = originPosition;
        originPosition = targetPosition;
        targetPosition = aux;
    }

    public override UIAnimation GetReverseAnimation()
    {
        return new UIPositionAnimation(this.AnimObject, targetPosition, originPosition, duration);
    }

    /*  Set Effect
     *      Sets effect
     *
     *      For more on Effects, please see Effects class
     */

    public override UIAnimation SetEffect(Effect.EffectUpdate effect, Quaternion rotation = default(Quaternion))
    {
        Vector2 direction = (targetPosition - originPosition).normalized;
        direction = (direction == Vector2.zero) ? Vector2.right : direction;
        Vector2 directionVector = rotation * direction;
        directionVector *= UIM.ScreenDimension.y;
        this.effectBehaviour = Effect.GetBehaviour(effect, directionVector);
        return this;
    }
}

public class UIBezierAnimation : UIPositionAnimation
{
    public UIBezierAnimation(UIeX obj, Vector2 origin, Vector2 target, float duration, Vector2 p1, Vector2 p2) : base(obj, origin, target, duration)
    {
        Vector2 mP0 = Vector2.Scale(origin, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP1 = Vector2.Scale(p1, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP2 = Vector2.Scale(p2, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP3 = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        effectBehaviour = Effect.BezierEffectBehaviour(mP0, mP1, mP2, mP3);
    }

    public UIBezierAnimation(UIeX obj, Vector2 origin, Vector2 target, float duration, Vector2 p1) : base(obj, origin, target, duration)
    {
        Vector2 mP0 = Vector2.Scale(origin, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP1 = Vector2.Scale(p1, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP2 = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        effectBehaviour = Effect.BezierEffectBehaviour(mP0, mP1, mP2);
    }

    public UIBezierAnimation(UIeXBase obj, Vector2 origin, Vector2 target, float duration, Vector2 p1, Vector2 p2) : base(obj, origin, target, duration)
    {
        Vector2 mP0 = Vector2.Scale(origin, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP1 = Vector2.Scale(p1, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP2 = Vector2.Scale(p2, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP3 = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        effectBehaviour = Effect.BezierEffectBehaviour(mP0, mP1, mP2, mP3);
    }

    public UIBezierAnimation(UIeXBase obj, Vector2 origin, Vector2 target, float duration, Vector2 p1) : base(obj, origin, target, duration)
    {
        Vector2 mP0 = Vector2.Scale(origin, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP1 = Vector2.Scale(p1, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        Vector2 mP2 = Vector2.Scale(target, UIM.ScreenDimension) - (Vector2)this.AnimObject.Rect.transform.position + this.AnimObject.Rect.anchoredPosition;
        effectBehaviour = Effect.BezierEffectBehaviour(mP0, mP1, mP2);
    }

    public override void OnUpdate(float timer)
    {
        this.AnimObject.Rect.anchoredPosition = effectBehaviour(timer);
    }
}

/*! UI Scale Animation
 *      UI Animation - Scale Animation
 *
 *      Overrides superclass abstract methods.
 *      Updates transform localScale.
 */

public class UIScaleAnimation : UIAnimation
{
    private Vector3 mainOriginScale;
    private Vector3 originScale;
    private Vector3 targetScale;
    private Effect.EffectBehaviour effectBehaviour;

    public UIScaleAnimation(UIeX obj, UIScaleAnimation animation) :
    this(obj, animation.originScale, animation.targetScale, animation.duration)
    { }

    public UIScaleAnimation(UIeX obj, Vector3 origin, Vector3 target, float duration)
    {
        this.AnimObject = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;
        this.mainOriginScale = origin;
        this.originScale = origin;
        this.targetScale = target;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
    }

    public UIScaleAnimation(UIeXBase obj, Vector3 origin, Vector3 target, float duration)
    {
        this.AnimObjectSecond = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;
        this.mainOriginScale = origin;
        this.originScale = origin;
        this.targetScale = target;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
    }

    public override void OnUpdate(float timer)
    {
        this.AnimObject.Rect.localScale = Vector3.Lerp(originScale, targetScale, updateBehaviour(timer)) + (Vector3)effectBehaviour(timer);
    }

    public override void OnEnd()
    {
        if(stoped)
            this.AnimObject.Rect.localScale = this.mainOriginScale;
        else
            this.AnimObject.Rect.localScale = targetScale;
    }

    public override void Reverse()
    {
        Vector3 aux = originScale;
        originScale = targetScale;
        targetScale = aux;
    }

    public override UIAnimation GetReverseAnimation()
    {
        return new UIScaleAnimation(this.AnimObject, targetScale, originScale, duration);
    }

    /*  Set Effect
     *      Sets effect
     *
     *      For more on Effects, please see Effects class
     */

    public override UIAnimation SetEffect(Effect.EffectUpdate effect, Quaternion rotation = default(Quaternion))
    {
        this.effectBehaviour = Effect.GetBehaviour(effect, rotation * (targetScale - originScale));
        return this;
    }
}

/*! UI Rotation Animation
 *      UI Animation - Rotation Animation
 *
 *      Overrides superclass abstract methods.
 *      Updates transform localRotation.
 */

public class UIRotationAnimation : UIAnimation
{
    private float originAngle;
    private float targetAngle;
    private Quaternion originRotation;
    private Quaternion targetRotation;
    private Effect.EffectBehaviour effectBehaviour;
    private bool unclamped = true;

    public UIRotationAnimation(UIeX obj, UIRotationAnimation animation) :
    this(obj, animation.originAngle, animation.targetAngle, animation.duration)
    { }

    public UIRotationAnimation(UIeX obj, Quaternion origin, Quaternion target, float duration)
    {
        this.AnimObject = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;
        this.originRotation = origin;
        this.targetRotation = target;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
        unclamped = false;
    }

    public UIRotationAnimation(UIeX obj, float origin, float target, float duration)
    {
        this.AnimObject = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;
        this.originAngle = origin;
        this.targetAngle = target;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
        unclamped = true;
    }

    public UIRotationAnimation(UIeXBase obj, Quaternion origin, Quaternion target, float duration)
    {
        this.AnimObjectSecond = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;
        this.originRotation = origin;
        this.targetRotation = target;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
        unclamped = false;
    }

    public UIRotationAnimation(UIeXBase obj, float origin, float target, float duration)
    {
        this.AnimObjectSecond = obj;
        this.duration = duration < 0.0000001f ? 0.0000001f : duration;
        this.originAngle = origin;
        this.targetAngle = target;
        updateBehaviour = Modifier.Linear;
        effectBehaviour = Effect.NoEffect;
        unclamped = true;
    }

    public override void OnUpdate(float timer)
    {
        if (unclamped)
        {
            this.AnimObject.Rect.localRotation = Quaternion.AngleAxis(Mathf.Lerp(originAngle, targetAngle, timer), Vector3.forward) * Quaternion.Euler(Vector3.forward * effectBehaviour(timer).x);
        }
        else
        {
            this.AnimObject.Rect.localRotation = Quaternion.Lerp(originRotation, targetRotation, updateBehaviour(timer)) * Quaternion.Euler(Vector3.forward * effectBehaviour(timer).x);
        }
    }

    public override void OnEnd()
    {
        if (unclamped)
        {
            this.AnimObject.Rect.localRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        }
        else
        {
            this.AnimObject.Rect.localRotation = targetRotation;
        }
    }

    public override void Reverse()
    {
        if (unclamped)
        {
            float aux = originAngle;
            originAngle = targetAngle;
            targetAngle = aux;
        }
        else
        {
            Quaternion aux = originRotation;
            originRotation = targetRotation;
            targetRotation = aux;
        }
    }

    public override UIAnimation GetReverseAnimation()
    {
        return new UIRotationAnimation(this.AnimObject, targetAngle, originAngle, duration);
    }

    public override UIAnimation SetEffect(Effect.EffectUpdate effect, Quaternion rotation = default(Quaternion))
    {
        this.effectBehaviour = Effect.GetBehaviour(effect, Vector2.right);
        return this;
    }
}

/*! UI Sprite Animation
 *      UI Animation - Color Animation
 *
 *      Overrides superclass abstract methods.
 *      Updates image color.
 */

public class UIFadeAnimation : UIAnimation
{
    bool _in;

    public UIFadeAnimation(UIeX obj, float duration, bool _in, float delay = 0, Action callback = null)
    {
        this.AnimObject = obj;
        this.duration = duration;
        this.SetDelay(delay);
        this._in = _in;
        this.UICallback = callback;
    }

    public UIFadeAnimation(UIeXBase obj, float duration, bool _in, float delay = 0, Action callback = null)
    {
        this.AnimObjectSecond = obj;
        this.duration = duration;
        this.SetDelay(delay);
        this._in = _in;
        this.UICallback = callback;
    }

    public override void OnUpdate(float timer)
    {
        if(_in)
            this.AnimObject.SetAlpha(timer);
        else
            this.AnimObject.SetAlpha(1-timer);
    }

    public override void OnEnd()
    {
        if (_in)
            this.AnimObject.SetAlpha(1);
        else
        {
            this.AnimObject.SetAlpha(0);
            this.AnimObject.gameObject.SetActive(false);
        }

        UICallback?.Invoke();
    }

    public override void Reverse()
    {
        this._in = !this._in;
    }

    public override UIAnimation GetReverseAnimation()
    {
        return new UIFadeAnimation(this.AnimObject, duration, !this._in, Delay, UICallback);
    }
}

public class UIColorAnimation : UIAnimation
{
    Color originColor;
    Color targetColor;
    UIImage target;

    public UIColorAnimation(UIImage obj, Color origin, Color target, float duration, float delay = 0, Action callback = null)
    {
        this.AnimObject = obj;
        this.target = obj;
        this.duration = duration;
        this.SetDelay(delay);
        this.UICallback = callback;
        this.originColor = origin;
        this.targetColor = target;
        this.updateBehaviour = Modifier.Linear;
    }

    public override void OnUpdate(float timer)
    {
        this.AnimObject.SetColor(Color.Lerp(originColor, targetColor, updateBehaviour(timer)));
    }

    public override void OnEnd()
    {
        UICallback?.Invoke();
    }

    public override void Reverse()
    {
        Color aux = this.targetColor;
        this.targetColor = originColor;
        this.originColor = aux;
    }

    public override UIAnimation GetReverseAnimation()
    {
        return new UIColorAnimation(this.target, targetColor, originColor, duration, Delay, UICallback);
    }
}

public class UIGroupAnimation : UIAnimation
{
    protected List<UIAnimation> anims;

    public UIGroupAnimation(List<UIAnimation> anim)
    {
        this.anims = anim;
        float dur = 0f;
        foreach (var ani in anim)
        {
            if (dur < ani.Duration + ani.Delay)
                dur = ani.Duration + ani.Delay;
        }
        this.duration = dur < 0.0000001f ? 0.0000001f : dur;
    }

    public override bool Update(float deltaTime)
    {
        bool contin = true;

        foreach(var anim in anims)
        {
            if (!anim.Update(deltaTime))
                contin = false;
        }

        return contin;
    }

    public override void OnUpdate(float timer)
    {
        foreach(var ani in anims)
        {
            ani.OnUpdate(timer);
        }
    }

    public override void OnEnd()
    {
        
    }

    public override void Reverse()
    {
    }

    public override UIAnimation GetReverseAnimation()
    {
        throw new NotImplementedException();
    }
}

public delegate float UpdateBehaviour(float deltaTime);

/*!  Modifier
 *      Change animation behaviour.
 *
 *      Returns a float value used in inside
 *      UIAnimation.OnUpdate to change the timer
 *      growth curve, changing the animation.
 *      To add a new modifier simply create a new
 *      UpdateBehaviour function.
 *      CAUTION:
 *      1. Functions must attend: f(0) = 0 & f(1) = 1.
 *      2. It is used inside a Lerp function, any
 *      values above 1 may have unexpected behaviour.
 */

public static class Modifier
{
    public static float Linear(float time)
    {
        return time;
    }

    public static float QuadOut(float time)
    {
        return time * time;
    }

    public static float QuadIn(float time)
    {
        return (float)Mathf.Pow(time, 0.50f);
    }

    public static float CubOut(float time)
    {
        return time * time * time;
    }

    public static float CubIn(float time)
    {
        return Mathf.Pow(time, 0.33f);
    }

    public static float PolyOut(float time)
    {
        return time * time * time * time;
    }

    public static float PolyIn(float time)
    {
        return Mathf.Pow(time, 0.25f);
    }

    public static float Sin(float time)
    {
        return 0.5f + 0.5f * Mathf.Cos((1 - time) * Mathf.PI);
    }

    public static float Tan(float time)
    {
        return 2 * time - Sin(time);
    }

    public static float CircularIn(float time)
    {
        return Mathf.Sqrt(Mathf.Sin(time * Mathf.PI / 2));
    }

    public static float CircularOut(float time)
    {
        return 1 - Mathf.Sqrt(Mathf.Cos(-time * Mathf.PI / 2));
    }
}

/*!  Effect
 *      Add new values to the animation.
 *
 *      Returns a Vector2 from (float time) adding a new behaviour
 *      to the animation.
 *      To add a new effect you must create a new EffectUpdate function
 *      You can use a float and a int parameter to adjust your effect
 *      CAUTION:
 *      1. Functions must attend: f(0) = 0 & f(1) = 0.
 *      2. You must also create a EffectGroup, so the effect can be
 *      used in a GroupAnimation
 */

public static class Effect
{
    public delegate EffectUpdate EffectGroup(float max, int bounce);

    public delegate Vector2 EffectBehaviour(float time);

    public delegate float EffectUpdate(float time);

    public static Vector2 NoEffect(float time)
    {
        return Vector2.zero;
    }

    public static EffectUpdate Spring(float max = 0.2f, int bounce = 2)
    {
        return (float time) => { return max * (1 - time * time) * Mathf.Sin(Mathf.PI * bounce * time * time); };
    }

    public static EffectUpdate Wave(float max = 0.2f, int bounce = 2)
    {
        return (float time) => { return max * Mathf.Sin(Mathf.PI * bounce * time); };
    }

    public static EffectUpdate Explosion(float max = 0.2f)
    {
        return (float time) => { return max * Mathf.Sqrt(Mathf.Sin(Mathf.Pow(time, 0.75f) * Mathf.PI)); };
    }

    public static EffectGroup SpringGroup()
    {
        return (float max, int bounce) => { return Spring(max, bounce); };
    }

    public static EffectGroup WaveGroup()
    {
        return (float max, int bounce) => { return Wave(max, bounce); };
    }

    public static EffectGroup ExplosionGroup()
    {
        return (float max, int bounce) => { return Explosion(max); };
    }

    /*  Get Behaviour
    *      NOTE: For movement animations, changing the directionVector can
    *      modify you effect.
    */

    public static EffectBehaviour GetBehaviour(EffectUpdate update, Vector2 directionVector)
    {
        return ((float time) => { return directionVector * update(time); });
    }

    public static EffectBehaviour BezierEffectBehaviour(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (float time) => { return (1 - time) * (1 - time) * (1 - time) * p0 + 3 * (1 - time) * (1 - time) * time * p1 + 3 * (1 - time) * time * time * p2 + time * time * time * p3; };
    }

    public static EffectBehaviour BezierEffectBehaviour(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        return (float time) => { return (1 - time) * (1 - time) * p0 + 2 * (1 - time) * time * p1 + time * time * p2; };
    }
}