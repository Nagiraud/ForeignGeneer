using ForeignGeneer.Assets.Scripts;
using Godot;
using ForeignGeneer.Assets.Scripts.Interface;

public partial class CircuitSlotUi : Control, BaseUi, IObserver
{
    [Export] public TextureRect icon;
    [Export] public TextureRect background;
    [Export] private Label countLabel;
    
    private Circuit circuit;
    private int slotIndex;
    private bool isOutput;
    
    public override void _Ready()
    {
        base._Ready();
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        SizeFlagsVertical = SizeFlags.ShrinkCenter;
    }

    public void initialize(Circuit circuit, int index, bool output = false)
    {
        // Détacher de l'ancien circuit si existant
        if (this.circuit != null)
        {
            circuit.inputInventory.detach(slotIndex, this);
        }

        this.circuit = circuit;
        slotIndex = index;
        isOutput = output;
        
        // S'attacher au nouvel inventaire
        circuit.inputInventory.attach(index, this);
        
        updateUi();
        adjustSize();
    }

    private void adjustSize()
    {
        int totalItems = circuit.currentRecipe.input[slotIndex].Stack;
        Vector2 newSize = totalItems switch
        {
            2 => new Vector2(80, 80),
            3 => new Vector2(110, 110),
            _ => new Vector2(64, 64)
        };

        CustomMinimumSize = newSize;
        SetDeferred("size", newSize);
        PivotOffset = newSize / 2;
    }

    private void updateUi()
    {
        var item = circuit.inputInventory.getItem(slotIndex);
        var recipeItem = circuit.currentRecipe.input[slotIndex];
        
        if (IsInstanceValid(icon)) 
            icon.Texture = item?.getResource()?.getInventoryIcon;
        
        if (IsInstanceValid(background))
            background.Texture = recipeItem.getResource()?.getInventoryIcon;
        
        if (IsInstanceValid(countLabel))
            countLabel.Text = $"{item?.Stack ?? 0} / {recipeItem.Stack}";
        
        updateItemProgress();
    }
    
    private void updateItemProgress()
    {
        if (!IsInstanceValid(icon) || !IsInstanceValid(background))
            return;

        StackItem stackItem = circuit.inputInventory.getItem(slotIndex);
        int maxAllowed = circuit.currentRecipe.input[slotIndex].Stack;

        if (stackItem != null)
        {
            float progress = Mathf.Clamp((float)stackItem.Stack / maxAllowed, 0.3f, 1.0f);
            icon.Scale = new Vector2(progress, progress);
            icon.PivotOffset = icon.Size / 2;
            background.Visible = stackItem.Stack == 0;
        }
    }

    // Implémentation IObserver
    public void update(InterfaceType? interfaceType = null) => updateUi();
    
    public void detach()
    {
        if (circuit != null)
        {
            circuit.inputInventory.detach(slotIndex, this);
        }
    }

    public void initialize(object data) { }

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
        var slotItem = circuit.inputInventory.getItem(slotIndex);
        var recipeItem = circuit.currentRecipe.input[slotIndex];

        // Cas 1: Pas d'item dans le curseur
        if (currentMouseItem == null)
        {
            if (slotItem != null)
            {
                InventoryManager.Instance.setCurrentItemInMouse(slotItem);
                circuit.inputInventory.deleteItem(slotIndex);
            }
        }
        // Cas 2: Item dans le curseur
        else if (!isOutput)
        {
            // Vérifier si l'item correspond au slot de recette
            if (currentMouseItem.getResource() != recipeItem.getResource())
                return;

            // Cas 2a: Slot vide
            if (slotItem == null)
            {
                if (currentMouseItem.getStack() > recipeItem.Stack)
                {
                    var excess = currentMouseItem.getStack() - recipeItem.Stack;
                    currentMouseItem.setStack(excess);
                    circuit.inputInventory.addItemToSlot(
                        new StackItem(currentMouseItem.getResource(), recipeItem.Stack), 
                        slotIndex);
                }
                else
                {
                    circuit.inputInventory.addItemToSlot(currentMouseItem, slotIndex);
                    InventoryManager.Instance.setCurrentItemInMouse(null);
                }
            }
            // Cas 2b: Slot avec item du même type
            else if (slotItem.getResource() == currentMouseItem.getResource())
            {
                int total = slotItem.getStack() + currentMouseItem.getStack();
                if (total > recipeItem.Stack)
                {
                    int excess = total - recipeItem.Stack;
                    slotItem.setStack(recipeItem.Stack);
                    currentMouseItem.setStack(excess);
                }
                else
                {
                    slotItem.setStack(total);
                    InventoryManager.Instance.setCurrentItemInMouse(null);
                }
            }
        }
        
        circuit.inputInventory.notifyInventoryUpdated();
    }

    private void handleRightClick()
    {
        var slotItem = circuit.inputInventory.getItem(slotIndex);
        if (slotItem == null || slotItem.getStack() <= 0) return;

        var currentMouseItem = InventoryManager.Instance.currentItemInMouse;
        var recipeItem = circuit.currentRecipe.input[slotIndex];

        // Vérifier que l'item correspond à la recette
        if (currentMouseItem != null && currentMouseItem.getResource() != recipeItem.getResource())
            return;

        StackItem splitItem = slotItem.split();
        
        if (currentMouseItem == null)
        {
            InventoryManager.Instance.setCurrentItemInMouse(splitItem);
        }
        else if (currentMouseItem.getResource() == splitItem.getResource())
        {
            int excess = currentMouseItem.add(splitItem.getStack());
            if (excess > 0)
            {
                slotItem.setStack(slotItem.getStack() + excess);
            }
        }

        circuit.inputInventory.notifyInventoryUpdated();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            detach();
        }
        base.Dispose(disposing);
    }
}