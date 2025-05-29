using System;
using System.Collections.Generic;
using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.Interface;
using ForeignGeneer.Assets.Scripts.Interface.Inventory;
using Godot;
using Godot.Collections;

public partial class InventoryManager : Node
{
    public static InventoryManager Instance { get; private set; }

    [Export] public int HotbarSize { get; private set; } = 5;
    [Export] private ItemStatic testItem;
    [Export] private ItemStatic testItem2;
    [Export] private ItemStatic testItem3;
    [Export] public int mainInventorySize { get; private set; } = 20;
    [Export] public int hotbarSize { get; private set; } = 5;
    public event Action OnHotbarSelectionChanged;
    public Inventory mainInventory { get; private set; }
    public Inventory hotbar { get; private set; }
    private StackItem _currentItemInMouse;
    public StackItem currentItemInMouse { get => _currentItemInMouse; private set => setCurrentItemInMouse(value); }

    public int currentSlotHotbar = 0;
    public PreviewObject currentPreview;
    public List<Inventory> inventory = new List<Inventory>();

    public override void _Ready()
    {
            Instance = this;
        mainInventory = new Inventory(mainInventorySize);
        hotbar = new Inventory(hotbarSize);
        
        inventory.Add(mainInventory);
        inventory.Add(hotbar);
        
        mainInventory.addItemToSlot(new StackItem(testItem, 50), 10);
        mainInventory.addItemToSlot(new StackItem(testItem, 67), 7);
        mainInventory.addItemToSlot(new StackItem(testItem2, 1), 1);
        mainInventory.addItemToSlot(new StackItem(testItem3, 1), 2);
        hotbar.addItemToSlot(new StackItem(testItem, 67), 1);
    }
    public void setCurrentItemInMouse(StackItem item)
    {
        // Détacher les observateurs de l'ancien item
        if (_currentItemInMouse != null)
        {
            _currentItemInMouse.OnStackModified -= OnMouseItemChanged;
        }

        _currentItemInMouse = item;

        // Attacher les observateurs au nouvel item
        if (_currentItemInMouse != null)
        {
            _currentItemInMouse.OnStackModified += OnMouseItemChanged;
            
            // Forcer la validation de l'item
            if (_currentItemInMouse.isEmpty() || _currentItemInMouse.getResource() == null)
            {
                _currentItemInMouse = null;
            }
        }

        UpdateMouseItemUI();
    }

    private void OnMouseItemChanged(StackItem item)
    {
        // Synchronisation immédiate
        if (item == null || item.isEmpty() || item.getResource() == null)
        {
            setCurrentItemInMouse(null);
        }
        else
        {
            UpdateMouseItemUI();
        }
    }

    private void UpdateMouseItemUI()
    {
        UiManager.instance?.closeUi("itemCursorUi");
        
        if (_currentItemInMouse != null && !_currentItemInMouse.isEmpty())
        {
            UiManager.instance?.openUi("itemCursorUi", _currentItemInMouse);
        }

        
    }

    // Méthodes pour manipuler l'item du curseur de manière sécurisée
    public void AddToMouseItem(int quantity)
    {
        if (currentItemInMouse == null) return;

        int remainingStack = currentItemInMouse.getStack() + quantity;

        // Vérifier que l'item peut être ajouté à l'inventaire
        if (remainingStack <= currentItemInMouse.getResource().getMaxStack) {
            currentItemInMouse.setStack(remainingStack);
        } else {
            // Si la pile dépasse la capacité maximale, on laisse juste la partie du stack que l'inventaire peut accepter
            int overflow = remainingStack - currentItemInMouse.getResource().getMaxStack;
            currentItemInMouse.setStack(currentItemInMouse.getResource().getMaxStack);

            // Retourner l'overflow à l'inventaire
            StackItem overflowItem = new StackItem
            {
                Resource = currentItemInMouse.getResource(),
                Stack = overflow
            };
            addItemToInventory(overflowItem);
        }

        if (currentItemInMouse.isEmpty()) {
            setCurrentItemInMouse(null);
        }
    }


    public void RemoveFromMouseItem(int quantity)
    {
        if (currentItemInMouse == null) return;
        
        currentItemInMouse.subtract(quantity);
        
        if (currentItemInMouse.isEmpty())
        {
            setCurrentItemInMouse(null);
        }
    }
    private Vector3 FindNearestFreeSpot(Vector3 origin)
    {
        Node3D player = Player.Instance;
        if (player == null)
        {
            GD.PrintErr("Player not found!", player);
            return origin;
        }
        return origin; 
    }

    private Vector3 AdjustHeightToGround(Vector3 position)
    {
        Node3D player = Player.Instance; 
        if (player == null)
        {
            GD.PrintErr("Player not found!");
            return position;
        }

        PhysicsDirectSpaceState3D spaceState = player.GetWorld3D().DirectSpaceState;
    
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(position + new Vector3(0, 2, 0), position - new Vector3(0, 5, 0));
    
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            return (Vector3)result["position"];
        }
    
        return position;
    }

    public int addItemToInventory(StackItem stack)
    {
        int looseItem = stack.getStack();
        bool added = false;

        foreach (Inventory inv in inventory)
        {
            int remainingStack = inv.addItem(stack);
            if (remainingStack == 0)
            {
                added = true;
                break;  
            }
            else
            {
                stack.setStack(remainingStack);
            }
        }

        return looseItem;
    }


    public StackItem getCurrentItem()
    {
        return hotbar.getItem(currentSlotHotbar);
    }
    public StackItem FindItem(ItemStatic item)
    {
        foreach (Inventory inv in inventory)
        {
            StackItem foundItem = inv.FindItem(item);
            if (foundItem != null)
            {
                return foundItem;
            }
        }
        return null;
    }
    public void removeCurrentItemToHotbar()
    {
        if (currentItemInMouse != null)
        {
            if (getCurrentItem() == null || getCurrentItem().getResource() != currentItemInMouse.getResource())
            {
                hotbar.addItemToSlot(currentItemInMouse, currentSlotHotbar);
            }
        }

        if (currentSlotHotbar + 1 < hotbarSize)
        {
            currentSlotHotbar++;
        }
        else
        {
            currentSlotHotbar = 0;
        }

        OnHotbarSelectionChanged?.Invoke();
        StopPreview();
    }

    
    public void addCurrentItemToHotbar()
    {
        if (currentItemInMouse != null)
        {
            if (getCurrentItem() == null || getCurrentItem().getResource() != currentItemInMouse.getResource())
            {
                hotbar.addItemToSlot(currentItemInMouse, currentSlotHotbar);
            }
        }

        if (currentSlotHotbar - 1 >= 0)
        {
            currentSlotHotbar--;
        }
        else
        {
            currentSlotHotbar = hotbarSize - 1;
        }

        OnHotbarSelectionChanged?.Invoke();
        StopPreview();
    }

    
    private bool isItemAuSol(string path)
    {
        PackedScene scene = GD.Load<PackedScene>(path);
        if (scene == null)
        {
            return false;
        }

        var instance = scene.Instantiate();
        bool isItemAuSol = instance is ItemAuSol;
        instance.QueueFree();
        return isItemAuSol;
    }

    /// <summary>
    /// Désactive la prévisualisation.
    /// </summary>
    public void StopPreview()
    {
        if (currentPreview != null)
        {
            currentPreview.Destroy();
            currentPreview = null;
        }
    }
}