using Godot;
using System;

public partial class Vertical : PanelContainer
{
    private AnimatedPanelContainer _progressBarContainer;
    private AnimatedPanelContainer _bottomContainer;
    private AnimatedPanelContainer _contentLabelContainer;
    private AnimatedPanelContainer _leftButtonPanelContainer;
    private AnimatedPanelContainer _rightButtonPanelContainer;
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

        _leftButtonPanelContainer.CreateNewTween().AnimatedTransparentShow(_duration);
        _rightButtonPanelContainer.CreateNewTween().AnimatedTransparentShow(_duration);
        _bottomContainer.CreateNewTween().AnimatedShow(_duration);
        _contentLabelContainer.CreateNewTween().AnimatedShow(_duration);
        _progressBarContainer.CreateNewTween().AnimatedShow(_duration);

        tween.TweenProperty(this, "custom_minimum_size", _size, _duration);
        tween.Finished += () => _isExpanded = true;
    }

    private void OnCollapseButtonPressed()
    {
        if (!_isExpanded) return;
        // save size
        _size = Size;
        // set minimum size
        CustomMinimumSize = Size;
        var tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

        _leftButtonPanelContainer.CreateNewTween().AnimatedTransparentHide(_duration);
        _rightButtonPanelContainer.CreateNewTween().AnimatedTransparentHide(_duration);
        _bottomContainer.CreateNewTween().AnimatedHide(_duration);
        _contentLabelContainer.CreateNewTween().AnimatedHide(_duration);
        _progressBarContainer.CreateNewTween().AnimatedHide(_duration);

        tween.TweenProperty(this, "custom_minimum_size", new Vector2(0, 0), _duration);
        tween.Finished += () => _isExpanded = false;
    }
}
