using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.block.playerStructure;
using Godot;
using ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;
using ForeignGeneer.Assets.Scripts.manager;

public partial class CentralInput : PlayerBaseStructure, IInputFactory<CraftingFactoryStatic>
{
	[Export]
	public CraftingFactoryStatic itemStatic
	{
		get => base.itemStatic as CraftingFactoryStatic;
		set => SetItemStatic(value);
	}
	[Export] public string factoryUiName { get; set; }
	[Export] public string recipeUiName { get; set; }
	public Inventory input { get; set; }
	public BaseCraft craft { get; set; }

	private CentralUi _centralUi;
	private Timer _craftTimer;

	public override void _Ready()
	{
		base._Ready();
		input = new Inventory(itemStatic.recipeList.Count);
		input.onInventoryUpdated += onInventoryUpdated;
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

		craft = new FixedInputCraft(recipe, input);
		craft.craftTimer = _craftTimer;
		openUi();
	}

	private void startCraft()
	{
		if (craft.startCraft(onCraftFinished))
		{
			EnergyManager.getInstance().addGlobalElectricity(itemStatic.electricalCost);
		}
		notify(InterfaceType.Progress);
	}

	private void onCraftFinished()
	{
		GD.Print("passe");
		if (craft != null)
		{
			GD.Print("!= null");
			craft.stopCraft();
			EnergyManager.getInstance().removeGlobalElectricity(itemStatic.electricalCost);
			startCraft();
		}
		notify(InterfaceType.Progress);
	}

	public override void openUi()
	{
		closeUi();
		if (craft == null)
		{
			UiManager.instance.openUi(recipeUiName, this);
			_centralUi = null;
		}
		else
		{
			UiManager.instance.openUi(factoryUiName, this);
			_centralUi = (CentralUi)UiManager.instance.getUi(factoryUiName);
			attach(_centralUi);
		}
	}

	public override void closeUi()
	{
		detach(_centralUi);
		_centralUi = null;
		UiManager.instance.closeUi();
	}
	
}
