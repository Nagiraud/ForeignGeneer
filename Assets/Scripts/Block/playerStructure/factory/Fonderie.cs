using ForeignGeneer.Assets.Scripts;
using Godot;
using ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;
using ForeignGeneer.Assets.Scripts.manager;

public partial class Fonderie : PlayerBaseStructure, IInputFactory<CraftingFactoryStatic>, IOutputFactory<CraftingFactoryStatic>
{
	[Export] public string factoryUiName { get; set; }
	[Export] public string recipeUiName { get; set; }
	[Export]
	public CraftingFactoryStatic itemStatic
	{
		get => base.itemStatic as CraftingFactoryStatic;
		set => SetItemStatic(value);
	}

	public BaseCraft craft { get; set; }
	public Inventory input { get; set; }
	public Inventory output { get; set; }

	private Timer _craftTimer;
	private FonderieUi _fonderieUi;

	public override void _Ready()
	{
		base._Ready();
		init();

		_craftTimer = new Timer();
		_craftTimer.Name = "CraftTimer";
		AddChild(_craftTimer);
	}

	public void init()
	{
		input = new Inventory(itemStatic.recipeList.Count);
		output = new Inventory(1);
		input.onInventoryUpdated += onInventoryUpdated;
		output.onInventoryUpdated += onInventoryUpdated;
	}

	public override void _Process(double delta)
	{
		if (craft != null && craft.isCrafting)
		{
			craft.updateCraftProgress(delta);
			notify(InterfaceType.Progress);
		}
	}

	private void onInventoryUpdated()
	{
		if (craft != null && craft.canContinue())
		{
			startCraft();
		}
		notify();
	}

	public void setCraft(Recipe recipe)
	{
		if (recipe == null)
		{
			craft = null;
			return;
		}

		craft = new FixedInputOutputCraft(recipe, input, output);
		craft.craftTimer = _craftTimer;
		craft.startCraft(onCraftFinished);
		openUi();
	}

	private void startCraft()
	{
		if (EnergyManager.getInstance().isDown() || craft.isCrafting)
		{
			return;
		}
		craft.startCraft(onCraftFinished);
		notify();
	}

	private void onCraftFinished()
	{
		if (craft != null)
		{
			craft.stopCraft();
			craft.addOutput();
			if (craft.canContinue())
				startCraft();
		}
		notify();
	}

	public override void openUi()
	{
		closeUi();
		if (craft == null)
		{
			UiManager.instance.openUi(recipeUiName, this);
			_fonderieUi = null;
		}
		else
		{
			UiManager.instance.openUi(factoryUiName, this);
			_fonderieUi = (FonderieUi)UiManager.instance.getUi(factoryUiName);
			attach(_fonderieUi);
		}
	}

	public override void closeUi()
	{
		detach(_fonderieUi);
		_fonderieUi = null;
		UiManager.instance.closeUi();
	}

	public override void dismantle()
	{
		craft?.stopCraft();
		input.onInventoryUpdated -= onInventoryUpdated;
		output.onInventoryUpdated -= onInventoryUpdated;
		base.dismantle();
	}
	
}
