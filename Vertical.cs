using Godot;
using System;

public partial class Vertical : PanelContainer
{
    private AnimatedPanelContainer _progressBarContainer;
    private AnimatedPanelContainer _bottomContainer;
    private AnimatedPanelContainer _contentLabelContainer;
    private AnimatedPanelContainer _leftButtonPanelContainer;
    private AnimatedPanelContainer _rightButtonPanelContainer;
    private Button _actionButton;
    private Vector2 _size;
    private bool _isExpanded = true;
    private double _duration = 0.6;
    public override void _Ready()
    {
        _progressBarContainer = GetNode<AnimatedPanelContainer>("%ProgressBarContainer");
        _bottomContainer = GetNode<AnimatedPanelContainer>("%BottomContainer");
        _contentLabelContainer = GetNode<AnimatedPanelContainer>("%ContentLabelContainer");
        _leftButtonPanelContainer = GetNode<AnimatedPanelContainer>("%LeftButtonPanelContainer");
        _rightButtonPanelContainer = GetNode<AnimatedPanelContainer>("%RightButtonPanelContainer");
        _actionButton = GetNode<Button>("%ActionButton");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void OnExpandButtonPressed()
    {
        if (CustomMinimumSize.Equals(_size)) return;
        if (_isExpanded) return;
        var tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

        _leftButtonPanelContainer.ConnectTween(tween).AnimatedTransparentShow(_duration);
        _rightButtonPanelContainer.ConnectTween(tween).AnimatedTransparentShow(_duration);
        _bottomContainer.ConnectTween(tween).AnimatedShow(_duration);
        _contentLabelContainer.ConnectTween(tween).AnimatedShow(_duration);
        _progressBarContainer.ConnectTween(tween).AnimatedShow(_duration);

        tween.TweenProperty(this, "custom_minimum_size", _size, _duration);
        tween.Finished += () => _isExpanded = true;
    }

    private void OnActionButtonPressed()
    {
        // save size
        _size = Size;
        // set minimum size
        CustomMinimumSize = Size;
        var tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

        _leftButtonPanelContainer.ConnectTween(tween).AnimatedTransparentHide(_duration);
        _rightButtonPanelContainer.ConnectTween(tween).AnimatedTransparentHide(_duration);
        _bottomContainer.ConnectTween(tween).AnimatedHide(_duration);
        _contentLabelContainer.ConnectTween(tween).AnimatedHide(_duration);
        _progressBarContainer.ConnectTween(tween).AnimatedHide(_duration);

        tween.TweenProperty(this, "custom_minimum_size", new Vector2(0, 0), _duration);
        tween.Finished += () => _isExpanded = false;
    }
}
