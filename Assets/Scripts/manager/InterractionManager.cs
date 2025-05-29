using ForeignGeneer.Assets.Scripts.Block;
using Godot;

namespace ForeignGeneer.Assets.Scripts.manager;

public partial class InterractionManager:Node
{
    public static InterractionManager instance;
    
    public override void _Ready()
    {
        if(instance == null)
            instance = this;
    }

    public void Interact(InteractType interactType)
    {
        RayCast3D raycast = Player.Instance.raycast;
        if (raycast != null)
        {
            raycast.ForceRaycastUpdate();
            if (raycast.IsColliding())
            {
                var collider = raycast.GetCollider();
                if(collider is IInteractable interactable)
                    interactable.interact(interactType);
            }
        }

    }
    /// <summary>
    /// Récupère la position mondiale ciblée par le raycast.
    /// </summary>
    /// <returns>La position mondiale du point de collision, ou Vector3.Zero si aucun objet n'est touché.</returns>
    public Vector3 getWorldCursorPosition()
    {
        RayCast3D raycast = Player.Instance.raycast;
        raycast.TargetPosition = new Vector3(0, -10 ,0);
        if (raycast.IsColliding())
        {
            return raycast.GetCollisionPoint();
        }

        return Vector3.Zero;
    }
}