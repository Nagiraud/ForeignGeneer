using System;
using System.Collections.Generic;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.manager;
using Godot;

public partial class Circuit : Node, IObservable
{
    public FixedInputOutputCraft _craft;
    public Recipe currentRecipe;
    private Timer _craftTimer;
    public Inventory inputInventory;
    public Inventory outputInventory;
    private List<IObserver> _observers = new List<IObserver>();

    // Liste pour stocker les positions des slots
    public List<Vector2> slotPositions = new List<Vector2>();

    public override void _Ready()
    {
        base._Ready();
        _craftTimer = new Timer();
        _craftTimer.Name = "CraftTimer";
        AddChild(_craftTimer);
        generateRandomRecipe();
        outputInventory = new Inventory(1);
        outputInventory.onInventoryUpdated += notify;
    }

    
    public void attach(IObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void detach(IObserver observer)
    {
        if (_observers.Contains(observer))
        {
            _observers.Remove(observer);
        }
    }

    public void notify(InterfaceType? interfaceType)
    {
        foreach (var observer in _observers)
        {
            observer.update(interfaceType);
        }
    }
    public void notify()
    {
        notify(null);
    }
    private void generateRandomRecipe()
    {
        var rand = new Random();
        currentRecipe = MiniGameManager.instance.listRecipe[rand.Next(MiniGameManager.instance.listRecipe.Count)];
        inputInventory = new Inventory(currentRecipe.input.Count);
        notify();
    }

    private void startCraft()
    {
        if (currentRecipe == null)
            generateRandomRecipe();
        
        _craft = new FixedInputOutputCraft(currentRecipe, inputInventory, outputInventory);
        _craft.craftTimer = _craftTimer;
        _craft.startCraft(onCraftFinished);
        notify();
    }

    private void onCraftFinished()
    {
        _craft.stopCraft();
        _craft.addOutput();
        notify();
    }

    private void onCraftChange()
    {
        generateRandomRecipe();
    }

    public void interact(InteractType interactType)
    {
        switch (interactType)
        {
            case InteractType.Modify:
                onCraftChange();
                break;
            case InteractType.Craft:
                startCraft();
                break;
        }
    }
}
