using Godot;
using System;

public partial class Options : Control
{

	private PackedScene titleScene = ResourceLoader.Load<PackedScene>("res://Assets/Scenes/Menu/titleScreen.tscn");

	private void _volumeTabToggled(bool isToggled)
	{
		GetNode<BoxContainer>("soundTab/optionVolume").Visible = isToggled;
	}

	///<tooltip>if active the Video option tab is opened else it is closed</tooltip>
	private void _videoTabToggled(bool isToggled)
	{
		GetNode<BoxContainer>("videoTab/optionVideo").Visible = isToggled;
	}

	///<tooltip>if active the Control option tab is opened else it is closed</tooltip>
	private void _controlTabToggled(bool isToggled)
	{
		GetNode<BoxContainer>("controlTab/optionControl").Visible = isToggled;
	}
	///<tooltip>close the option settings</tooltip>
	private void _closeOption()
	{
		UiManager.instance.closeUi();
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
