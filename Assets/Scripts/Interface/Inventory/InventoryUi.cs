using Godot;

public partial class InventoryUi : Control
{
    [Export] public PackedScene slotUiPackedScene;
    private HBoxContainer hotbarContainer;
    private GridContainer mainInventoryContainer;

    public override void _Ready()
    {
        hotbarContainer = GetNode<HBoxContainer>("Hotbar");
        mainInventoryContainer = GetNode<GridContainer>("MainInventory");

        InitializeInventoryUi();
        UpdateUi();
    }
    
    /// <summary>
    /// Initialise l'UI de l'inventaire.
    /// </summary>
    private void InitializeInventoryUi()
    {
        for (int i = 0; i < InventoryManager.Instance.HotbarSize; i++)  
        {
            var slot = slotUiPackedScene.Instantiate<SlotUI>();
            hotbarContainer.AddChild(slot);
            slot.initialize( InventoryManager.Instance.hotbar,i);
        }

        for (int i = 0; i < InventoryManager.Instance.mainInventorySize; i++)
        {
            var slot = slotUiPackedScene.Instantiate<SlotUI>();
            mainInventoryContainer.AddChild(slot);
            slot.initialize(InventoryManager.Instance.mainInventory,i);
        }
    }

    /// <summary>
    /// Met à jour l'UI de l'inventaire.
    /// </summary>
    public void UpdateUi()
    {
        for (int i = 0; i < InventoryManager.Instance.HotbarSize; i++)
        {
                var slot = hotbarContainer.GetChild(i) as SlotUI;
                slot?.initialize(InventoryManager.Instance.hotbar,i);
        }

        for (int i = 0; i < InventoryManager.Instance.mainInventorySize; i++)
        {
                var slot = mainInventoryContainer.GetChild(i) as SlotUI;
                slot?.initialize(InventoryManager.Instance.mainInventory,i);
            
        }
    }
}