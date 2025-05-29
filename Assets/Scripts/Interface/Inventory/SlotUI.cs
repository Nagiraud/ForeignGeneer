using System;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Interface;
using Godot;

public partial class SlotUI : Control,BaseUi
{
    private TextureRect _icon;
    private Label _countLabel;
    private Inventory _inventory;
    private TextureRect _backIcon;
    private Panel _background;
    [Export]private bool _isOutputSlot;
    private int _slotIndex;
    private Timer _hoverTimer;
    private bool _isHovered = false;
    [Export] private string _overlay = "overlayItemInformation"; 

    public override void _Ready()
    {
        _hoverTimer = new Timer();
        _hoverTimer.WaitTime = 1.5f;
        _hoverTimer.OneShot = true;
        _hoverTimer.Connect("timeout", new Callable(this, nameof(showItemDescription)));
        AddChild(_hoverTimer);
    }
    
    /// <summary>
    /// Initialise le slot avec un inventaire et un index.
    /// </summary>
    public void initialize(Inventory inventory, int slotIndex, bool isOutputSlot = false)
    {
        // Détacher d'abord de l'ancien inventaire si nécessaire
        if (_inventory != null)
        {
            _inventory.detach(_slotIndex, this);
        }

        _inventory = inventory;
        _slotIndex = slotIndex;
        if(isOutputSlot)
            _isOutputSlot = isOutputSlot;

        _background = GetNode<Panel>("Background");
        _backIcon = GetNode<TextureRect>("Background/BackIcon");
        _icon = GetNode<TextureRect>("Icon");
        _countLabel = GetNode<Label>("CountLabel");
        
        // S'attacher au nouvel inventaire
        if (_inventory != null)
        {
            _inventory.attach(_slotIndex, this);
        }
        
        updateUi();
    }

    /// <summary>
    /// Met à jour l'affichage du slot en fonction de son contenu.
    /// </summary>
    public void updateUi()
    {
        var stackItem = _inventory.getItem(_slotIndex);
        if (_icon ==null || _countLabel == null)
            return;
        if (stackItem != null && stackItem.getStack() > 0 && stackItem.getResource() != null )
        {
            if(stackItem.getResource().getInventoryIcon!=null)
                _icon.Texture = stackItem.getResource().getInventoryIcon;
            _countLabel.Text = stackItem.getStack() > 1 ? stackItem.getStack().ToString() : "";
        }
        else
        {
            _icon.Texture = null;
            _countLabel.Text = "";
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                handleLeftClick();
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                handleRightClick();
            }
        }
    }

    private void handleLeftClick()
    {
        var currentMouseItem = InventoryManager.Instance.currentItemInMouse;
        var slotItem = _inventory.getItem(_slotIndex);
    
        bool inventoryChanged = false;  // Variable pour savoir si l'inventaire a changé
    
        // Cas 1: Pas d'item dans le curseur
        if (currentMouseItem == null)
        {
            if (slotItem != null)
            {
                // Prendre l'item du slot
                InventoryManager.Instance.setCurrentItemInMouse(slotItem);
                _inventory.deleteItem(_slotIndex);
                inventoryChanged = true;
            }
        }
        // Cas 2: Item dans le curseur et slot vide
        else if (slotItem == null && !_isOutputSlot)
        {
            // Placer l'item du curseur dans le slot
            _inventory.addItemToSlot(currentMouseItem, _slotIndex);
            InventoryManager.Instance.setCurrentItemInMouse(null);
            inventoryChanged = true;
        }
        // Cas 3: Items de même type - fusionner les stacks
        else if (slotItem.getResource() == currentMouseItem.getResource())
        {
            int remaining = slotItem.add(currentMouseItem.getStack());
            currentMouseItem.setStack(remaining);
        
            if (remaining <= 0)
            {
                InventoryManager.Instance.setCurrentItemInMouse(null);
            }
        
            inventoryChanged = true;
        }
        // Cas 4: Items différents - échanger les items
        else
        {
            StackItem tempSlotItem = new StackItem(slotItem.getResource(), slotItem.getStack());

            _inventory.deleteItem(_slotIndex);
            _inventory.addItemToSlot(currentMouseItem, _slotIndex);
            InventoryManager.Instance.setCurrentItemInMouse(tempSlotItem);

            inventoryChanged = true;
        }

        // Mise à jour de l'UI uniquement si un changement a eu lieu
        if (inventoryChanged)
        {
            updateUi();
        }

        updateUi();
    }


    private void handleRightClick()
    {
        var stackItem = _inventory.getItem(_slotIndex);
        if (stackItem != null && stackItem.getStack() > 0)
        {
            StackItem splitItem = stackItem.split();
            
            StackItem currentMouseItem = InventoryManager.Instance.currentItemInMouse;
            if (currentMouseItem == null)
            {
                InventoryManager.Instance.setCurrentItemInMouse(splitItem);
            }
            else if (splitItem != null && currentMouseItem.getResource() == splitItem.getResource())
            {
                int excess = currentMouseItem.add(splitItem.getStack()); 

                if (excess > 0)
                {
                    stackItem.setStack(stackItem.getStack() + excess);
                }

                InventoryManager.Instance.setCurrentItemInMouse(currentMouseItem);
            } 
            
        }
        else if (stackItem != null && stackItem.getStack() == 1)
        {
            InventoryManager.Instance.setCurrentItemInMouse(stackItem);
            _inventory.deleteItem(_slotIndex);
        }
        updateUi();
    }

    /// <summary>
    /// Définit la texture de fond du slot.
    /// </summary>
    public void setBackgroundTexture(Texture2D texture)
    {
        if (_backIcon != null)
        {
            _backIcon.Texture = texture;
        }
        else
        {
            GD.PrintErr("Background TextureRect is not assigned!");
        }
    }
    private void _on_mouse_entered()
    {
        _isHovered = true;
        _hoverTimer.Start();
    }

    private void _on_mouse_exited()
    {
        _isHovered = false;
        _hoverTimer.Stop();
        hideItemDescription();
    }

    private void showItemDescription()
    {
        if (_isHovered && _inventory.getItem(_slotIndex) != null)
        {
            UiManager.instance.openUi(_overlay, _inventory.getItem(_slotIndex).getResource());
        }
    }

    private void hideItemDescription()
    {
        UiManager.instance.closeUi(_overlay);
    }
    
    /// <summary>
    /// Vide le slot.
    /// </summary>
    public void clearSlot()
    {
        _icon.Texture = null;
        _countLabel.Text = "";
    }

    public void update(InterfaceType? interfaceType = null)
    {
        updateUi();
    }

    public void detach()
    {
        if (_inventory != null)
        {
            _inventory.detach(_slotIndex, this);
        }
    }

    public void initialize(object data)
    {
        
    }

    public override void _ExitTree()
    {
        detach();
        base._ExitTree();
    }
}
