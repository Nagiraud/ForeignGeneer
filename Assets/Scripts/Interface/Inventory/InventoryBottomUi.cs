using System.Collections.Generic;
using System.Linq;
using Godot;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Interface;

public partial class InventoryBottomUi : Control, BaseUi
{
	[Export]private FlowContainer _flowContainer;
	[Export] private PackedScene _slotUi;
	[Export] private PackedScene _slotUiBlocked;
	private Recipe _recipe;
	public override void _Ready()
	{
		
	}
	public override void _Process(double delta)
	{
	}

	private void updateUi()
	{
		foreach (Node child in _flowContainer.GetChildren())
		{
			child.QueueFree();
		}
		updateSlotUi();
	}
	public void update(InterfaceType? interfaceType = null)
	{
		switch (interfaceType)
		{
			default:
				updateUi();
				break;	
		}
	}

	private void updateSlotUi()
	{
		foreach (Node child in _flowContainer.GetChildren())
		{
			child.QueueFree();
		}

		Inventory inv = InventoryManager.Instance.mainInventory;
		for (int i = 0; i < inv.slots.Count; i++)
		{
			StackItem stackItem = inv.slots[i];
			if (stackItem == null || _recipe.input.Any(recipeItem => recipeItem.getResource() == stackItem.getResource()))
			{
				SlotUI slotUi = _slotUi.Instantiate<SlotUI>();
				slotUi.initialize(inv, i);
				_flowContainer.AddChild(slotUi);
			}
			else
			{
				SlotUiBlocked slotUi = _slotUiBlocked.Instantiate<SlotUiBlocked>();
				slotUi.initialize(stackItem);
				_flowContainer.AddChild(slotUi);
			}
		}
	}
	public void detach()
	{
	}

	public void initialize(object data)
	{
		_recipe = (Recipe)data;
		updateSlotUi();
	}
}
