using System;
using System.Collections.Generic;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.Interface;
using Godot;

public partial class FonderieUi : Control ,BaseUi
{
    private Fonderie _fonderie;
    [Export] private PackedScene _slotUi;
    private VBoxContainer _inputList;
    private VBoxContainer _outputContainer;
    private TextEdit _craftText;
    private SlotUI _outputSlot;
    private ProgressBar _progressBar;
    private List<SlotUI> _inputSlots = new List<SlotUI>();
    private Button _button;

    public override void _Ready()
    {
        base._Ready();
        _inputList = GetNode<VBoxContainer>("Machine/Container/InputList");
        _outputContainer = GetNode<VBoxContainer>("Machine/Container/OutputContainer");
        _craftText = GetNode<TextEdit>("Machine/CraftText");
        _progressBar = GetNode<ProgressBar>("Machine/Container/ProgressBar");
        _button = GetNode<Button>("Machine/Button");
        _button.Connect("pressed", new Callable(this, nameof(onButtonBack)));
    }

    /// <summary>
    /// Initialise l'interface avec les données de la fonderie.
    /// </summary>
    /// <param name="data">La fonderie à afficher.</param>
    public void initialize(Object data)
    {
        if (data is Fonderie fonderie)
        {
            _fonderie = fonderie;

            if (_fonderie.input != null && _fonderie.output != null)
            {
                _fonderie.input.onInventoryUpdated += updateInputSlot;
                _fonderie.output.onInventoryUpdated += updateOutputSlot;
            }

            initializeInputSlots();
            initializeOutputSlot();
            updateUi();
        }
    }

    private void initializeInputSlots()
    {
        foreach (var child in _inputList.GetChildren())
        {
            child.QueueFree();
        }
        _inputSlots.Clear();

        for (int i = 0; i < _fonderie.input.slots.Count; i++)
        {
            var slot = _slotUi.Instantiate<SlotUI>();
            slot.initialize(_fonderie.input, i);
            slot.setBackgroundTexture(_fonderie.craft.recipe.input[i].getResource().getInventoryIcon);
            _inputSlots.Add(slot);
            _inputList.AddChild(slot);
        }
    }

    private void initializeOutputSlot()
    {
        foreach (var child in _outputContainer.GetChildren())
        {
            child.QueueFree();
        }
        _outputSlot = _slotUi.Instantiate<SlotUI>();
        _outputSlot.initialize(_fonderie.output, 0, true);
        updateOutputSlotBackground(); 
        _outputContainer.AddChild(_outputSlot);  
    }

    private void updateCraftText()
    {
        if (_fonderie.craft != null && _fonderie.craft.recipe != null)
        {
            _craftText.Text = "Résultat : " + _fonderie.craft.recipe.output.getResource().GetName();
            foreach (StackItem stack in _fonderie.craft.recipe.input)
            {
                _craftText.Text += $"\n - {stack.getStack()} x {stack.getResource().GetName()}";
            }
        }
    }

    private void updateOutputSlotBackground()
    {
        if (_outputSlot != null && _fonderie.craft != null && _fonderie.craft.recipe != null)
        {
            _outputSlot.setBackgroundTexture(_fonderie.craft.recipe.output.getResource().getInventoryIcon);
        }
    }

    /// <summary>
    /// Met à jour l'interface utilisateur.
    /// </summary>
    public void updateUi()
    {
        if (_fonderie == null)
            return;
        updateInputSlot();
        updateOutputSlot();
        updateProgressBar();
        updateCraftText();  
        updateOutputSlotBackground();
    }

    private void updateInputSlot()
    {
        for (int i = 0; i < _fonderie.input.slots.Count; i++)
        {
            if (_inputSlots[i] != null)
            {
                var slotItem = _fonderie.input.getItem(i);
                if (slotItem != null)
                {
                    _inputSlots[i].updateUi();  
                }
                else
                {
                    _inputSlots[i].clearSlot(); 
                }
            }
        }
    }

    private void updateOutputSlot()
    {
        if (_outputSlot != null)
        {
            var outputItem = _fonderie.output.getItem(0);
            if (outputItem != null)
            {
                _outputSlot.updateUi();  
            }
            else
            {
                _outputSlot.clearSlot(); 
            }
        }
    }
    public void close()
    {
        _fonderie.interact(InteractType.Close);
    }

    /// <summary>
    /// Met à jour la barre de progression du craft.
    /// </summary>
    /// <param name="progress">La progression actuelle (entre 0 et 1).</param>
    public void updateProgressBar()
    {
        _progressBar.Value = _fonderie.craft.craftProgress * 100;
    }
    
    public override void _ExitTree()
    {
        if (_fonderie != null)
        {
            _fonderie.input.onInventoryUpdated -= updateInputSlot;
            _fonderie.output.onInventoryUpdated -= updateOutputSlot;
        }
    }

    private void onButtonBack()
    {
        _fonderie.setCraft(null);
        close();
    }
    public void update(InterfaceType? interfaceType)
    {
        switch (interfaceType)
        {
            case InterfaceType.Progress:
                updateProgressBar();
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
        _fonderie.detach(this);
    }
}
