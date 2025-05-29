using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class Recipe : Resource
{
	[Export] public Godot.Collections.Array<StackItem> input = new();
	[Export] public StackItem output = null;

	[Export] public float duration { get; set; }

	public Recipe()
	{
	}
	
}
