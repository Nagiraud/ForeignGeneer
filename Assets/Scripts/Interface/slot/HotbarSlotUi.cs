using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Interface;
using Godot;

public partial class HotbarSlotUi: Panel,BaseUi
{
    [Export] private TextureRect _textureRect;
    [Export] private Label _label;
    private int _hotbarIndex;
    [Export] private StyleBoxFlat _selectedStyle;
    [Export] private StyleBoxFlat _normalStyle;
    public void update(InterfaceType? interfaceType = null)
    {
       updateUi();
    }

    private void updateUi()
    {
        StackItem _stackItem = InventoryManager.Instance.hotbar.slots[_hotbarIndex];
        if (_stackItem != null)
        {
            _label.Text = _stackItem.getStack().ToString();
            _textureRect.Texture = _stackItem.getResource().getInventoryIcon;
        }

        else
        {
            _textureRect.Texture = null;
            _label.Text =null;
        }
            
        
        GD.Print(_textureRect.Texture);
    }

    private void updateSelectedSlot()
    {
        if (InventoryManager.Instance.currentSlotHotbar == _hotbarIndex)
        {
            AddThemeStyleboxOverride("panel", _selectedStyle);
        }
        else
        {
            AddThemeStyleboxOverride("panel", _normalStyle);
        }
    }
    public override void _ExitTree()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnHotbarSelectionChanged -= updateSelectedSlot;
        }
    }
    public void detach()
    {
        InventoryManager.Instance.hotbar.detach(_hotbarIndex, this);
    }

    public void initialize(object data)
    {
        _hotbarIndex = (int)data;
        Inventory inv = InventoryManager.Instance.hotbar;
        inv.attach(_hotbarIndex, this);
        updateUi();
        InventoryManager.Instance.OnHotbarSelectionChanged += updateSelectedSlot;
        updateSelectedSlot();
    }
}