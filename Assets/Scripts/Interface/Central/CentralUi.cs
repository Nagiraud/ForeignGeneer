using System;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;
using Godot;
using ForeignGeneer.Assets.Scripts.Interface;
using ForeignGeneer.Assets.Scripts.manager;

public partial class CentralUi : Control,BaseUi
{
	[Export] private PackedScene _slotUi;
	private CentralInput _central;
	private SlotUI _inputSlot;
	private ProgressBar _craftProgressBar;
	private HBoxContainer _inputList;
	private VBoxContainer _mainContainer;
	private TextEdit _craftText;
	private Button _resetCraftButton;
	private Label _electricityLabel;

	public override void _Ready()
	{
		base._Ready();
		_mainContainer = GetNode<VBoxContainer>("Machine/Container");
		_inputList = GetNode<HBoxContainer>("Machine/Container/InputList");
		_craftProgressBar = GetNode<ProgressBar>("Machine/Container/ProgressBar");
		_craftText = GetNode<TextEdit>("Machine/CraftText");
		_electricityLabel = GetNode<Label>("Machine/Quantity");
		_resetCraftButton = GetNode<Button>("Machine/Button");
		_resetCraftButton.Connect("pressed", new Callable(this, nameof(onResetCraftButtonPressed)));
	}

	/// <summary>
	/// Initializes the central interface. This method should be called once to initialize the UI.
	/// </summary>
	/// <param name="data">The central node associated with this interface.</param>
	public void initialize(Object data)
	{
		_central = (CentralInput)data;

		if (_inputSlot == null)
		{
			_inputSlot = _slotUi.Instantiate<SlotUI>();
			_inputSlot.initialize(_central.input, 0);
			_inputSlot.setBackgroundTexture(_central.craft.recipe.input[0].getResource().getInventoryIcon);
			_inputList.AddChild(_inputSlot);
		}

		_craftText.Text = "Résultat : Énergie";
		foreach (StackItem stack in _central.craft.recipe.input)
		{
			_craftText.Text += $"\n - {stack.getStack()} x {stack.getResource().GetName()}";
		}

		updateUi();
	}

	/// <summary>
	/// Updates the central interface UI. This method is called on each UI update.
	/// </summary>
	public void updateUi()
	{
		updateProgressBar();
		updateElectricity();
		_inputSlot.updateUi();
	}

	public void close()
	{
		_central.interact(InteractType.Close);
	}

	/// <summary>
	/// Updates the progress bar of the central.
	/// </summary>
	/// <param name="progress">The current progress (between 0 and 1).</param>
	public void updateProgressBar()
	{
		try
		{
			// Double vérification de la validité de la ProgressBar
			if (IsInstanceValid(_craftProgressBar))
			{
				_craftProgressBar.Value = _central.craft.craftProgress * 100;
			}
			else
			{
				_craftProgressBar = null; // Nettoyage si invalide
			}
		}
		catch (ObjectDisposedException)
		{
			_craftProgressBar = null;
			GD.PushWarning("ProgressBar access after disposal");
		}
	}

	/// <summary>
	/// Updates the electricity display.
	/// </summary>
	public void updateElectricity()
	{
		if (_electricityLabel != null && _central != null)
		{
			_electricityLabel.Text = $"Puissance : {_central.itemStatic.electricalCost} kW/s || Électricité totale : {EnergyManager.getInstance().getGlobalElectricity()} kW/s";
		}
	}
	/// <summary>
	/// Bouton retour
	/// </summary>
	private void onResetCraftButtonPressed()
	{
		_central.setCraft(null);
	}
	public void update(InterfaceType? interfaceType)
	{
		switch (interfaceType)
		{
			case InterfaceType.Progress:
				updateProgressBar();
				break;
			case InterfaceType.Energy:
				updateElectricity();
				break;
			case InterfaceType.Close:
				close();
				break;
			default:
				updateUi();
				break;
		}
	}
	public void detach()
	{
		_central.detach(this);
	}
}
