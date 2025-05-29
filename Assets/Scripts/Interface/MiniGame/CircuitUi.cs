using System.Collections.Generic;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.Interface;
using ForeignGeneer.Assets.Scripts.manager;
using Godot;
using Godot.Collections;

namespace ForeignGeneer.Assets.Scripts.Interface.MiniGame;

public partial class CircuitUi : Control, BaseUi
{
    [Export] private InventoryBottomUi _inventoryBottomUi;
    [Export] private Button _craftButton;
    [Export] private Button _changeButton;
    [Export] private SlotUI _outputSlotUi;
    [Export] private Array<CircuitSlotUi> _slots;

    private Circuit _circuit;

    public override void _Ready()
    {
        base._Ready();
        _circuit = MiniGameManager.instance.circuit;

        _inventoryBottomUi.initialize(_circuit.currentRecipe);
        _outputSlotUi.initialize(_circuit.outputInventory, 0);

        _craftButton.Pressed += OnCraftButtonPressed;
        _changeButton.Pressed += OnChangeButtonPressed;

        InitCircuit();
    }

    private void InitCircuit()
    {
        int slotCount = Mathf.Min(_slots.Count, _circuit.currentRecipe.input.Count);
        
        for (int i = 0; i < slotCount; i++)
        {
            _slots[i].initialize(_circuit, i, false);
        }
    }

    private void OnCraftButtonPressed()
    {
        _circuit.interact(InteractType.Craft);
    }

    private void OnChangeButtonPressed()
    {
        _circuit.interact(InteractType.Modify);
        InitCircuit();
    }

    public void update(InterfaceType? interfaceType = null) {}

    public void detach()
    {
        _circuit.detach(this);
    }

    public void initialize(object data) {}
}