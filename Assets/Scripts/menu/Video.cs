using Godot;
using System;

public partial class Video : BoxContainer
{
	[Export]
	public filterManager filter;
	private bool _fullscreen = false;
	private float _brightness = 1.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_fullscreen)
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		
	}

	private void _brightnessUpdate(float newBrightness)
	{
		_brightness = newBrightness;
	}
	private void _FullscreenToggled()
	{
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
		{
			GetNode<Button>("Label2/fullscreenButton").Text = "Off";
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		else
		{
			GetNode<Button>("Label2/fullscreenButton").Text = "On";
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
	}
	
	private void _DaltonismChange(int Index){
		filterManager.Instance.SetFilter(Index);
	}
	
}
