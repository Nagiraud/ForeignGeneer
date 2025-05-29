using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Interface;
using Godot;

public partial class SlotUiBlocked : Control, BaseUi
{
    private TextureRect _icon;
    private Label _countLabel;
    private StackItem _stackItem;
    private TextureRect _backIcon;
    public void update(InterfaceType? interfaceType = null)
    {
        switch (interfaceType)
        {
            default:
                updateUi();
                break;
        }
    }

    public void detach()
    {
        
    }

    private void updateUi()
    {
        if(_stackItem == null)
            return;
        _backIcon.Texture = _stackItem.getResource().getInventoryIcon;
        _countLabel.Text = _stackItem.getStack().ToString();
    }
    public void initialize(object data)
    {
        _stackItem = (StackItem)data;
        _backIcon = GetNode<TextureRect>("Background/BackIcon");
        _countLabel = GetNode<Label>("CountLabel");
        updateUi();
    }
}