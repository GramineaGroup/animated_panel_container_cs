#if TOOLS
using Godot;
using System;

[Tool]
public partial class Plugin : EditorPlugin
{
	public override void _EnterTree()
	{
		var script = GD.Load<Script>("res://addons/animated_panel_container_cs/" + nameof(AnimatedPanelContainer) + ".cs");
		var texture = GD.Load<Texture2D>("res://addons/animated_panel_container_cs/Icon.png");
		AddCustomType(nameof(AnimatedPanelContainer), nameof(PanelContainer), script, texture);
	}

	public override void _ExitTree()
	{
		RemoveCustomType(nameof(AnimatedPanelContainer));
	}
}
#endif
