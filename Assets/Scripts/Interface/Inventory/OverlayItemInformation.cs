using Godot;

namespace ForeignGeneer.Assets.Scripts.Interface.Inventory;

public partial class OverlayItemInformation: Control,BaseUi
{
    [Export] Label _nameLabel = null!;
    [Export] Label _descriptionLabel = null!;
    [Export] TextureRect _icon = null!;
    ItemStatic _stackItem;
    public void initialize(object data)
    {
        if (data == null)  
            return;
        _stackItem = data as ItemStatic;
        _nameLabel.SetText(_stackItem?.GetName());
        _descriptionLabel.SetText(_stackItem?.getDescription());
        _icon.SetTexture(_stackItem?.getInventoryIcon);
        Position = GetViewport().GetMousePosition();
    }

    public void updateUi()
    {
       
    }

    public void close()
    {
        
    }

    public void update(InterfaceType? interfaceType)
    {
        
    }

    public void detach()
    {
        
    }
}