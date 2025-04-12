@tool
extends EditorPlugin
const PLUGIN_NAME = "AnimatedPanelContainer"

func _enter_tree() -> void:
	# Initialization of the plugin goes here.
	add_custom_type(PLUGIN_NAME, "PanelContainer", preload("AnimatedPanelContainer.cs"), preload("icon.svg"))

func _exit_tree() -> void:
	# Clean-up of the plugin goes here.
	remove_custom_type(PLUGIN_NAME)

## Get the current path of the plugin
func get_plugin_path() -> String:
	return get_script().resource_path.get_base_dir()
