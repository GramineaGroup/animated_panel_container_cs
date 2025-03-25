using System;
using Godot;

[Tool]
public partial class AnimatedPanelContainer : PanelContainer
{
    private Vector2 _customMinimumSize;
    private Color _modulate;
    private double _delay = 0;
    private Tween _tween;

    public AnimatedPanelContainer ConnectTween(Tween tween)
    {
        _tween = tween;
        // reset delay and tween when finished
        _tween.Finished += () =>
        {
            _delay = 0;
            _tween = null;
        };
        return this;
    }

    public AnimatedPanelContainer AnimatedHide(double duration)
    {
        AnimatedTransparentHide(duration);
        AnimatedShrink(duration);
        return this;
    }

    public AnimatedPanelContainer AnimatedShow(double duration)
    {
        // divided by 2 so eveny Animated method has the same duartion
        AnimatedExpand(duration / 2);
        AnimatedTransparentShow(duration / 2);
        return this;
    }

    public AnimatedPanelContainer AnimatedTransparentHide(double duration)
    {
        CheckTween();
        // save current opacity
        _modulate = Modulate;
        var t = _tween.TweenMethod(Callable.From((Color target) => InternalAnimatedTransparentHide(target)), Modulate, new Color(Modulate.R, Modulate.G, Modulate.B, 0), duration);
        // append delay for chained call
        // hopefully godot can finish in time
        if (_delay > 0) t.SetDelay(duration);
        _delay = duration;
        return this;
    }

    private void InternalAnimatedTransparentHide(Color target)
    {
        InternalAnimatedTransparentShowHide(target);
        if (Modulate.A == 0f)
        {
            // hide children when transparent, but rect size wont change due to minimum size
            foreach (var child in GetChildren())
            {
                // Ideally, only 1 child
                (child as Control).Visible = false;
            }
        }
    }

    public AnimatedPanelContainer AnimatedTransparentShow(double duration)
    {
        CheckTween();
        var t = _tween.TweenMethod(Callable.From((Color target) => InternalAnimatedTransparentShow(target)), Modulate, _modulate, duration);
        if (_delay > 0) t.SetDelay(duration);
        _delay = duration;
        return this;
    }

    private void InternalAnimatedTransparentShow(Color target)
    {
        if (Modulate.A == 0f)
        {
            foreach (var child in GetChildren())
            {
                (child as Control).Visible = true;
            }
        }
        InternalAnimatedTransparentShowHide(target);
    }

    private void InternalAnimatedTransparentShowHide(Color target)
    {
        Modulate = target;
    }

    public AnimatedPanelContainer AnimatedShrink(double duration)
    {
        CheckTween();
        // save current size
        _customMinimumSize = Size;
        // set minimum size
        CustomMinimumSize = Size;
        var t = _tween.TweenMethod(Callable.From((Vector2 target) => InternalAnimatedExpandShrink(target)), Size, new Vector2(0, 0), duration);
        if (_delay > 0) t.SetDelay(duration);
        _delay = duration;
        return this;
    }

    public AnimatedPanelContainer AnimatedExpand(double duration)
    {
        CheckTween();
        var t = _tween.TweenMethod(Callable.From((Vector2 target) => InternalAnimatedExpandShrink(target)), Size, _customMinimumSize, duration);
        if (_delay > 0) t.SetDelay(duration);
        _delay = duration;
        return this;
    }

    private void InternalAnimatedExpandShrink(Vector2 target)
    {
        CustomMinimumSize = target;
    }

    private void CheckTween()
    {
        if (_tween == null) throw new InvalidOperationException("Tween is not set. Call ConnectTween first.");
    }
}