using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

///<tooltip>manage all buttons in menu </tooltip>
public partial class MenuManager : Control
{

	///<tooltip>Fonction pour changer les scene</tooltip>
	 public void ChangeScene(string scenePath){
		// Load the new scene
		PackedScene newScene = ResourceLoader.Load<PackedScene>(scenePath);
		
		if (newScene != null){
			// instantiate the new scene
			Node newSceneInstance = newScene.Instantiate();
			GetTree().Root.GetNode("TitleScreen").CallDeferred("queue_free");
			// Add the new scene to the root of the scene tree
			GetTree().Root.AddChild(newSceneInstance);
		}
		else{
			GD.PrintErr("Failed to load scene: " + scenePath);
		}
	}

	///<tooltip>change scene to mainGame scene</tooltip>
	private void _onButtonStartDown(){
		ChangeScene("res://Assets/Scenes/main.tscn");
	}
	
	///<tooltip>change scene to mainGame scene</tooltip>
	private void _onNewGameButtonDown(){
		ChangeScene("res://Assets/Scenes/main.tscn");
	}
	
	///<tooltip>open the option settings</tooltip>
	private void _onOptionButtonDown(){
		UiManager.instance.openUi("MenuOptions");
	}
	///<tooltip>open the credit scene</tooltip>
	private void _onCreditButtonDown(){
		GD.Print("Credits opened");
		ChangeScene("res://Assets/Scenes/Menu/creditsScreen.tscn");
	}
	///<tooltip>close the game</tooltip>
	private void _onQuitButtonDown(){
		GetTree().Quit();
	}
}
