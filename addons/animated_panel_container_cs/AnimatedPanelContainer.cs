using System;
using Godot;

[Tool]
public partial class AnimatedPanelContainer : PanelContainer
{
    [Export]
    private Tween.TransitionType DefaultTrans;
    [Export]
    private Tween.EaseType DefaultEase;
    // default Linear
    private Tween.TransitionType _trans;
    private Tween.EaseType _ease;
    protected Vector2 _currentMinimumSize;
    private Color _modulate;
    private Tween _tween;
    private bool _isHideCalled = false;
    private double _currentDuration = 0d;
    private double _delay = 0d;
    private string _classname = nameof(AnimatedPanelContainer);

    public AnimatedPanelContainer CreateNewTween()
    {
        if (_tween != null && _tween.IsRunning())
        {
            // end the Tween animation immediately, by setting delta longer than the whole duration of the Tween animation
            _tween.CustomStep(_currentDuration + 1);
        }
        _tween = CreateTween().SetParallel(false).SetEase(DefaultEase).SetTrans(DefaultTrans);
        _tween.Finished += () =>
        {
            _tween = null;
            _ease = DefaultEase;
            _trans = DefaultTrans;
        };
        return this;
    }


    private void CheckTween()
    {
        if (_tween == null) throw new InvalidOperationException("Tween is not set. Call CreateNewTween first.");
    }

    /// <summary>
    /// should be called before any Animated methods
    /// </summary>
    /// <param name="delay">double</param>
    /// <returns>AnimatedPanelContainer</returns>
    public AnimatedPanelContainer SetDelay(double delay)
    {
        _delay = delay;
        return this;
    }

    /// <summary>
    /// should be called before any Animated methods
    /// </summary>
    /// <param name="trans">Tween.TransitionType</param>
    /// <returns>AnimatedPanelContainer</returns>
    public AnimatedPanelContainer SetTrans(Tween.TransitionType trans)
    {
        _trans = trans;
        return this;
    }

    /// <summary>
    /// should be called before any Animated methods
    /// </summary>
    /// <param name="ease">Tween.EaseType</param>
    /// <returns>AnimatedPanelContainer</returns>
    public AnimatedPanelContainer SetEase(Tween.EaseType ease)
    {
        _ease = ease;
        return this;
    }

    public AnimatedPanelContainer AnimatedHide(double duration)
    {
        AnimatedTransparentHide(duration);
        AnimatedCollapse(duration);
        return this;
    }

    public AnimatedPanelContainer AnimatedShow(double duration)
    {
        AnimatedExpand(duration);
        AnimatedTransparentShow(duration);
        return this;
    }

    public new AnimatedPanelContainer Hide()
    {
        TransparentHide();
        Collapse();
        return this;
    }

    public new AnimatedPanelContainer Show()
    {
        Expand();
        TransparentShow();
        return this;
    }

    public AnimatedPanelContainer TransparentHide()
    {
        if (Modulate.A == 0) return this;
        _isHideCalled = true;
        _modulate = Modulate;
        InternalTransparentHide(new Color(Modulate.R, Modulate.G, Modulate.B, 0));
        return this;
    }

    public AnimatedPanelContainer AnimatedTransparentHide(double duration)
    {
        // why do you want to hide again
        if (Modulate.A == 0) return this;
        CheckTween();
        _isHideCalled = true;
        _modulate = Modulate;
        var t = _tween.TweenMethod(Callable.From((Color target) => InternalTransparentHide(target)), Modulate, new Color(Modulate.R, Modulate.G, Modulate.B, 0), duration).SetEase(_ease).SetTrans(_trans);
        if (_delay > 0)
        {
            t.SetDelay(_delay);
            _delay = 0;
        }
        if (_currentDuration != 0d) _currentDuration += duration;
        return this;
    }

    private void InternalTransparentHide(Color target)
    {
        InternalTransparentShowHide(target);
        if (Modulate.A == 0f)
        {
            // hide children when transparent, but rect size wont change due to minimum size
            ToggleChildVisibility(false);
        }
    }

    public AnimatedPanelContainer TransparentShow()
    {
        var modulate = Modulate;
        if (_isHideCalled)
        {
            // no need to show again
            if (modulate.A == _modulate.A) return this;
            // previously set to 0
            modulate = _modulate;
        }
        else
        {
            // not set to 0, so we set to 0
            // and restore
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
        }
        InternalTransparentShow(modulate);
        return this;
    }

    public AnimatedPanelContainer AnimatedTransparentShow(double duration)
    {
        CheckTween();
        // init state, so we make modulate.a = 0
        // and transition back to original a value
        var modulate = Modulate;
        if (_isHideCalled)
        {
            // no need to show again
            if (modulate.A == _modulate.A) return this;
            modulate = _modulate;
        }
        else
        {
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
        }
        var t = _tween.TweenMethod(Callable.From((Color target) => InternalTransparentShow(target)), Modulate, modulate, duration).SetEase(_ease).SetTrans(_trans);
        if (_delay > 0)
        {
            t.SetDelay(_delay);
            _delay = 0;
        }
        // duartion is not set => this is the first call has duration
        if (_currentDuration != 0d) _currentDuration += duration;
        return this;
    }

    private void InternalTransparentShow(Color target)
    {
        if (Modulate.A == 0f)
        {
            ToggleChildVisibility(true);
            Visible = true;
        }
        InternalTransparentShowHide(target);
    }

    private void InternalTransparentShowHide(Color target)
    {
        Modulate = target;
    }

    public AnimatedPanelContainer Collapse(Vector2 newSize = new Vector2())
    {
        // set minimum size
        CustomMinimumSize = Size;
        _currentMinimumSize = newSize;
        InternalExpandCollapse(_currentMinimumSize);
        return this;
    }

    public AnimatedPanelContainer AnimatedCollapse(double duration, Vector2 newSize = new Vector2())
    {
        CheckTween();
        _currentMinimumSize = newSize;
        var t = _tween.TweenMethod(Callable.From((Vector2 target) => InternalExpandCollapse(target)), Size, newSize, duration).SetEase(_ease).SetTrans(_trans);
        if (_delay > 0)
        {
            t.SetDelay(_delay);
            _delay = 0;
        }
        if (_currentDuration != 0d) _currentDuration += duration;
        return this;
    }

    public AnimatedPanelContainer Expand(Vector2 newSize = new Vector2())
    {
        if (newSize.Equals(Vector2.Zero))
        {
            // assuming nobody wants to do this in this func
            UpdateCustomMinimumSize();
            if (!Visible)
            {
                // hide all child
                ToggleChildVisibility(false);
                Visible = true;
            }
        }
        else
        {
            _currentMinimumSize = newSize;
        }
        InternalExpandCollapse(_currentMinimumSize);
        return this;
    }

    public AnimatedPanelContainer AnimatedExpand(double duration, Vector2 newSize = new Vector2())
    {
        CheckTween();
        if (newSize.Equals(Vector2.Zero))
        {
            // assuming nobody wants to do this in this func
            UpdateCustomMinimumSize();
            if (!Visible)
            {
                // hide all child
                ToggleChildVisibility(false);
                Visible = true;
            }
        }
        else
        {
            _currentMinimumSize = newSize;
        }
        var t = _tween.TweenMethod(Callable.From((Vector2 target) => InternalExpandCollapse(target)), Size, _currentMinimumSize, duration).SetEase(_ease).SetTrans(_trans);
        if (_delay > 0)
        {
            t.SetDelay(_delay);
            _delay = 0;
        }
        CustomMinimumSize = _currentMinimumSize;
        if (_currentDuration != 0d) _currentDuration += duration;
        return this;
    }

    private void UpdateCustomMinimumSize()
    {
        _currentMinimumSize = GetChildrenCombinedMinimumSize();
    }

    private void InternalExpandCollapse(Vector2 target)
    {
        CustomMinimumSize = target;
    }

    public AnimatedPanelContainer AnimatedScale(Vector2 scale, double duration)
    {
        CheckTween();
        _currentMinimumSize = Size * scale;
        var t = _tween.TweenMethod(Callable.From((Vector2 target) => InternalScale(target)), Scale, scale, duration).SetEase(_ease).SetTrans(_trans);
        CustomMinimumSize = _currentMinimumSize;
        if (_currentDuration != 0d) _currentDuration += duration;
        return this;
    }

    private void InternalScale(Vector2 target)
    {
        Scale = target;
    }

    private void ToggleChildVisibility(bool visible)
    {
        foreach (var child in GetChildren())
        {
            if (child is Control c)
            {
                c.Visible = visible;
            }
            if (child is Node2D n)
            {
                n.Visible = visible;
            }
        }
    }

    private Vector2 GetChildrenCombinedMinimumSize()
    {
        var ret = Vector2.Zero;
        foreach (var child in GetChildren())
        {
            if (child is Control c)
            {
                ret += c.GetCombinedMinimumSize();
            }
        }
        return ret;
    }
}
