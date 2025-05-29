using ForeignGeneer.Assets.Scripts.Interface.Factory;
using Godot;

namespace ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;

public partial class CraftingTable: PlayerBaseStructure, IOutputFactory<CraftingFactoryStatic>
{
	[Export] private string _craftingUiName;
	[Export]
	public CraftingFactoryStatic itemStatic
	{
		get => base.itemStatic as CraftingFactoryStatic;
		set => SetItemStatic(value);
	}
	public BaseCraft craft { get; set; }
	public Inventory output { get; set; }
	private CraftingTableUi _craftingTableUi;
	private Timer _craftTimer;
	public override void _Ready()
	{
		base._Ready();
		output = new Inventory(1);
		_craftTimer = new Timer();
		_craftTimer.Name = "CraftTimer";
		AddChild(_craftTimer);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (craft != null && craft.isCrafting)
		{
			craft.updateCraftProgress(delta);
			notify(InterfaceType.Progress);
		}
			
	}

	public void setCraft(Recipe recipe)
	{
		if(craft is not null && craft.isCrafting)
			return;
		craft = new BulkCraftWithOutput(recipe,InventoryManager.Instance.inventory,output);
		craft.craftTimer = _craftTimer;
		craft.startCraft(onCraftFinished);
		notify();
	}

	public void onCraftFinished()
	{
		craft.stopCraft();
		craft.addOutput();
		notify();
	}
	public override void openUi()
	{
		closeUi();
		UiManager.instance.openUi(_craftingUiName, this);
		_craftingTableUi = (CraftingTableUi)UiManager.instance.getUi(_craftingUiName);
		attach(_craftingTableUi);
	}

	public override void closeUi()
	{
		detach(_craftingTableUi);
		_craftingTableUi = null;
		GD.Print(_observers.Count);
		UiManager.instance.closeUi();
	}

}
