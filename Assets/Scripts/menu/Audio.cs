using Godot;
using System;

public partial class Audio : BoxContainer
{

	[Export]
	public AudioManager audio;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audio = new AudioManager();
		foreach(Node label in GetChildren()){
			if (label.GetChild(0) is HSlider bus) {
				bus.Value = audio.getBusVolume(bus.Name);
			}
		}
	}
	///<tooltip> emet les signaux pour les utiliser dans le script dedier</tooltip>
	private void _onMasterVolumeUpdate(float newVolume)
	{
		GD.Print("input receive Master");
		audio.SetBusVolume("Master", newVolume);
	}
	private void _onBGMUpdate(float newVolume)
	{
		GD.Print("input receive BGM");
		audio.SetBusVolume("BGM", newVolume);
	}
	private void _onSFXUpdate(float newVolume)
	{
		GD.Print("input receive SFX");
		audio.SetBusVolume("SFX", newVolume);
	}
	private void _onRMUpdate(float newVolume)
	{
		GD.Print("input receive RM");
		audio.SetBusVolume("RM", newVolume);
	}
}
