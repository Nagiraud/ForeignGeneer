using Godot;
using System;

public partial class credits : Node
{
	private double delay;
	public override void _Ready()
	{
		delay = 5;
	}

	public override void _Process(double delta)
	{
		delay -= delta;
		if(delay <= 0){
			PackedScene newScene = ResourceLoader.Load<PackedScene>("res://Assets/Scenes/Menu/titleScreen.tscn");
			Node newSceneInstance = newScene.Instantiate();
			GetTree().Root.AddChild(newSceneInstance);
			GetTree().Root.GetNode("creditsScreen").QueueFree();
		}
	}
}
