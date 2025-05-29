using Godot;

namespace ForeignGeneer.Assets.Scripts.Interface.Slot;
public partial class MiniSlotUi : Control, BaseUi
{
    [Export] private TextureRect _textureRect;

    [Export] private string _overlayName;

    private Timer _hoverTimer;
    private StackItem _stackItem;
    public override void _Ready()
    {
        _hoverTimer = new Timer();
        _hoverTimer.WaitTime = 0.5f;
        _hoverTimer.OneShot = true;
        _hoverTimer.Timeout += showPopup;
        AddChild(_hoverTimer);

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    public void initialize(object data)
    {
        _stackItem = (StackItem)data;
        _textureRect.Texture = _stackItem.getResource().getInventoryIcon;
    }
    private void OnMouseEntered()
    {
        _hoverTimer.Start();
    }

    private void OnMouseExited()
    {
        _hoverTimer.Stop();
        UiManager.instance.closeUi(_overlayName);
    }

    private void showPopup()
    {
        UiManager.instance.openUi(_overlayName, _stackItem.getResource());
    }

    public void update(InterfaceType? interfaceType)
    {
    }

    public void detach()
    {
    }
}