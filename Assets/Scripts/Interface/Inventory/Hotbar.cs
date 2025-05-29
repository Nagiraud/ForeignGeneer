
using Godot;
using System.Collections.Generic;

public partial class Hotbar : HBoxContainer
{
	[Export] public PackedScene slotUIPrefab; 
	public override void _Ready()
	{
		for(int i = 0; i < InventoryManager.Instance.hotbar.slots.Count; i++)
		{
			HotbarSlotUi slotUi = slotUIPrefab.Instantiate<HotbarSlotUi>();
			slotUi.initialize(i);
			AddChild(slotUi);
		}
	}
}
