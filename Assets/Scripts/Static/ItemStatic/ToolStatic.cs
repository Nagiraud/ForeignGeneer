
using ForeignGeneer.Assets.Scripts.Block;
using Godot;
[GlobalClass]
public partial class ToolStatic : ItemStatic
{
    [Export] public Tool tool;
    public override void LeftClick()
    {
        RayCast3D raycast = Player.Instance.raycast;
        raycast.ForceRaycastUpdate();
        GD.Print(raycast.GetCollider());
        if (raycast.IsColliding())
        {
            Node collider = (Node)raycast.GetCollider();
            if (collider.IsInGroup(tool.ToString()))
            {
                IInteractable interactable = collider as IInteractable;
                interactable?.interact(InteractType.Dismantle);
            }
        }
    }
}