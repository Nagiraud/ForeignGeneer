using System;
using ForeignGeneer.Assets.Scripts.Block;
using Godot;

namespace ForeignGeneer.Assets.Scripts.block.playerStructure;

public partial class BaseStructure : Node3D,IPlayerStructure<ItemStatic>
{
    [Export]public ItemStatic itemStatic { get; set; }

    public void interact(InteractType interactType)
    {
        switch (interactType)
        {
            case InteractType.Dismantle:
                dismantle();
                break;
        }
    }

    public void dismantle()
    {
        if (itemStatic is NaturalSatic natural)
        {
            Random rand = new Random();
            InventoryManager.Instance.addItemToInventory(new StackItem(natural,rand.Next(natural.randomGiveItem)));
        }
        else
        {
            InventoryManager.Instance.addItemToInventory(new StackItem(itemStatic));
        }
        QueueFree();
    }
}